using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Osmosis.Models
{
    public class ActiveGuids
    {
        [Key] public int id { get; set; }
        public string guid { get; set; }
        public int id_user { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime? expirationDate { get; set; }
    }
}
