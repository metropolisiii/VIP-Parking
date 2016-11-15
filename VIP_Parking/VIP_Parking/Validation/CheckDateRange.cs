using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VIPParking.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CheckDateRange : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = (DateTime)value;
            if (dt.AddDays(1) < DateTime.UtcNow)
            {
                return new ValidationResult("Please enter a date greater than or equal to today");
            }
            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = ErrorMessageString;
            ModelClientValidationRule checkDateRangeRule = new ModelClientValidationRule();
            checkDateRangeRule.ErrorMessage = errorMessage;
            checkDateRangeRule.ValidationType = "checkdaterange"; // This is the name the jQuery adapter will use
            yield return checkDateRangeRule;
        }
    }
}

