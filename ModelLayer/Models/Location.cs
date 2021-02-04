using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    [Table("Location")]
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id{get; set;}

        [Required]
        public string Address{get; set;}

        public List<LocationLine> Items{get; set;} = new List<LocationLine>();


        public Location(){}
        public Location(string a)
        {
            Address = a;
        }
    }
}

/*
    Ask Mark:
    2. Proper C# Form - PascalCase Variables?
    3. One-Many, One-One Problem
    4. Ugly Docs

    Location l = repo.getLocation();
    l.items.length();
*/