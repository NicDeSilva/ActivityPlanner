using System;
using System.ComponentModel.DataAnnotations;

namespace belt.Models
{
    public class RegisterForm
    {
        [Required(ErrorMessage = "You must enter your name!")]
        [MinLength(2, ErrorMessage = "Name must be longer than 2 characters!")]
        [MaxLength(45, ErrorMessage = "Name must be shorter than 45 characters!")]
        public string Name {get; set;}

        [Required(ErrorMessage = "You must enter your email!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        public string Email {get; set;}
        
        [Required(ErrorMessage = "You must enter a password!")]
        [MinLength(8, ErrorMessage = "Password must be 8 or more characters!")]
        [MaxLength(20, ErrorMessage = "Password be 20 characters or less!")]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        [Required(ErrorMessage = "You must confirm your password!")]
        [MinLength(8, ErrorMessage = "Password must be 8 or more characters!")]
        [MaxLength(20, ErrorMessage = "Password be 20 characters or less!")]
        [DataType(DataType.Password)]
        public string ConfirmPw {get; set;}

    }
}