﻿using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_1.Models
{
    public class CustomerModel
    {

        public int? CustomerID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string CustomerName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string HomeAddress { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string Email { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string MobileNo { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string GSTNO { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string CityName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string PinCode { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public double? NetAmount { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public int UserID { get; set; }
    }
    public class CustomerUserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
