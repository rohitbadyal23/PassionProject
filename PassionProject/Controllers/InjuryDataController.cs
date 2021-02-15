using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject.Models;
using System.Diagnostics;

namespace PassionProject.Controllers
{
    public class InjuryDataController : ApiController
    {
        //This variable is our database access point
        private PassionDataContext db = new PassionDataContext();

        /// <summary>
        /// Gets a list or Injury in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Injuries including their ID, and name.</returns>
        /// <example>
        /// GET: api/InjuryData/GetInjuries
        /// </example>
        [ResponseType(typeof(IEnumerable<InjuryDto>))]
        public IHttpActionResult GetInjuries()
        {
            List<Injury> Injuries = db.Injuries.ToList();
            List<InjuryDto> InjuryDtos = new List<InjuryDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Injury in Injuries)
            {
                InjuryDto NewInjury = new InjuryDto
                {
                    InjuryID = Injury.InjuryID,
                    InjuryName = Injury.InjuryName
                };
                InjuryDtos.Add(NewInjury);
            }

            return Ok(InjuryDtos);
        }

        /// <summary>
        /// Gets a list or Players in the database associated with a particular sponsor. Returns a status code (200 OK)
        /// </summary>
        /// <param name="id">The input Injury id</param>
        /// <returns>A list of Players including their ID, name, number, position, and injury description</returns>
        /// <example>
        /// GET: api/PlayerData/GetTeamsForSponsor
        /// </example>
        [ResponseType(typeof(IEnumerable<PlayerDto>))]
        public IHttpActionResult GetPlayersForInjury(int id)
        {
            //sql equivalent
            //select * from Players
            //inner join playerinjuries on playerinjuries.playerid = players.playerid
            //inner join injuries on injuries.injuryid = playerinjuries.injuryid
            List<Player> Players = db.Players
                .Where(p => p.Injuries.Any(i => i.InjuryID == id))
                .ToList();
            List<PlayerDto> PlayerDtos = new List<PlayerDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Player in Players)
            {
                PlayerDto NewPlayer = new PlayerDto
                {
                    PlayerID = Player.PlayerID,
                    PlayerName = Player.PlayerName,
                    PlayerNumber = Player.PlayerNumber,
                    PlayerPosition = Player.PlayerPosition,
                    PlayerInjuryDescription = Player.PlayerInjuryDescription,
                    TeamID = Player.TeamID
                };
                PlayerDtos.Add(NewPlayer);
            }

