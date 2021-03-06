﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProject.Models
{
    
    public class Player
    {
        [Key]
        public int PlayerID { get; set; }

        public string PlayerName { get; set; }

        public int PlayerNumber { get; set; }

        public string PlayerPosition { get; set; }

        public string PlayerInjuryDescription { get; set; }

        public bool PlayerHasPic { get; set; }
        public string PlayerPicExtension { get; set; }

        //A player plays for one team
        [ForeignKey("Team")]
        public int TeamID { get; set; }
        public virtual Team Team { get; set; }

        //A player can have many injuries 
        public ICollection<Injury> Injuries { get; set; }
    }

    
    public class PlayerDto
    {
        public int PlayerID { get; set; }

        [DisplayName("Player Name")]
        public string PlayerName { get; set; }

        [DisplayName("Player Number")]
        public int PlayerNumber { get; set; }

        [DisplayName("Player Position")]
        public string PlayerPosition { get; set; }

        [DisplayName("Player Injury Description")]
        public string PlayerInjuryDescription { get; set; }

        public bool PlayerHasPic { get; set; }
        public string PlayerPicExtension { get; set; }

        public int TeamID { get; set; }
    }
}