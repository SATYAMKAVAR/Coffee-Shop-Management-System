using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_1.Models
{
    public class OrderModel
    {
        public int? OrderID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string PaymentMode { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public double TotalAmount { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string ShippingAddress { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public int UserID { get; set; }
    }
    public class OrderUserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
