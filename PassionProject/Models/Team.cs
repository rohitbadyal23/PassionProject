using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProject.Models
{
    public class Team
    {
        public int TeamID { get; set; }
        
        public string TeamName { get; set; }

        public string TeamLocation { get; set; }

        public string TeamArena { get; set; }

        public bool TeamHasPic { get; set; }

        //For when the team has an image, recorded the extension (.png, .gif, .jpg, etc.)
        public string TeamPicExtension { get; set; }

        //A team can have many players
        public ICollection<Player> Players { get; set; }
    }

    public class TeamDto
    {
        public int TeamID { get; set; }

        [DisplayName("Team Name")]
        public string TeamName { get; set; }

        [DisplayName("Team Location")]
        public string TeamLocation { get; set; }

        [DisplayName("Team Arena")]
        public string TeamArena { get; set; }

        public bool TeamHasPic { get; set; }

        public string TeamPicExtension { get; set; }
    }
}