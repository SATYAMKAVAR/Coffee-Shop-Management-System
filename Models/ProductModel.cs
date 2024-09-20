﻿using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_1.Models
{
    public class ProductModel
    {
        public int? ProductID { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public double? ProductPrice { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "This is Required")]
        public int UserID { get; set; }

    }
    public class ProductUserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}