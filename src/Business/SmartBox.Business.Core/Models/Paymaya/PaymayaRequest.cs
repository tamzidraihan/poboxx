using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Paymaya
{
    public class Amount
    {
        public string value { get; set; }
    }

    public class Item
    {
        public Item()
        {
            amount = new Amount();
            totalAmount = new TotalAmount();
        }

        public Amount amount { get; set; }
        public TotalAmount totalAmount { get; set; }
        public string name { get; set; }
    }

    public class RedirectUrl
    {
        public string success { get; set; }
        public string failure { get; set; }
        public string cancel { get; set; }
    }

    public class PaymayaRequestModel
    {
        public PaymayaRequestModel()
        {
            totalAmount = new TotalAmount();
            buyer = new Buyer();
            items = new List<Item>();
            redirectUrl = new RedirectUrl();
        }
        public Buyer buyer { get; set; }
        public TotalAmount totalAmount { get; set; }
        public List<Item> items { get; set; }
        public string requestReferenceNumber { get; set; }
        public RedirectUrl redirectUrl { get; set; }
    }
    public class Buyer
    {
        public Contact contact { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
    public class Contact
    {
        public string phone { get; set; }
        public string email { get; set; }
    }
    public class TotalAmount
    {
        public string value { get; set; }
        public string currency { get; set; }
    }
}
