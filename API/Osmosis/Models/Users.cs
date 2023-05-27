using Microsoft.AspNetCore.Mvc;
using Osmosis.Models.Validator;
using System.ComponentModel.DataAnnotations;

namespace Osmosis.Models
{
    public class Users
    {
        [Key] public int id { get; set; }
        [RequireWhenIsNew] public string? name { get; set; }
        [RequireWhenIsNew] public string? login { get; set; }
        [RequireWhenIsNew] public string? email { get; set; }
        [RequireWhenIsNew] public string? password { get; set; }
        [RequireWhenIsNew] public int id_role { get; set; }
        public bool active { get; set; } = true;
    }


    public class UserLogin 
    {
        [Required] public string username { get; set; }
        [Required] public string password { get; set; }
    }

}
