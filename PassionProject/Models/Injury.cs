using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProject.Models
{
    public class Injury
    {
        [Key]
        public int InjuryID { get; set; }

        public string InjuryName { get; set; }

        //Utilizes the inverse property to specify the "Many"
        //One Injury Many Players
        public ICollection<Player> Players { get; set; }
    }

    public class InjuryDto
    {
        public int InjuryID { get; set; }

        public string InjuryName { get; set; }
    }
}