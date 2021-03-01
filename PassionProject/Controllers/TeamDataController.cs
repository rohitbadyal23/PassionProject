using System;
using System.IO;
using System.Web;
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
    public class TeamDataController : ApiController
    {
        //This variable is our database access point
        private PassionDataContext db = new PassionDataContext();

        /// <summary>
        /// Gets a list or Teams in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Teams including their ID, name, location and arena.</returns>
        /// <example>
        /// GET: api/TeamData/GetTeams
        /// </example>
        [ResponseType(typeof(IEnumerable<TeamDto>))]
        [Route("api/teamdata/getteams")]
        public IHttpActionResult GetTeams()
        {
            List<Team> Teams = db.Teams.ToList();
            List<TeamDto> TeamDtos = new List<TeamDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Team in Teams)
            {
                TeamDto NewTeam = new TeamDto
                {
                    TeamID = Team.TeamID,
                    TeamName = Team.TeamName,
                    TeamLocation = Team.TeamLocation,
                    TeamArena = Team.TeamArena,
                    TeamHasPic = Team.TeamHasPic,
                    TeamPicExtension = Team.TeamPicExtension
                };
                TeamDtos.Add(NewTeam);
            }

            return Ok(TeamDtos);
        }

        /// <summary>
        /// Gets a list of players in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input teamid</param>
        /// <returns>A list of players associated with the team</returns>
        /// <example>
        /// GET: api/TeamData/GetPlayersForTeam
        /// </example>
        [ResponseType(typeof(IEnumerable<PlayerDto>))]
        public IHttpActionResult GetPlayersForTeam(int id)
        {
            List<Player> Players = db.Players.Where(p => p.TeamID == id)
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

        
        // Gets a list of teams and has seperate pages
        [ResponseType(typeof(IEnumerable<TeamDto>))]
        [Route("api/teamdata/getteamspage/{StartIndex}/{PerPage}")]
        public IHttpActionResult GetTeamsPage(int StartIndex, int PerPage)
        {
            List<Team> Teams = db.Teams.OrderBy(t => t.TeamID).Skip(StartIndex).Take(PerPage).ToList();
            List<TeamDto> TeamDtos = new List<TeamDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Team in Teams)
            {
                TeamDto NewTeam = new TeamDto
                {
                    TeamID = Team.TeamID,
                    TeamName = Team.TeamName,
                    TeamLocation = Team.TeamLocation,
                    TeamArena = Team.TeamArena,
                    TeamHasPic = Team.TeamHasPic,
                    TeamPicExtension = Team.TeamPicExtension
                };
                TeamDtos.Add(NewTeam);
            }

            return Ok(TeamDtos);
        }

        /// <summary>
        /// Finds a particular Team in the database with a 200 status code. If the Team is not found, return 404.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <returns>Information about the Team, including Team id, name, location and arena,</returns>
        // <example>
        // GET: api/TeamData/FindTeam/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(TeamDto))]
        public IHttpActionResult FindTeam(int id)
        {
            //Find the data
            Team Team = db.Teams.Find(id);
            //if not found, return 404 status code.
            if (Team == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            TeamDto TeamDto = new TeamDto
            {
                TeamID = Team.TeamID,
                TeamName = Team.TeamName,
                TeamLocation = Team.TeamLocation,
                TeamArena = Team.TeamArena,
                TeamHasPic = Team.TeamHasPic,
                TeamPicExtension = Team.TeamPicExtension
            };


            //pass along data as 200 status code OK response
            return Ok(TeamDto);
        }

        /// <summary>
        /// Updates a Team in the database given information about the Team.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <param name="Team">A Team object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/TeamData/UpdateTeam/5
        /// FORM DATA: Team JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateTeam(int id, [FromBody] Team Team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Team.TeamID)
            {
                return BadRequest();
            }

            db.Entry(Team).State = EntityState.Modified;
            // Picture update is handled by another method
            db.Entry(Team).Property(t => t.TeamHasPic).IsModified = false;
            db.Entry(Team).Property(t => t.TeamPicExtension).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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
        /// Receives team picture data, uploads it to the webserver and updates the team's TeamHasPic option
        /// </summary>
        /// <param name="id">the team id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// POST: api/TeamData/UpdateTeamPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UpdateTeamPic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var TeamPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (TeamPic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(TeamPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/Teams/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Teams/"), fn);

                                //save the file
                                TeamPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the team haspic and picextension fields in the database
                                Team SelectedTeam = db.Teams.Find(id);
                                SelectedTeam.TeamHasPic = haspic;
                                SelectedTeam.TeamPicExtension = extension;
                                db.Entry(SelectedTeam).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Team Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                            }
                        }
                    }

                }
            }

            return Ok();
        }

        /// <summary>
        /// Adds a Team to the database.
        /// </summary>
        /// <param name="Team">A Team object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/TeamData/AddTeam
        ///  FORM DATA: Team JSON Object
        /// </example>
        [ResponseType(typeof(Team))]
        [HttpPost]
        public IHttpActionResult AddTeam([FromBody] Team Team)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Teams.Add(Team);
            db.SaveChanges();

            return Ok(Team.TeamID);
        }

        /// <summary>
        /// Deletes a Team in the database
        /// </summary>
        /// <param name="id">The id of the Team to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/TeamData/DeleteTeam/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteTeam(int id)
        {
            Team Team = db.Teams.Find(id);
            if (Team == null)
            {
                return NotFound();
            }
            if (Team.TeamHasPic && Team.TeamPicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Teams/" + id + "." + Team.TeamPicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
            }

            db.Teams.Remove(Team);
            db.SaveChanges();

            return Ok();
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
        /// Finds a Team in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <returns>TRUE if the Team exists, false otherwise.</returns>
        private bool TeamExists(int id)
        {
            return db.Teams.Count(e => e.TeamID == id) > 0;
        }
    }
}