using System;
using System.ComponentModel.DataAnnotations;

namespace belt.Models
{
    public class Plan
    {
       [Key]
       public int Id {get; set;}
        public User User {get; set;}
        public Activ Activity {get; set;}
    }
}