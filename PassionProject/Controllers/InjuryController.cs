using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PassionProject.Models;
using PassionProject.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace PassionProject.Controllers
{
    public class InjuryController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static InjuryController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44375/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }



        // GET: Injury/List
        public ActionResult List()
        {
            string url = "injurydata/getinjuries";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<InjuryDto> SelectedInjuries = response.Content.ReadAsAsync<IEnumerable<InjuryDto>>().Result;
                return View(SelectedInjuries);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Injury/Details/5
        public ActionResult Details(int id)
        {
            UpdateInjury ViewModel = new UpdateInjury();

            string url = "injurydata/findinjury/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                //Put data into Injury data transfer object
                InjuryDto SelectedInjury = response.Content.ReadAsAsync<InjuryDto>().Result;
                ViewModel.injury = SelectedInjury;

                //find players that are injuried by this injury
                url = "injurydata/getplayersforinjury/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Injury data transfer object
                IEnumerable<PlayerDto> SelectedPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
                ViewModel.injuriedplayers = SelectedPlayers;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }

        }

        // GET: Injury/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Injury/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Injury InjuryInfo)
        {
            Debug.WriteLine(InjuryInfo.InjuryName);
            string url = "injurydata/addinjury";
            Debug.WriteLine(jss.Serialize(InjuryInfo));
            HttpContent content = new StringContent(jss.Serialize(InjuryInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Injuryid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Injuryid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Injury/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateInjury ViewModel = new UpdateInjury();

            string url = "injurydata/findinjury/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                //Put data into Injury data transfer object
                InjuryDto SelectedInjury = response.Content.ReadAsAsync<InjuryDto>().Result;
                ViewModel.injury = SelectedInjury;

                //find players that are injuried by this injury
                url = "injurydata/getplayersforinjury/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Injury data transfer object
                IEnumerable<PlayerDto> SelectedPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
                ViewModel.injuriedplayers = SelectedPlayers;

                //find players that are not injuried by this injury
                url = "injurydata/getplayersnotinjuried/" + id;
                response = client.GetAsync(url).Result;

                //put data into data transfer object
                IEnumerable<PlayerDto> UninjuriedPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
                ViewModel.allplayers = UninjuriedPlayers;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }
        }

        // GET: Injuried/Uninjury/playerid/injuryid
        [HttpGet]
        [Route("Injury/Uninjury/{playerid}/{injuryid}")]

        public ActionResult Uninjury(int playerid, int injuryid)
        {
            string url = "injurydata/uninjury/" + playerid + "/" + injuryid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = injuryid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: injury/injury
        // First Injury is the noun (the injury themselves)
        // second Injury is the verb (the act of injuring)
        // The Injury(1) Injuries(2) a player
        [HttpPost]
        [Route("Injury/injury/{playerid}/{injuryid}")]
        public ActionResult Injury(int playerid, int injuryid)
        {
            string url = "injurydata/injury/" + playerid + "/" + injuryid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = injuryid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Injury/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Injury InjuryInfo)
        {
            Debug.WriteLine(InjuryInfo.InjuryName);
            string url = "injurydata/updateinjury/" + id;
            Debug.WriteLine(jss.Serialize(InjuryInfo));
            HttpContent content = new StringContent(jss.Serialize(InjuryInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("update Injury request succeeded");
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                Debug.WriteLine("update Injury request failed");
                Debug.WriteLine(response.StatusCode.ToString());
                return RedirectToAction("Error");
            }
        }

        // GET: Injury/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "injurydata/findinjury/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                //Put data into Injury data transfer object
                InjuryDto SelectedInjury = response.Content.ReadAsAsync<InjuryDto>().Result;
                return View(SelectedInjury);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Injury/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "injurydata/deleteinjury/" + id;
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
