using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.Helpers;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartBox.Business.Shared.Models;

namespace SmartBox.Business.Shared
{
    public class SharedServices
    {
        public static MailMessage BuildEmailLetter(string to, string subject, string body, System.Net.Mail.Attachment attachment = null)
        {

            MailMessage message = new MailMessage
            {
                IsBodyHtml = true,
                Subject = subject,
            };

            if (to.Contains(","))
            {
                foreach (var email in to.Split(","))
                {
                    message.To.Add(new MailAddress(email));
                }
            }
            else
            {
                message.To.Add(new MailAddress(to));
            }

            message.Body = body;

            if (attachment != null)
                message.Attachments.Add(attachment);

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
            message.AlternateViews.Add(htmlView);
            message.BodyEncoding = Encoding.UTF8;

            return message;
        }
        public static bool IsValidEmail(string email)
        {
            var isMatched = true;
            var emails = email.Split(';');
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            foreach (var mail in emails)
            {
                
                 var regex = new Regex(pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
                isMatched = regex.IsMatch(mail);
                if (!isMatched)
                {
                    return isMatched;
                }
            }
            
            
            return isMatched;
        }
        public static string GenerateOTP(short length, bool isNumberOnly)
        {
            var alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            var numbers = "1234567890";
            var characters = numbers;
            var otp = string.Empty;

            if (isNumberOnly)
            {
                characters += alphabets + small_alphabets + numbers;
            }

            characters = numbers;

            for (int i = 0; i < length; i++)
            {
                string character;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character, StringComparison.Ordinal) != -1);
                otp += character;
            }
            return otp;
        }

        //public static DateTime GetNextMonth()
        //{
        //    var date = DateTime.Now.AddMonths(1);
        //    return date;
        //}

        //public static DateTime GetPreviousMonth()
        //{
        //    var date = DateTime.Now.AddMonths(-1);
        //    return date;
        //}

        public static string LoadEmailTemplate()
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.GetFullPath("EmailTemplates/FWD.html")))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }

        public static string SetVerbMessage(bool isPlural)
        {
            return isPlural ? "are" : "is";
        }

        public static string HashPassword(string password)
        {
            var hash = Crypto.HashPassword(password);
            return hash;
        }

        public static bool VerifyPassword(string password)
        {
            var hash = Crypto.HashPassword(password);
            var verified = Crypto.VerifyHashedPassword(hash, password);

            return verified;
        }

        public static bool VerifyPassword(string password, string dbPassword)
        {
            var verified = Crypto.VerifyHashedPassword(dbPassword, password);

            return verified;
        }
        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(result);
        }

        public static string GenerateCompanyUserKeyId(int id, string companyName)
        {
            var year = DateTime.Now.Year;
            var seconds = DateTime.Now.Second;
            var milisec = DateTime.Now.Millisecond;
            double b = Math.Floor(Math.Log10(milisec) + 1);
            string prefix = "";

            if (companyName.Length == 2)
                prefix = "0" + companyName;
            else if (companyName.Length == 1)
                prefix = "00" + companyName;
            else
                prefix = companyName.Substring(0,3);

            string mili;
            if (b == 1)
                mili = string.Format("00{0}", milisec);
            else if (b == 2)
                mili = string.Format("0{0}", milisec);
            else
                mili = milisec.ToString();

            return $"C{prefix.ToUpper()}{year}{mili}{seconds}";
        }

        public static string GenerateLastUserKeyId(int id)
        {
            var year = DateTime.Now.Year;
            var seconds = DateTime.Now.Second;

            return id.ToString($"{year}0000000000{seconds}");
        }

        public static string GenerateReferenceId(string name)
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day.ToString();
            var seconds = DateTime.Now.Second.ToString();

            var format = "{0}_" + $"{year}.{month}.{day}.{seconds}";
            var refId = string.Format(format, name);

            return refId;
        }

        public static string GenerateCompanyKeyId(int id, string name, bool isParent)
        {
            var year = DateTime.Now.Year;
            var second = "";
            int l = 1 - name.Length % 2;
            var mid = name.Substring(name.Length / 2 - l, l + 1);
            
            mid = mid[..1];

            if (name.Length != 1)
                second = name.Substring(1, 1);
            else
                second = name[..1];

            var firstW = string.Concat(name[..1], second, mid).ToUpper();
            
            if (firstW.Length == 2)
                firstW += "0";
         
            if(isParent)
                return id.ToString($"{firstW}{year}0000{ DateTime.Now.Second}");
            else
                return id.ToString($"{firstW}F{year}00{ DateTime.Now.Second}");
        }
        public static async Task<FileReaderResponse> ReadFile(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    return new FileReaderResponse
                    {
                        Content = await reader.ReadToEndAsync()
                    };
                }
            }
            catch (Exception ex)
            {
                return new FileReaderResponse
                {
                    isError = true,
                    ErrorMessage = $"FileName: {filePath} & ErrorMessage: {ex.Message}"
                };
            }
        }



        #region Query Extension
        public static string InsertQueryBuilder(IEnumerable<string> fields)
        {
            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (string columnName in fields)
            {
                columns.Append($"{columnName}, ");
                values.Append($"@{columnName}, ");

            }
            string insertQuery = $"({ columns.ToString().TrimEnd(',', ' ')}) VALUES ({ values.ToString().TrimEnd(',', ' ')}) ";

            return insertQuery;
        }

        public static string UpdateQueryBuilder(List<string> fields)
        {            StringBuilder updateQueryBuilder = new StringBuilder();

            foreach (string columnName in fields)
            {
                updateQueryBuilder.AppendFormat("{0}=@{0}, ", columnName);
            }
            return updateQueryBuilder.ToString().TrimEnd(',', ' ');
        }


        #endregion

        //public static string RenderEmailContent(string emailTemplate, string messageContent)
        //{
        //    emailTemplate = emailTemplate.Replace(Constants.NotificationParameters.MessageContent, messageContent);
        //    return emailTemplate;
        //}

        //public static string RenderHTMLContent(string html, string messageContent)
        //{
        //    html = html.Replace(Constants.NotificationParameters.Content, messageContent);
        //    return html;
        //}
        //public static string RenderHTMLRecipients(string html, string messageRecipient)
        //{
        //    html = html.Replace(Constants.NotificationParameters.Recipient, messageRecipient);
        //    return html;
        //}

    }
}
