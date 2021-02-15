using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject.Models.ViewModels
{
    public class UpdateInjury
    {
        //base information about the injury
        public InjuryDto injury { get; set; }
        //display all players that this injury is on
        public IEnumerable<PlayerDto> injuriedplayers { get; set; }
        //display players which could be injuried in a dropdownlist
        public IEnumerable<PlayerDto> allplayers { get; set; }
    }
}