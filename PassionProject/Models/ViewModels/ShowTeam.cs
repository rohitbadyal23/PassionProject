using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PassionProject.Models.ViewModels
{
  public class ShowTeam
    {
        //Information about the team
        public TeamDto team { get; set; }

        //Information about all players on that team
        public IEnumerable<PlayerDto> teamplayers { get; set; }
    }
}