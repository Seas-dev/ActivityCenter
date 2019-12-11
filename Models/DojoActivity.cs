using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ActivityCenter.Models
{
    public class DojoActivity
    {
        [Key]
        public int ActivityId {get;set;}
        [Required]
        [Display(Name="Activity Name")]
        public string ActName {get;set;}
        [Required]
        [Display(Name="Start Date")]
        [DataType(DataType.Date)]
        [FutureDate]
        // add future date after testing the "dont show the past events black belt validation"
        public DateTime ActStart {get;set;}
        // a derived field from the start time and the duration, will be filled out in the backend
        public DateTime ActEnd {get;set;}
        [Required]
        public string Description {get;set;}
        public int UserId{get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        //Navagation Properties
        public List<Association> Attendees {get;set;}
        public User Creator {get;set;}

        // The idea with this not mapped is to have the user enter a duration in minutes/hours/days
        // Then use that int to fill in the ActEnd field with the addition between start and duration
        // I'm hoping I can then use these 2 fields for validation so that the user can't double book an activity
        [NotMapped]
        [Required]
        // the number of minutes/hours/days
        public int DurationNum {get;set;}
        [NotMapped]
        [Required]
        // the designation of minuts/hours/days
        public string DurationLen {get;set;}
        // The Time an event starts, which will then be converted and then added to ActStart
        [NotMapped]
        [Required]
        [DataType(DataType.Time)]
        public string TimeStart {get;set;}

        // Custom Validations Here

        // Specifies if the date enterd is in the past, and if the date is set in the past, then return an error
        public class FutureDateAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime today = DateTime.Today;
                if (((DateTime)value) < today)
                {
                    return new ValidationResult("Date of activity must be in the future.");
                }
                return ValidationResult.Success;
            }
        }

    }
}