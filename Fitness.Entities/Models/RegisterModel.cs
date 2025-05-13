using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace FitnessManagement.Dtos
{
    public class RegisterModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string Role { get; set; }
        
      



    }

   
}