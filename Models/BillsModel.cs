using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_1.Models
{
    public class BillsModel
    {
        public int? BillID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string BillNumber { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public DateTime BillDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "This Field is Required")]
        public int? OrderID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public double? TotalAmount { get; set; }
        public double? Discount { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public double? NetAmount { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public int UserID { get; set; }

    }
    public class BillsUserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
    public class BillsOrderDropDownModel
    {
        public int OrderID { get; set; }
        public int OrderNumber { get; set; }
    }
}
