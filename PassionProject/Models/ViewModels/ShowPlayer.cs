using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject.Models.ViewModels
{
    public class ShowPlayer
    {
        public PlayerDto player { get; set; }
        //information about the team the player plays for
        public TeamDto team { get; set; }

        //Information about all sponsors for that team
        public IEnumerable<InjuryDto> playerinjuries { get; set; }
    }
}