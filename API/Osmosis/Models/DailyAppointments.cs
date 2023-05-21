using Osmosis.Services;
using System.ComponentModel.DataAnnotations;

namespace Osmosis.Models
{
    public class DailyAppointments
    {
        [Key] public int id { get; set; }
        [RequireWhenIsNew] public TimeSpan startTime { get; set; }
        [RequireWhenIsNew] public int id_doctor { get; set; }
    }
}
