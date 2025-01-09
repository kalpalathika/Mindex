using System;
using System.ComponentModel.DataAnnotations;

namespace challenge.DTO
{
    public class CompensationDto
    {
        [Required(ErrorMessage = "EmployeeID is required.")]
        public string EmployeeID { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "EffectiveDate is required.")]
        public DateTime EffectiveDate { get; set; }
    }
}
