using System.ComponentModel.DataAnnotations;

namespace Nice_Admin_1.Models
{
    public class UserModel
    {
        public int? UserID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public Boolean IsActive { get; set; }
    }
}
