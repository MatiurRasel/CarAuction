using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.Register
{
    public class RegisterViewModel
    {
        [Required]
        public string Email {get; set;}
        [Required]
        public string Password {get; set;}
         [Required]
         [DisplayName("User Name")]
        public string UserName {get; set;}
         [Required]
         [DisplayName("Full Name")]
        public string FullName {get; set;}
        public string ReturnUrl {get; set;}
        public string Button {get; set;}
    }
}