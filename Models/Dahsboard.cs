using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace belt.Models
{
    public class Dashboard
    {
        public User User {get; set;}
        public List<Activ> Activities {get; set;}
    }
}