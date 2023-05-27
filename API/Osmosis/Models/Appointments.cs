using Osmosis.Models.Validator;
using System.ComponentModel.DataAnnotations;

namespace Osmosis.Models
{
    public class Appointment
    {
        [Key] public int id { get; set; }
        [RequireWhenIsNew] public string? patientName { get; set; }
        [RequireWhenIsNew] public string? patientDocument { get; set; }
        [RequireWhenIsNew] public DateTime? appointmentTime { get; set; }
        [RequireWhenIsNew] public int id_doctor { get; set; }
        [Required] public int id_status { get; set; }
    }
}
