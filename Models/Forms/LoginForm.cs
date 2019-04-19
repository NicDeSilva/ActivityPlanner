using System;
using System.ComponentModel.DataAnnotations;

namespace belt.Models
{
    public class LoginForm
    {
        [Required(ErrorMessage = "You must enter your email!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        public string Email1 {get; set;}
        
        [Required(ErrorMessage = "You must enter your password!")]
        [DataType(DataType.Password)]
        public string Password1 {get; set;}
    }
}