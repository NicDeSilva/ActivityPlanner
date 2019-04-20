using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using belt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace belt.Controllers
{
    public class HomeController : Controller
    {
        private DojoContext dbContext;

        public HomeController(DojoContext context)
        {
            dbContext = context;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        
        [Route("wall")]
        [HttpGet]
        public IActionResult Wall()
        {
            if(HttpContext.Session.Keys.Contains("userID"))
            {
                Dashboard dashboardInfo = new Dashboard();
                dashboardInfo.Activities = dbContext.Activities.Where(a => a.Date > DateTime.Now).Include(a => a.Creator).Include(a => a.Guests).OrderBy(a => a.Date).ToList();
                dashboardInfo.User = dbContext.Users.FirstOrDefault(user => user.Id == HttpContext.Session.GetInt32("userID"));


                return View(dashboardInfo);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Route("/new")]
        [HttpGet]
        public IActionResult New()
        {
            if(HttpContext.Session.Keys.Contains("userID"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Route("/new")]
        [HttpPost]
        public IActionResult SubmitActivity(Activ sub)
        {
            User loggedIn = dbContext.Users.Include(user => user.plans).ThenInclude(plan => plan.Activity).Where(user => user.Id == HttpContext.Session.GetInt32("userID")).FirstOrDefault();
                        DateTime startOrig;
                        DateTime endOrig;

                        DateTime startNew;
                        DateTime endNew;
                if(sub.TimeUnit == "hours")
                {
                    sub.Duration = sub.Duration /24;
                }
                else if(sub.TimeUnit == "minutes")
                {
                    sub.Duration = sub.Duration /(60*24);
                }
                    foreach (var plan in loggedIn.plans)
                    {
                        startOrig = plan.Activity.Date;
                        endOrig = plan.Activity.Date + plan.Activity.Duration;

                        startNew = sub.Date.Date + sub.Time;
                        endNew = sub.Date.Date + sub.Time + sub.Duration;
                        System.Console.WriteLine($"**** Is new ending {endNew} before the old start {startOrig} or is the new start {startNew} after the old end {endOrig}???");
                        System.Console.WriteLine("endNew.CompareTo(startOrig) < 0 "+endNew.CompareTo(startOrig));
                        System.Console.WriteLine("startNew.CompareTo(endOrig) > 0 "+startNew.CompareTo(endOrig));
                        if((endNew.CompareTo(startOrig) < 0) || (startNew.CompareTo(endOrig) > 0))
                        {
                            System.Console.WriteLine("pass");
                        }
                        else{ModelState.AddModelError("Date", "Schedule conflict with your events!");}
                    }
            if(sub.Date+sub.Time <= DateTime.Now)
            {
                ModelState.AddModelError("Date", "Date must be in the future!");
            }

            if(ModelState.IsValid)
            {
                dbContext.Add(sub);
                dbContext.SaveChanges();

                Plan newPlan = new Plan();
                newPlan.User = loggedIn;
                newPlan.Activity = sub;
                newPlan.Activity.Date = newPlan.Activity.Date.Date + sub.Time;
                newPlan.Activity.Creator = loggedIn;
                newPlan.Activity.Guests = new List<Plan>();
                System.Console.WriteLine(newPlan.Activity.Guests);

                dbContext.Add(newPlan);
                newPlan.Activity.Guests.Add(newPlan);
                dbContext.SaveChanges();

                return RedirectToAction("Wall");
            }

            return View("New");
        }

        [Route("/delete/{id}")]
        [HttpPost]
        public IActionResult Delete()
        {
            Int32.TryParse(RouteData.Values["id"].ToString(), out int actId);

            if(HttpContext.Session.Keys.Contains("userID"))
            {
                List<Plan> relatedPlans = dbContext.Plans.Include(p => p.Activity).Where(p => p.Activity.Id == actId).ToList();
                Activ specActivity = relatedPlans[0].Activity;

                foreach (var specPlan in relatedPlans)
                {
                    dbContext.Remove(specPlan);
                }

                dbContext.Remove(specActivity);
                dbContext.SaveChanges();

                return RedirectToAction("Wall");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Route("/rsvp/{id}")]
        [HttpPost]
        public IActionResult RSVP()
        {
            Int32.TryParse(RouteData.Values["id"].ToString(), out int actId);

            if(HttpContext.Session.Keys.Contains("userID"))
            {
                
                Activ specActivity = dbContext.Activities.Include(a => a.Guests).FirstOrDefault(a => a.Id == actId);
                User loggedIn = dbContext.Users.Include(user => user.plans).ThenInclude(plan => plan.Activity).Where(user => user.Id == HttpContext.Session.GetInt32("userID")).FirstOrDefault();


                        DateTime startOrig;
                        DateTime endOrig;

                        DateTime startNew;
                        DateTime endNew;

                    foreach (var plan in loggedIn.plans)
                    {
                        startOrig = plan.Activity.Date;
                        endOrig = plan.Activity.Date + plan.Activity.Duration;

                        startNew = specActivity.Date;
                        endNew = specActivity.Date + specActivity.Duration;
                        System.Console.WriteLine($"**** Is new ending {endNew} before the old start {startOrig} or is the new start {startNew} after the old end {endOrig}???");
                        System.Console.WriteLine("endNew.CompareTo(startOrig) < 0 "+endNew.CompareTo(startOrig));
                        System.Console.WriteLine("startNew.CompareTo(endOrig) > 0 "+startNew.CompareTo(endOrig));
                        if((endNew.CompareTo(startOrig) < 0) || (startNew.CompareTo(endOrig) > 0))
                        {
                            System.Console.WriteLine("pass");
                        }
                        else{
                            System.Console.WriteLine($"conflict");
                            TempData["conflict"] = true;
                            return RedirectToAction("Wall");
                            }
                    }

                Plan newPlan = new Plan();
                newPlan.User = loggedIn;
                newPlan.Activity = specActivity;

                dbContext.Add(newPlan);
                specActivity.Guests.Add(newPlan);
                dbContext.SaveChanges();

                return RedirectToAction("Wall");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        

        [Route("/unrsvp/{id}")]
        [HttpPost]
        public IActionResult UNRSVP()
        {
            Int32.TryParse(RouteData.Values["id"].ToString(), out int actId);

            if(HttpContext.Session.Keys.Contains("userID"))
            {
                
                Activ specActivity = dbContext.Activities.Include(a => a.Guests).ThenInclude(guest => guest.User).FirstOrDefault(a => a.Id == actId);
                User loggedIn = dbContext.Users.FirstOrDefault(user => user.Id == HttpContext.Session.GetInt32("userID"));

                Plan specPlan = specActivity.Guests.FirstOrDefault(guest => guest.User == loggedIn);


                dbContext.Plans.Remove(specPlan);
                dbContext.SaveChanges();

                return RedirectToAction("Wall");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Route("/activity/{id}")]
        [HttpGet]
        public IActionResult Info()
        {
            Int32.TryParse(RouteData.Values["id"].ToString(), out int actId);

            if(HttpContext.Session.Keys.Contains("userID"))
            {
                Dashboard dashboardInfo = new Dashboard();
                dashboardInfo.Activities = new List<Activ>();
                dashboardInfo.Activities.Add(dbContext.Activities.Include(a => a.Guests).ThenInclude(guest => guest.User).FirstOrDefault(a => a.Id == actId));
                dashboardInfo.User = dbContext.Users.FirstOrDefault(user => user.Id == HttpContext.Session.GetInt32("userID"));

                return View("Info", dashboardInfo);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }        

    }
}

