using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace belt.Models
{
    public class User
    {
       [Key]
       public int Id {get; set;}
        public string Name {get; set;}

        public string Email {get; set;}
        public string PwHash {get; set;}

        public List<Plan> plans {get; set;}

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}

        public User()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}