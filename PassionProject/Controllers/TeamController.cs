using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PassionProject.Models;
using PassionProject.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace PassionProject.Controllers
{
    public class TeamController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static TeamController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

           // HttpClientHandler handler = new HttpClientHandler()
           // {
           //     Proxy = HttpWebRequest.GetSystemWebProxy()
           // };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44375/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        }


        // GET: Team/List?{PageNum}
        //Page number set to 0 if the page number is not included
        public ActionResult List(int PageNum=0)
        {   
            //Gets all the teams
            string url = "teamdata/getteams";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                
                IEnumerable<TeamDto> SelectedTeams = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;
                // -- Start of Pagination Algorithm --

                // Find the total number of team
                int TeamCount = SelectedTeams.Count();
                // Number of players to display per page
                int PerPage = 8;
                // Determines the maximum number of pages (rounded up), assuming a page 0 start.
                int MaxPage = (int)Math.Ceiling((decimal)TeamCount / PerPage) - 1;

                // Lower boundary for Max Page
                if (MaxPage < 0) MaxPage = 0;
                // Lower boundary for Page Number
                if (PageNum < 0) PageNum = 0;
                // Upper Bound for Page Number
                if (PageNum > MaxPage) PageNum = MaxPage;

                // The Record Index of our Page Star
                int StartIndex = PerPage * PageNum;

                //Helps us generate the HTML which shows "Page 1 of ..." on the list view
                ViewData["PageNum"] = PageNum;
                ViewData["PageSummary"] = " " + (PageNum + 1) + " of " + (MaxPage + 1) + " ";

                // -- End of Pagination Algorithm --


                // Send back another request to get teams, this time according to our paginated logic rules
                // GET api/teamdata/getteamspage/{startindex}/{perpage}
                url = "teamdata/getteamspage/" + StartIndex + "/" + PerPage;
                response = client.GetAsync(url).Result;

                // Retrieve the response of the HTTP Request
                IEnumerable<TeamDto> SelectedTeamsPage = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;

                //Return the paginated of teamss instead of the entire list
                return View(SelectedTeamsPage);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Team/Details/5
        public ActionResult Details(int id)
        {
            ShowTeam ViewModel = new ShowTeam();
            string url = "teamdata/findteam/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                //Put data into Team data transfer object
                TeamDto SelectedTeam = response.Content.ReadAsAsync<TeamDto>().Result;
                ViewModel.team = SelectedTeam;

                url = "teamdata/getplayersforteam/" + id;
                response = client.GetAsync(url).Result;
                IEnumerable<PlayerDto> SelectedPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
                ViewModel.teamplayers = SelectedPlayers;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Team/Create
        public ActionResult Create()
        {
            UpdateTeam ViewModel = new UpdateTeam();
            //get information about players that could play for this team.
            string url = "playerdata/getplayers";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<PlayerDto> PotentialPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
            ViewModel.allplayers = PotentialPlayers;

            return View(ViewModel);
        }

        // POST: Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Team TeamInfo)
        {
            Debug.WriteLine(TeamInfo.TeamName);
            string url = "Teamdata/addTeam";
            Debug.WriteLine(jss.Serialize(TeamInfo));
            HttpContent content = new StringContent(jss.Serialize(TeamInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Teamid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Teamid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Team/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateTeam ViewModel = new UpdateTeam();
            string url = "teamdata/findteam/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                //Put data into Team data transfer object
                TeamDto SelectedTeam = response.Content.ReadAsAsync<TeamDto>().Result;
                ViewModel.team = SelectedTeam;

                //get information about players that could play for this team.
                url = "playerdata/getplayers";
                response = client.GetAsync(url).Result;
                IEnumerable<PlayerDto> PotentialPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
                ViewModel.allplayers = PotentialPlayers;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Team/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Team TeamInfo, HttpPostedFileBase TeamPic)
        {
            string url = "teamdata/updateteam/" + id;
            Debug.WriteLine(jss.Serialize(TeamInfo));
            HttpContent content = new StringContent(jss.Serialize(TeamInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                //only attempt to send player picture data if we have it
                if (TeamPic != null)
                {
                    Debug.WriteLine("Calling Update Image method.");
                    //Send over image data for team
                    url = "teamdata/updateteampic/" + id;

                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(TeamPic.InputStream);
                    requestcontent.Add(imagecontent, "TeamPic", TeamPic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }

                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Team/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "teamdata/findteam/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                //Put data into Team data transfer object
                TeamDto SelectedTeam = response.Content.ReadAsAsync<TeamDto>().Result;
                return View(SelectedTeam);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Team/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "teamdata/deleteteam/" + id;
            //post body is empty
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
