using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;


namespace ActivityCenter.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [MinLength(2)]
        public string Name {get;set;}
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email {get;set;}
        [Required]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters.")]
        [UniqueChars]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        // Navigation Property Here
        public List<Association> EventsAttending {get;set;}
        public List<DojoActivity> ActivitiesCreated {get;set;}


        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

        // testing the PW for special characters
        public class UniqueCharsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    Regex rex = new Regex(@"^[a-zA-z]+[0-9]+[!@#$%&*]");
                    if (!rex.IsMatch((string)value))
                    {
                        return new ValidationResult("You must have at least 1 letter, 1 number, and 1 special character in your password.");
                    }

                }
                return ValidationResult.Success;
            }
        }
    }
}