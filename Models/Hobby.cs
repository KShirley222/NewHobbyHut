using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HobbyHut.Models
{
    public class Hobby
    {
        [Key]
        public int HobbyId { get; set; }
        [Required]
        [Display(Name="Hobby Name")]
        public string HobbyName { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Association> HobbyUser { get; set; }


    }
}