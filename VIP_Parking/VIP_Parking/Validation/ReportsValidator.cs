using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using VIP_Parking.ViewModels;

namespace VIPParking.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportsValidator : ValidationAttribute
    {
        // Have to override IsValid
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ReportsVM)validationContext.ObjectInstance;

            //Date fields must be filled out
            if (model.StartDate == null || model.EndDate == null)
                return new ValidationResult("A date range must be specified.");            

            //From date must be before To date
            DateTime startdate = DateTime.ParseExact(model.StartDate, "MM/dd/yyyy", null);
            DateTime enddate = DateTime.ParseExact(model.EndDate, "MM/dd/yyyy", null);
            if (startdate > enddate)
                return new ValidationResult("Start date must occur before the end date");

            return ValidationResult.Success;
        }
    }
}