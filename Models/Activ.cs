using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace belt.Models
{
    public class Activ
    {
       [Key]
       public int Id {get; set;}

       [Required(ErrorMessage = "You must provide a title!")]
        public string Title {get; set;}
        [Required(ErrorMessage = "You must provide a description!")]
        public string Description {get; set;}

        [Required(ErrorMessage = "You must provide a date!")]
        [DataType(DataType.Date)]
        public DateTime Date {get; set;}

        [Required(ErrorMessage = "You must provide a time!")]
        [NotMapped]
        [DataType(DataType.Time)]
        public TimeSpan Time {get; set;}

        [Required(ErrorMessage = "You must provide a duration!")]
        [DataType(DataType.Duration)]
        public TimeSpan Duration {get; set;}

        [Required(ErrorMessage = "You must provide a unit of time!")]
        [NotMapped]
        public string TimeUnit {get; set;}

        public User Creator {get; set;}
        public List<Plan> Guests {get; set;}

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}

        public Activ()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}