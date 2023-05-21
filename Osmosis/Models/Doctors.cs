using Osmosis.Services;
using System.ComponentModel.DataAnnotations;

namespace Osmosis.Models
{
    public class Doctors
    {
        [Key] public int id { get; set; }
        [RequireWhenIsNew] public string? name { get; set; }
        [RequireWhenIsNew] public string? specialty { get; set; }
        [RequireWhenIsNew] public TimeSpan? entryTime { get; set; }
        [RequireWhenIsNew] public TimeSpan? departureTime { get; set; }
        [RequireWhenIsNew] public TimeSpan? appointmentPeriod { get; set; }
        public bool active { get; set; } = true;
    }
}
