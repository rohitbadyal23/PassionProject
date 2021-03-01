using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject.Models.ViewModels
{
    //The View Model required to update a team
    public class UpdateTeam
    {
        //Information about the team
        public TeamDto team { get; set; }
        //Needed for a dropdownlist which presents the player with a choice of teams to play for
        public IEnumerable<PlayerDto> allplayers { get; set; }
    }
}