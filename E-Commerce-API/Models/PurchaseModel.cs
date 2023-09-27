using System;

namespace E_Commerce_API.Models
{
    public class PurchaseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime BuyingDate { get; set; }
        public int Quantity { get; set; }
        public double TotalPayment { get; set; }
        public int ProductId { get; set; }

    }
}
