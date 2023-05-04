using System.ComponentModel.DataAnnotations;

namespace Osmosis.Models
{
    public class UserRoles
    {
        [Key] public int id { get; set; }
        public string name { get; set; }
    }
}