            return Ok(PlayerDtos);
        }

        /// <summary>
        /// Gets a list or Players in the database NOT associated with a sponsor. These could be potentially sponsored teams.
        /// </summary>
        /// <param name="id">The input injury id</param>
        /// <returns>A list of Teams including their ID, name, number, position, and injury description.</returns>
        /// <example>
        /// GET: api/PlayerData/GetPlayersForInjury
        /// </example>
        [ResponseType(typeof(IEnumerable<PlayerDto>))]
        public IHttpActionResult GetPlayersNotInjuried(int id)
        {
            List<Player> Players = db.Players
                .Where(p => !p.Injuries.Any(i => i.InjuryID == id))
                .ToList();
            List<PlayerDto> PlayerDtos = new List<PlayerDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Player in Players)
            {
                PlayerDto NewPlayer = new PlayerDto
                {
                    PlayerID = Player.PlayerID,
                    PlayerName = Player.PlayerName,
                    PlayerNumber = Player.PlayerNumber,
                    PlayerPosition = Player.PlayerPosition,
                    PlayerInjuryDescription = Player.PlayerInjuryDescription,
                    TeamID = Player.TeamID
                };
                PlayerDtos.Add(NewPlayer);
            }

            return Ok(PlayerDtos);
        }

        /// <summary>
        /// Finds a particular Injury in the database with a 200 status code. If the Injury is not found, return 404.
        /// </summary>
        /// <param name="id">The Injury id</param>
        /// <returns>Information about the Injury, including Injury id, name</returns>
        // <example>
        // GET: api/InjuryData/FindInjury/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(InjuryDto))]
        public IHttpActionResult FindInjury(int id)
        {
            //Find the data
            Injury Injury = db.Injuries.Find(id);
            //if not found, return 404 status code.
            if (Injury == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            InjuryDto InjuryDto = new InjuryDto
            {
                InjuryID = Injury.InjuryID,
                InjuryName = Injury.InjuryName
            };


            //pass along data as 200 status code OK response
            return Ok(InjuryDto);
        }

        /// <summary>
        /// Updates a Injury in the database given information about the Injury.
        /// </summary>
        /// <param name="id">The Injury id</param>
        /// <param name="Injury">A Injury object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/InjuryData/UpdateInjury/5
        /// FORM DATA: Injury JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateInjury(int id, [FromBody] Injury Injury)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Injury.InjuryID)
            {
                return BadRequest();
            }

            db.Entry(Injury).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InjuryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Adds a Injury to the database.
        /// </summary>
        /// <param name="Injury">A Injury object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/Injuries/AddInjury
        ///  FORM DATA: Injury JSON Object
        /// </example>
        [ResponseType(typeof(Injury))]
        [HttpPost]
        public IHttpActionResult AddInjury([FromBody] Injury Injury)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Injuries.Add(Injury);
            db.SaveChanges();

            return Ok(Injury.InjuryID);
        }

        /// <summary>
        /// Deletes a Injury in the database
        /// </summary>
        /// <param name="id">The id of the Injury to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/Injuries/DeleteInjury/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteInjury(int id)
        {
            Injury Injury = db.Injuries.Find(id);
            if (Injury == null)
            {
                return NotFound();
            }

            db.Injuries.Remove(Injury);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a relationship between a particular player and a Injury
        /// </summary>
        /// <param name="playerid">The player id</param>
        /// <param name="injuryid">The Injury id</param>
        /// <returns>status code of 200 OK</returns>
        [HttpGet]
        [Route("api/injurydata/uninjury/{playerid}/{injuryid}")]
        public IHttpActionResult Uninjury(int playerid, int injuryid)
        {
            //First select the injury (also loading in player data)
            Injury SelectedInjury = db.Injuries
                .Include(i => i.Players)
                .Where(i => i.InjuryID == injuryid)
                .FirstOrDefault();

            //Then select the player
            Player SelectedPlayer = db.Players.Find(playerid);

            if (SelectedInjury == null || SelectedPlayer == null || !SelectedInjury.Players.Contains(SelectedPlayer))
            {

                return NotFound();
            }
            else
            {
                //Remove the Injury from the player
                SelectedInjury.Players.Remove(SelectedPlayer);
                db.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        /// Adds a relationship between a particular player and an injury
        /// </summary>
        /// <param name="playerid">The player id</param>
        /// <param name="injuryid">The Injury id</param>
        /// <returns>status code of 200 OK</returns>
        [HttpGet]
        [Route("api/injurydata/injury/{playerid}/{injuryid}")]
        public IHttpActionResult Injury(int playerid, int injuryid)
        {
            //First select the injury (also loading in player data)
            Injury SelectedInjury = db.Injuries
                .Include(i => i.Players)
                .Where(i => i.InjuryID == injuryid)
                .FirstOrDefault();

            //Then select the player
            Player SelectedPlayer = db.Players.Find(playerid);

            if (SelectedInjury == null || SelectedPlayer == null || SelectedInjury.Players.Contains(SelectedPlayer))
            {

                return NotFound();
            }
            else
            {
                //Add the injury from the player
                SelectedInjury.Players.Add(SelectedPlayer);
                db.SaveChanges();
                return Ok();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a Injury in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Injury id</param>
        /// <returns>TRUE if the Injury exists, false otherwise.</returns>
        private bool InjuryExists(int id)
        {
            return db.Injuries.Count(e => e.InjuryID == id) > 0;
        }

        private bool InjuryAssociated(int playerid, int injuryid)
        {
            return true;
        }
    }
}
