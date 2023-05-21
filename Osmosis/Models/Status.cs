using Osmosis.Services;
using System.ComponentModel.DataAnnotations;

namespace Osmosis.Models
{
    public class Status
    {
        [Key] public int id { get; set; }
        [RequireWhenIsNew] public string name { get; set; }
        [RequireWhenIsNew] public bool createDocument { get; set; }
    }
}
