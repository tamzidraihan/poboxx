using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SmartBox.Business.Core.Models.Payment;
using SmartBox.Business.Core.Models.PayMongo;
using SmartBox.Business.Services.Service.Payment;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using SmartBox.Business.Core.Models.Paymaya;
using SmartBox.Business.Shared;
using Microsoft.AspNetCore.Authorization;
using SmartBox.Business.Core.Entities.Payment;
using SmartBox.Business.Services.Service.Locker;
using SmartBox.Business.Core.Models.Booking;
using System.Linq;
using SmartBox.Business.Services.Service.AppMessage;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly PaymongoSettingsModel _paymongoSettingsModel;
        private readonly PaymayaSettingsModel _paymayaSettingsModel;
        private readonly ILockerService lockerService;
        private readonly IAppMessageService _appMessageService;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger,
                                 IOptions<PaymongoSettingsModel> paymongoSettingsModel,
                                 ILockerService lockerService,
                                 IOptions<PaymayaSettingsModel> paymayaSettingsModel, IAppMessageService appMessageService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _paymongoSettingsModel = paymongoSettingsModel.Value;
            _paymayaSettingsModel = paymayaSettingsModel.Value;
            this.lockerService = lockerService;
            _appMessageService = appMessageService;
        }


        /// <summary>
        /// Get a list of Payment Method for booking
        /// </summary>
        /// <returns> returns lisf available payment method</returns>
        /// <response code="200">PaymentMethodModel list</response>

        [HttpGet("GetPaymentMethod")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Authorize]
        public async Task<ActionResult<List<PaymentMethodModel>>> GetPaymentMethod()
        {
            var model = await _paymentService.GetPaymentMethod();
            return Ok(model);
        }

        /// <summary>
        /// Get a list of Payment Method for booking
        /// </summary>
        /// <returns> returns list of available payment method</returns>
        /// <response code="200">PaymentMethodModel list</response> 
        [HttpPost("CheckOut")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Authorize]
        public async Task<ActionResult<List<PaymentMethodModel>>> CheckOut()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.paymongo.com/v1/sources"),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "Authorization", "Basic " +  _paymongoSettingsModel.AuthorizationKey },
                },
                Content = new StringContent("{\"data\":{\"attributes\":{\"amount\":10000,\"redirect\":{\"success\":\"https://liztest.website2.me/success-authorized\",\"failed\":\"https://liztest.website2.me/failed-authorized\"},\"type\":\"gcash\",\"currency\":\"PHP\"}}}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return Ok(body);
            }
        }

        /// <summary>
        /// Create a payment source request to paymongo
        /// </summary>
        /// <remarks>
        ///  <p style ="font-size:large;">The following parameter to be pass:</p>
        ///  <br/>For paymongo paymentMethod
        ///  <ul style ="font-size:large;">
        ///     <li> 1 = gcash</li>
        ///     <li> 2 = grab_pay</li>
        ///  </ul>
        ///  <br/> For currency
        ///  <ul style ="font-size:large;">
        ///     <li> 1 = PHP</li>
        ///  </ul>
        ///  <br/> For the amount, according to the website of Paymongo:
        ///  <p style ="font-size:large;">
        ///   A positive integer with minimum amount of 10000. 
        ///  <br/>10000 is the smallest unit in cents. If you want to receive an amount of 100.00, the value that you should pass is 10000. 
        ///  <br/>If you want to receive an amount of 1500.50, the value that you should pass is 150050. 
        ///  <br/>The amount is also considered as the gross amount.
        ///  </p>
        /// </remarks>
        /// <returns> returns the redirect page url for user to authorize payment</returns>
        /// <response code="200">PostPaymongoSourceModel with IsSuccessful set to true </response> 
        [HttpPost("PaymongoCreateSource")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Authorize]
        public async Task<ActionResult<PostPaymentSourceModel>> PaymongoCreateSource(short paymentMethod, int amount, short currency)
        {
            var client = new HttpClient();
            var paymentTransaction = "";
            var selectedCurrency = "";
            var model = new PostPaymentSourceModel();

            if (paymentMethod == 1)
                paymentTransaction = GlobalConstants.PaymongoPayment.Gcash;
            if (paymentMethod == 2)
                paymentTransaction = GlobalConstants.PaymongoPayment.GrabPay;

            if (currency == 1)
                selectedCurrency = GlobalConstants.PaymongoCurrency.PHP;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_paymongoSettingsModel.PaymentSourcesUrl),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "Authorization",  string.Concat("Basic ", _paymongoSettingsModel.AuthorizationKey)},
                },
                Content = new StringContent("{\"data\":{\"attributes\":{\"amount\":" + amount + ",\"redirect\":{\"success\":\"" + _paymongoSettingsModel.SuccessAuthorizeUrl + "\",\"failed\":\"" + _paymongoSettingsModel.FailedAuthorizeUrl + "\"},\"type\":\"" + paymentTransaction + "\",\"currency\":\"" + selectedCurrency + "\"}}}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    model.IsSuccessful = true;
                    model.ReferenceId = JObject.Parse(body)["data"]["id"].ToString();
                    model.RedirectUrl = JObject.Parse(body)["data"]["attributes"]["redirect"]["checkout_url"].ToString();
                    model.Message = "You will now be redirected to the payment page ";
                    model.SystemMessage = "Creation of payment source was successful";
                    _logger.LogInformation("paymongo creation of payment source for id " + model.ReferenceId);

                    return Ok(model);
                }
                else
                {
                    model.IsSuccessful = false;
                    model.Message = JObject.Parse(body)["errors"][0]["detail"].ToString();
                    model.SystemMessage = "paymongo api return error on creation of payment source";
                    _logger.LogError(model.Message + paymentMethod);

                    return BadRequest(model);
                }

            }

            //model.IsSuccessful = false;
            //model.Message = "Unable to redirect to payment page";
            //model.SystemMessage = "Error on creating payment source";
            //_logger.LogError("paymongo failed on creation of payment source for " + paymentMethod);

            //return BadRequest(model);
        }

        [HttpPost("PaymongoCreatePaymentIntent")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Authorize]
        public async Task<ActionResult<List<PaymentMethodModel>>> PaymongoCreatePaymentIntent(int amount, string selectedCurrency)
        {
            var client = new HttpClient();
            var model = new PaymentReferenceModel();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_paymongoSettingsModel.PaymentIntentUrl),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "Authorization",  string.Concat("Basic ", _paymongoSettingsModel.AuthorizationKey) },
                },
                Content = new StringContent("{\"data\":{\"attributes\":{\"amount\":" + amount + ",\"payment_method_allowed\":[\"atome\",\"card\",\"dob\",\"paymaya\"],\"payment_method_options\":{\"card\":{\"request_three_d_secure\":\"any\"}},\"currency\":\"" + selectedCurrency + "\",\"capture_type\":\"automatic\"}}}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };

            using (var response = await client.SendAsync(request))
            {
                //response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var id = JObject.Parse(body)["data"]["id"].ToString();

                    _logger.LogInformation("Paymongo intent success creation of payment for " + id);
                    model.ReferenceId = id;
                    model.IsSuccessful = true;
                    model.Message = "Payment intent successfully made";
                    return Ok(model);
                }
                else
                {
                    _logger.LogError("Paymongo intent failed creation of payment for ");
                    model.IsSuccessful = false;
                    model.SystemMessage = "Paymongo intent failed creation of payment for ";
                    model.Message = "Payment intent failed";
                    return BadRequest(model);
                }
            }
        }

        [HttpPost("PaymongoWebhook")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<PaymentReferenceModel>> PaymongoWebhook()
        {
            _logger.LogInformation("paymongo webhook called");
            var rawRequestBody = await Request.GetRawBodyAsync();
            var model = new PaymentReferenceModel();

            if (rawRequestBody != null)
            {
                _logger.LogInformation("Paymongo webhook call with data");
                _logger.LogInformation(rawRequestBody);

                PaymentInfoModel info = new PaymentInfoModel
                {
                    WebhookId = JObject.Parse(rawRequestBody)["data"]["id"].ToString(),
                    PaymentSourceId = JObject.Parse(rawRequestBody)["data"]["attributes"]["data"]["id"].ToString(),
                    Amount = Convert.ToInt32(JObject.Parse(rawRequestBody)["data"]["attributes"]["data"]["attributes"]["amount"].ToString()),
                    SourceStatus = JObject.Parse(rawRequestBody)["data"]["attributes"]["type"].ToString(),
                    Type = JObject.Parse(rawRequestBody)["data"]["attributes"]["data"]["attributes"]["type"].ToString()
                };

                var ret = await _paymentService.SetPaymentInfo(info);
                if (ret > 0)
                {
                    _logger.LogInformation("Paymongo success creation of payment for " + info.PaymentSourceId);
                    model.IsSuccessful = true;
                    model.Message = "Payment successfully made";
                    _logger.LogInformation("Paymongo success creation was saved on data " + info.PaymentSourceId);

                    return Ok(model);
                }
                return Ok(model);
            }
            else
            {
                _logger.LogError("Paymongo webhook error - rawRequestBody is null");
                model.IsSuccessful = false;
                model.Message = "Payment failed";
                model.SystemMessage = "Paymongo webhook error - rawRequestBody is null";
                return BadRequest(model);
            }
        }

        /// <summary>
        /// Create a payment request to paymongo
        /// </summary>
        /// <param name="referenceId">Provide the the payment source id from paymongo to create a payment request</param>
        /// <returns> returns whether the payment was successful or not</returns>
        /// <response code="200">PostPaymongoSourceModel with IsSuccessful set to true </response> 
        [HttpPost("PaymongoRequestPayment")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<PaymentReferenceModel>> PaymongoRequestPayment(string referenceId)
        {
            _logger.LogInformation("paymongo webhook called");
            var model = new PaymentReferenceModel();

            var infoModel = await _paymentService.GetPaymentInfoModel(referenceId);
            if (infoModel != null)
            {
                var client = new HttpClient();

                var description = " creation of payment for " + infoModel.PaymentSourceId;

                if (infoModel.SourceStatus == "source.chargeable")
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(_paymongoSettingsModel.PaymentUrl),
                        Headers =
                            {
                                { "Accept", "application/json" },
                                { "Authorization", string.Concat("Basic ", _paymongoSettingsModel.AuthorizationKey) }, //"Basic c2tfdGVzdF90YWliTEhpcjdrMzdnM0huOTlkUVdYNHU6" 
                            },
                        Content = new StringContent("{\"data\":{\"attributes\":{\"amount\": " + infoModel.Amount + ",\"source\":{\"id\":\"" + infoModel.PaymentSourceId + "\",\"type\":\"source\"},\"currency\":\"PHP\",\"description\":\"" + description + "\"}}}")
                        {
                            Headers =
                                {
                                    ContentType = new MediaTypeHeaderValue("application/json")
                                }
                        }
                    };
                    using (var response = await client.SendAsync(request))
                    {

                        var body = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation(body);

                        model.ReferenceId = infoModel.PaymentSourceId;
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Paymongo success creation of payment for " + infoModel.PaymentSourceId);
                            model.IsSuccessful = true;
                            model.Message = "Payment successfully made";
                            return Ok(model);
                        }
                        else
                        {
                            _logger.LogError("Paymongo failed creation of payment for " + infoModel.PaymentSourceId);
                            model.IsSuccessful = false;
                            model.SystemMessage = "Paymongo failed creation of payment for " + infoModel.PaymentSourceId;
                            model.Message = "Payment failed";
                            return BadRequest(model);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Paymongo is not in source.chargeable unable to create payment request for id " + infoModel.PaymentSourceId);
                    model.IsSuccessful = false;
                    model.SystemMessage = "Paymongo is not in source.chargeable unable to create payment request for id " + infoModel.PaymentSourceId;
                    model.Message = "Payment failed";

                    return BadRequest(model);
                }
            }
            else
            {
                return BadRequest(model);
            }
        }

        [HttpGet("PaymayaCheckOut")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Authorize]
        public async Task<ActionResult<PostPaymentSourceModel>> PaymayaCheckOut(double totalAmount,
             string itemName, string selectedCurrency = "PHP",
             [FromQuery] int[] lockerDetailIdsForNewBooking = null,
             DateTime? StoragePeriodStartForNewBooking = null,
             DateTime? StoragePeriodEndForNewBooking = null,
             PaymentInternalType paymentType = PaymentInternalType.NewBooking,
             int? lockerTransactionIdOfExistingBooking = null,
             DateTime? newStoragePeriodEndDateForExtendBooking = null)
        {
            var model = new PostPaymentSourceModel();
            if (paymentType == PaymentInternalType.NewBooking)
            {
                if (lockerDetailIdsForNewBooking == null || lockerDetailIdsForNewBooking.Length < 1)
                {
                    model.IsSuccessful = false;
                    model.SystemMessage = model.Message = "lockerDetailIdForNewBooking is required for New Booking!";
                    _logger.LogError(model.Message);
                    return BadRequest(model);
                }
                if (!StoragePeriodStartForNewBooking.HasValue)
                {
                    model.IsSuccessful = false;
                    model.SystemMessage = model.Message = "StoragePeriodStartForNewBooking is required for New Booking!";
                    _logger.LogError(model.Message);
                    return BadRequest(model);
                }
                if (!StoragePeriodEndForNewBooking.HasValue)
                {
                    model.IsSuccessful = false;
                    model.SystemMessage = model.Message = "StoragePeriodEndForNewBooking is required for New Booking!";
                    _logger.LogError(model.Message);
                    return BadRequest(model);
                }
                if (StoragePeriodStartForNewBooking.HasValue && StoragePeriodEndForNewBooking.HasValue)
                {
                    foreach (var locker in lockerDetailIdsForNewBooking)
                    {
                        var existingBookings = await lockerService.GetActiveBookingCount(locker, StoragePeriodStartForNewBooking.Value,
                       StoragePeriodEndForNewBooking.Value);
                        if (existingBookings > 0)
                        {
                            model.IsSuccessful = false;
                            model.SystemMessage = model.Message = $"You cannot extend your booking to {StoragePeriodEndForNewBooking.Value} as a booking has already been made for this slot.";
                            _logger.LogError(model.Message);
                            return BadRequest(model);
                        }
                    }
                }
            }
            if (paymentType == PaymentInternalType.ExtendingBooking
                && (!lockerTransactionIdOfExistingBooking.HasValue
                || !newStoragePeriodEndDateForExtendBooking.HasValue))
            {
                model.IsSuccessful = false;
                model.SystemMessage = model.Message = "LockerTransactionId and NewStorageEndDate is required to Extend Booking!";
                _logger.LogError(model.Message);
                return BadRequest(model);
            }
            if (paymentType == PaymentInternalType.CancelingBooking
                && !lockerTransactionIdOfExistingBooking.HasValue)
            {
                model.IsSuccessful = false;
                model.SystemMessage = model.Message = "LockerTransactionId is required to Cancel Booking!";
                _logger.LogError(model.Message);
                return BadRequest(model);
            }
            if (paymentType == PaymentInternalType.ExtendingBooking
            && lockerTransactionIdOfExistingBooking.HasValue
            && newStoragePeriodEndDateForExtendBooking.HasValue)
            {
                var existingBooking = await lockerService.GetLockerBooking(lockerTransactionIdOfExistingBooking.Value);
                if (existingBooking == null)
                {
                    model.IsSuccessful = false;
                    model.SystemMessage = model.Message = "Booking is invalid!";
                    _logger.LogError(model.Message);
                    return BadRequest(model);
                }
                if (newStoragePeriodEndDateForExtendBooking <= existingBooking.StoragePeriodEnd)
                {
                    model.IsSuccessful = false;
                    model.SystemMessage = model.Message = "New date is invalid!";
                    _logger.LogError(model.Message);
                    return BadRequest(model);
                }
                var existingBookings = await lockerService.GetActiveBookingCount(existingBooking.LockerDetailId, existingBooking.StoragePeriodStart,
                    newStoragePeriodEndDateForExtendBooking.Value, lockerTransactionIdOfExistingBooking.Value);
                if (existingBookings > 0)
                {
                    model.IsSuccessful = false;
                    model.SystemMessage = model.Message = $"You cannot extend your booking to {newStoragePeriodEndDateForExtendBooking.Value} as a booking has already been made for this slot.";
                    _logger.LogError(model.Message);
                    return BadRequest(model);
                }
            }
            var client = new HttpClient();
            var reference = SharedServices.GenerateReferenceId(itemName);

            var paymentRequest = new PaymayaRequestModel();

            Item item = new Item
            {
                name = itemName
            };

            item.totalAmount.value = totalAmount.ToString();
            item.amount.value = totalAmount.ToString();

            paymentRequest.totalAmount.value = totalAmount.ToString();
            paymentRequest.totalAmount.currency = selectedCurrency;
            paymentRequest.requestReferenceNumber = reference;
            paymentRequest.items.Add(item);

            paymentRequest.redirectUrl.success = _paymayaSettingsModel.SuccessPaymentUrl;
            paymentRequest.redirectUrl.failure = _paymayaSettingsModel.FailedPaymentUrl;
            paymentRequest.redirectUrl.cancel = _paymayaSettingsModel.CancelPaymentUrl;

            string json = JsonConvert.SerializeObject(paymentRequest);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_paymayaSettingsModel.CheckOutUrl),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "Authorization", string.Concat("Basic ", _paymayaSettingsModel.AuthorizationKey) },
                },

                Content = httpContent
            };


            using (var response = await client.SendAsync(request))
            {

                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {


                    if ((paymentType == PaymentInternalType.CancelingBooking
                        || paymentType == PaymentInternalType.ExtendingBooking)
                        && lockerTransactionIdOfExistingBooking.HasValue)
                    {
                        model.ReferenceId = JObject.Parse(body)["checkoutId"].ToString();

                        await _paymentService.SetPaymentTransaction(new PaymentTransactionModel
                        {
                            Amount = totalAmount,
                            InternalStatus = PaymentInternalStatus.Pending,
                            InternalType = paymentType,
                            LockerTransactionsId = lockerTransactionIdOfExistingBooking,
                            TransactionId = model.ReferenceId,
                            NewStoragePeriodEndDate = newStoragePeriodEndDateForExtendBooking
                        }, true);
                    }
                    model.IsSuccessful = true;
                  
                    model.RedirectUrl = JObject.Parse(body)["redirectUrl"].ToString();
                    model.Message = "You will now be redirected to the payment page ";
                    model.SystemMessage = "Creation of Paymaya checkout url was successful";
                    _logger.LogInformation("Creation of Paymaya checkout url was successful " + model.ReferenceId);

                    return Ok(model);
                }
                else
                {
                    model.IsSuccessful = false;
                    model.SystemMessage = "Paymaya api return error on creation of checkout url source";
                    model.Message = model.SystemMessage;
                    string rest = "";
                    if (response.Content != null) {
                          rest = await response.Content.ReadAsStringAsync();
               
                    }
                    _logger.LogError(model.Message + "\\n" + rest);
                    return BadRequest(model);
                }
            }
        }
        [HttpPost("PaymayaCheckOutForNewBooking")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Authorize]
        public async Task<ActionResult<PostPaymentSourceModel>> PaymayaCheckOut(NewBookingCheckOutModel model)
        {
            var responseModel = new PostPaymentSourceModel();
            if (string.IsNullOrEmpty(model.ItemName))
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "ItemName is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (string.IsNullOrEmpty(model.SelectedCurrency))
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "SelectedCurrency is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (model.TotalAmount < 1)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "TotalAmount should greater than zero!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            var invalidBookings = model.Bookings.Any(v => v.LockerDetailId < 1);
            if (invalidBookings)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "lockerDetailId is required for all Bookings!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            foreach (var booking in model.Bookings)
            {
                var existingBookings = await lockerService.GetActiveBookingCount(booking.LockerDetailId, booking.StoragePeriodStart,
               booking.StoragePeriodEnd);
                if (existingBookings > 0)
                {
                    responseModel.IsSuccessful = false;

                   string res = _appMessageService.GetApplicationMessage(ApplicationMessageNumber.ErrorMessage.AlreadyBooked).Result;
                   res = res.Replace(MessageParameters.StartDate, booking.StoragePeriodStart.ToString());
                   res = res.Replace(MessageParameters.EndDate, booking.StoragePeriodEnd.ToString());

                   responseModel.SystemMessage = responseModel.Message = res;
                    _logger.LogError(responseModel.Message);
                    return BadRequest(responseModel);
                }
            }
            var client = new HttpClient();
            var reference = SharedServices.GenerateReferenceId(model.ItemName);

            var paymentRequest = new PaymayaRequestModel();

            Item item = new Item
            {
                name = model.ItemName,
            };

            Buyer buyer = new Buyer
            {
                contact = new Contact
                {
                    email = model.Email,
                    phone = model.Phone
                },
                firstName = model.FirstName,
                lastName = model.LastName,
            };
            paymentRequest.buyer = buyer;
            item.totalAmount.value = model.TotalAmount.ToString();
            item.amount.value = model.TotalAmount.ToString();

            paymentRequest.totalAmount.value = model.TotalAmount.ToString();
            paymentRequest.totalAmount.currency = model.SelectedCurrency;
            paymentRequest.buyer = buyer;
           
            paymentRequest.requestReferenceNumber = reference;
            paymentRequest.items.Add(item);

            paymentRequest.redirectUrl.success = _paymayaSettingsModel.SuccessPaymentUrl;
            paymentRequest.redirectUrl.failure = _paymayaSettingsModel.FailedPaymentUrl;
            paymentRequest.redirectUrl.cancel = _paymayaSettingsModel.CancelPaymentUrl;

            string json = JsonConvert.SerializeObject(paymentRequest);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_paymayaSettingsModel.CheckOutUrl),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "Authorization", string.Concat("Basic ", _paymayaSettingsModel.AuthorizationKey) },
                },

                Content = httpContent
            };


            using (var response = await client.SendAsync(request))
            {

                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    responseModel.IsSuccessful = true;
                    responseModel.ReferenceId = JObject.Parse(body)["checkoutId"].ToString();
                    responseModel.RedirectUrl = JObject.Parse(body)["redirectUrl"].ToString();
                    responseModel.Message = "You will now be redirected to the payment page ";
                    responseModel.SystemMessage = "Creation of Paymaya checkout url was successful";
                    _logger.LogInformation("Creation of Paymaya checkout url was successful " + responseModel.ReferenceId);

                    return Ok(responseModel);
                }
                else
                {
                    responseModel.IsSuccessful = false;
                    responseModel.SystemMessage = "Paymaya api return error on creation of checkout url source";
                    responseModel.Message = responseModel.SystemMessage;

                    string rest = "";
                    if (response.Content != null)
                    {
                        rest = await response.Content.ReadAsStringAsync();

                    }
                    _logger.LogError(responseModel.Message + "\\n" + rest);

                    return BadRequest(responseModel);
                }

            }
        }


        [HttpPost("PaymayaCheckOutForBookingExtention")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Authorize]
        public async Task<ActionResult<PostPaymentSourceModel>> PaymayaCheckOut(BookingExtentionCheckOutModel booking)
        {
            var responseModel = new PostPaymentSourceModel();

            if (booking.lockerTransactionId < 1)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "lockerTransactionId is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (string.IsNullOrEmpty(booking.ItemName))
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "ItemName is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (string.IsNullOrEmpty(booking.SelectedCurrency))
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "SelectedCurrency is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (booking.TotalAmount < 1)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "TotalAmount should greater than zero!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            var existingBooking = await lockerService.GetLockerBooking(booking.lockerTransactionId);
            if (existingBooking == null)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "Booking is invalid!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (booking.NewStoragePeriodEnd <= existingBooking.StoragePeriodEnd)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "New date is invalid!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            var existingBookings = await lockerService.GetActiveBookingCount(existingBooking.LockerDetailId, existingBooking.StoragePeriodStart,
                booking.NewStoragePeriodEnd, booking.lockerTransactionId);
            if (existingBookings > 0)
            {
                responseModel.IsSuccessful = false;

                string res = _appMessageService.GetApplicationMessage(ApplicationMessageNumber.ErrorMessage.BookingExtensionAlreadyExist).Result;
                res = res.Replace(MessageParameters.NewStoragePeriodEnd, booking.NewStoragePeriodEnd.ToString());
              
                responseModel.SystemMessage = responseModel.Message = res;
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }

            var client = new HttpClient();
            var reference = SharedServices.GenerateReferenceId(booking.ItemName);

            var paymentRequest = new PaymayaRequestModel();

            Item item = new Item
            {
                name = booking.ItemName,
            };
            Buyer buyer = new Buyer
            {
                contact = new Contact
                {
                    email = booking.Email,
                    phone = booking.Phone
                },
                firstName = booking.FirstName,
                lastName = booking.LastName,
            };
            paymentRequest.buyer = buyer;
            item.totalAmount.value = booking.TotalAmount.ToString();
            item.amount.value = booking.TotalAmount.ToString();

            paymentRequest.totalAmount.value = booking.TotalAmount.ToString();
            paymentRequest.totalAmount.currency = booking.SelectedCurrency;
            paymentRequest.requestReferenceNumber = reference;
            paymentRequest.items.Add(item);

            paymentRequest.redirectUrl.success = _paymayaSettingsModel.SuccessPaymentUrl;
            paymentRequest.redirectUrl.failure = _paymayaSettingsModel.FailedPaymentUrl;
            paymentRequest.redirectUrl.cancel = _paymayaSettingsModel.CancelPaymentUrl;

            string json = JsonConvert.SerializeObject(paymentRequest);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_paymayaSettingsModel.CheckOutUrl),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "Authorization", string.Concat("Basic ", _paymayaSettingsModel.AuthorizationKey) },
                },

                Content = httpContent
            };


            using (var response = await client.SendAsync(request))
            {

                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await _paymentService.SetPaymentTransaction(new PaymentTransactionModel
                    {
                        Amount = booking.TotalAmount,
                        InternalStatus = PaymentInternalStatus.Pending,
                        InternalType = PaymentInternalType.ExtendingBooking,
                        LockerTransactionsId = booking.lockerTransactionId,
                        TransactionId = reference,
                        NewStoragePeriodEndDate = booking.NewStoragePeriodEnd
                    });

                    responseModel.IsSuccessful = true;
                    responseModel.ReferenceId = JObject.Parse(body)["checkoutId"].ToString();
                    responseModel.RedirectUrl = JObject.Parse(body)["redirectUrl"].ToString();
                    responseModel.Message = "You will now be redirected to the payment page ";
                    responseModel.SystemMessage = "Creation of Paymaya checkout url was successful";
                    _logger.LogInformation("Creation of Paymaya checkout url was successful " + responseModel.ReferenceId);

                    return Ok(responseModel);
                }
                else
                {
                    responseModel.IsSuccessful = false;
                    responseModel.SystemMessage = "Paymaya api return error on creation of checkout url source";
                    responseModel.Message = responseModel.SystemMessage;

                    string rest = "";
                    if (response.Content != null)
                    {
                        rest = await response.Content.ReadAsStringAsync();

                    }
                    _logger.LogError(responseModel.Message + "\\n" + rest);


                    return BadRequest(responseModel);
                }

            }
        }
        [HttpPost("PaymayaCheckOutForCancelBooking")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Authorize]
        public async Task<ActionResult<PostPaymentSourceModel>> PaymayaCheckOut(BookingCancelCheckOutModel booking)
        {
            var responseModel = new PostPaymentSourceModel();
            if (booking.lockerTransactionId < 1)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "lockerTransactionId is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (string.IsNullOrEmpty(booking.ItemName))
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "ItemName is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (string.IsNullOrEmpty(booking.SelectedCurrency))
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "SelectedCurrency is required!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            if (booking.TotalAmount < 1)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "TotalAmount should greater than zero!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }
            var existingBooking = await lockerService.GetLockerBooking(booking.lockerTransactionId);
            if (existingBooking == null)
            {
                responseModel.IsSuccessful = false;
                responseModel.SystemMessage = responseModel.Message = "lockerTransactionId is invalid!";
                _logger.LogError(responseModel.Message);
                return BadRequest(responseModel);
            }

            var client = new HttpClient();
            var reference = SharedServices.GenerateReferenceId(booking.ItemName);

            var paymentRequest = new PaymayaRequestModel();

            Item item = new Item
            {
                name = booking.ItemName
            };

            Buyer buyer = new Buyer
            {
                contact = new Contact
                {
                    email = booking.Email,
                    phone = booking.Phone
                },
                firstName = booking.FirstName,
                lastName = booking.LastName,
            };
            paymentRequest.buyer = buyer;
            item.totalAmount.value = booking.TotalAmount.ToString();
            item.amount.value = booking.TotalAmount.ToString();

            paymentRequest.totalAmount.value = booking.TotalAmount.ToString();
            paymentRequest.totalAmount.currency = booking.SelectedCurrency;
            paymentRequest.requestReferenceNumber = reference;
            paymentRequest.items.Add(item);

            paymentRequest.redirectUrl.success = _paymayaSettingsModel.SuccessPaymentUrl;
            paymentRequest.redirectUrl.failure = _paymayaSettingsModel.FailedPaymentUrl;
            paymentRequest.redirectUrl.cancel = _paymayaSettingsModel.CancelPaymentUrl;

            string json = JsonConvert.SerializeObject(paymentRequest);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_paymayaSettingsModel.CheckOutUrl),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "Authorization", string.Concat("Basic ", _paymayaSettingsModel.AuthorizationKey) },
                },

                Content = httpContent
            };


            using (var response = await client.SendAsync(request))
            {

                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await _paymentService.SetPaymentTransaction(new PaymentTransactionModel
                    {
                        Amount = booking.TotalAmount,
                        InternalStatus = PaymentInternalStatus.Pending,
                        InternalType = PaymentInternalType.CancelingBooking,
                        LockerTransactionsId = booking.lockerTransactionId,
                        TransactionId = reference
                    });

                    responseModel.IsSuccessful = true;
                    responseModel.ReferenceId = JObject.Parse(body)["checkoutId"].ToString();
                    responseModel.RedirectUrl = JObject.Parse(body)["redirectUrl"].ToString();
                    responseModel.Message = "You will now be redirected to the payment page ";
                    responseModel.SystemMessage = "Creation of Paymaya checkout url was successful";
                    _logger.LogInformation("Creation of Paymaya checkout url was successful " + responseModel.ReferenceId);

                    return Ok(responseModel);
                }
                else
                {
                    responseModel.IsSuccessful = false;
                    responseModel.SystemMessage = "Paymaya api return error on creation of checkout url source";
                    responseModel.Message = responseModel.SystemMessage;

                    string rest = "";
                    if (response.Content != null)
                    {
                        rest = await response.Content.ReadAsStringAsync();

                    }
                    _logger.LogError(responseModel.Message + "\\n" + rest);


                    return BadRequest(responseModel);
                }
            }
        }

        [HttpPost("PaymayaWebhook")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<PaymentReferenceModel>> PaymayaWebhook()
        {
            _logger.LogInformation("paymaya webhook called");
            var rawRequestBody = await Request.GetRawBodyAsync();
            var model = new PaymentReferenceModel();

            if (rawRequestBody != null)
            {
                string type = "";
                double amount = 0;

                var status = JObject.Parse(rawRequestBody)["paymentStatus"];

                var status1 = JObject.Parse(rawRequestBody)["status"];
                _logger.LogInformation("paymaya webhook call with data");
                _logger.LogInformation("paymaya webhook call with status: " + Convert.ToString(status) + " " + Convert.ToString(status1));
                _logger.LogInformation(rawRequestBody);


                if (status != null)
                {

                    if (JObject.Parse(rawRequestBody)["paymentStatus"].ToString() == GlobalConstants.PayamayaStatus.PaymentSuccess)
                    {
                        try
                        {
                            _logger.LogInformation("paymaya webhook call with status: " + JObject.Parse(rawRequestBody)["paymentStatus"].ToString());

                            if (JObject.Parse(rawRequestBody)["fundSource"] != null)
                                if (JObject.Parse(rawRequestBody)["fundSource"]["type"] != null)
                                    type = JObject.Parse(rawRequestBody)["fundSource"]["type"].ToString();

                            if (JObject.Parse(rawRequestBody)["totalAmount"] != null)
                                if (JObject.Parse(rawRequestBody)["totalAmount"]["value"] != null)
                                    amount = Convert.ToDouble(JObject.Parse(rawRequestBody)["totalAmount"]["value"]);

                            PaymentTransactionModel info = new PaymentTransactionModel
                            {
                                TransactionId = JObject.Parse(rawRequestBody)["id"].ToString(),
                                Status = JObject.Parse(rawRequestBody)["paymentStatus"].ToString(),
                                Amount = amount,
                                Type = type,

                            };

                            var ret = await _paymentService.SetPaymentTransaction(info);

                            if (ret > 0)
                            {
                                _logger.LogInformation("Payamaya success creation of payment for " + info.TransactionId);
                                model.IsSuccessful = true;
                                _logger.LogInformation("Payamaya success creation was saved on data " + info.TransactionId);


                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error in maya webhook success " + ex.Message);
                        }
                    }
                    else if (JObject.Parse(rawRequestBody)["paymentStatus"].ToString() == GlobalConstants.PayamayaStatus.PaymentFailed)
                    {
                        try
                        {
                            _logger.LogInformation("paymaya webhook call with status: " + JObject.Parse(rawRequestBody)["paymentStatus"].ToString());
                            PaymentTransactionModel info = new PaymentTransactionModel
                            {
                                TransactionId = JObject.Parse(rawRequestBody)["id"].ToString(),
                                Status = GlobalConstants.PayamayaStatus.PaymentFailed
                            };

                            var ret = await _paymentService.SavePaymentTransactionInfo(info);



                            model.IsSuccessful = false;
                            model.Message = "Payment has failed. Please check your payment details.";
                            model.SystemMessage = "Payment has failed. Please check your payment details.";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error in maya webhook payment failed " + ex.Message);
                        }
                    }
                    else if (JObject.Parse(rawRequestBody)["paymentStatus"].ToString() == GlobalConstants.PayamayaStatus.PaymentExpired)
                    {
                        try
                        {
                            _logger.LogInformation("paymaya webhook call with status: " + JObject.Parse(rawRequestBody)["paymentStatus"].ToString());
                            PaymentTransactionModel info = new PaymentTransactionModel
                            {
                                TransactionId = JObject.Parse(rawRequestBody)["id"].ToString(),
                                Status = GlobalConstants.PayamayaStatus.PaymentExpired
                            };

                            var ret = await _paymentService.SavePaymentTransactionInfo(info);
                            model.IsSuccessful = false;
                            model.Message = "The payment link for this transaciton has expired. Please create a new transaction.";
                            model.SystemMessage = "The payment link for this transaciton has expired. Please create a new transaction..";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error in maya webhook payment expired " + ex.Message);
                        }
                    }
                    else if (JObject.Parse(rawRequestBody)["paymentStatus"].ToString() == GlobalConstants.PayamayaStatus.AuthFailed)
                    {
                        try
                        {
                            _logger.LogInformation("paymaya webhook call with status: " + JObject.Parse(rawRequestBody)["paymentStatus"].ToString());
                            PaymentTransactionModel info = new PaymentTransactionModel
                            {
                                TransactionId = JObject.Parse(rawRequestBody)["id"].ToString(),
                                Status = GlobalConstants.PayamayaStatus.AuthFailed
                            };

                            var ret = await _paymentService.SavePaymentTransactionInfo(info);
                            model.IsSuccessful = false;
                            model.Message = "The payment link for this transaciton has expired. Please create a new transaction.";
                            model.SystemMessage = "The payment link for this transaciton has expired. Please create a new transaction..";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error in maya webhook auth failed " + ex.Message);
                        }
                    }

                    return Ok(model);
                }
                else if (status1 != null)
                {
                    _logger.LogInformation("Paymaya webhook TEST success");
                    model.IsSuccessful = false;
                    model.Message = "Paymaya webhook TEST ONLY";

                
                }

                return Ok(model);

            }

            else
            {
                _logger.LogError("Paymaya webhook error - rawRequestBody is null");
                model.IsSuccessful = false;
                model.Message = "Payment failed";
                model.SystemMessage = "Paymaya webhook error - rawRequestBody is null";
                return BadRequest(model);
            }
            }
          

        [HttpGet("GetPaymayaPaymentStatus")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        //[Authorize]
        [AllowAnonymous]
        public async Task<ActionResult<PaymentTransactionModel>> GetPaymayaPaymentStatus(string referenceId)
        {
            var model = await _paymentService.GetPaymentTransaction(referenceId);
            return Ok(model);
        }
    }
}
