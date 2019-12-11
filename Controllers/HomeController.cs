using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        // *********************
        // HTTPGets
        // *********************

        // Login/Register Page
        [HttpGet("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpGet("home")]
        public IActionResult ActivityBoard()
        {
            int? LocalVar = HttpContext.Session.GetInt32("UserId");
            if (LocalVar == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.user = dbContext.Users.FirstOrDefault(i => i.UserId == (int)LocalVar);

            // List<WeddingPlan> allPlans = dbContext.WeddingPlans
            //     .Include(c => c.Creator)
            //     .Include(g => g.Attendees)
            //     .ThenInclude(u => u.User)
            //     .ToList();
            DateTime today = DateTime.Now;
            List<DojoActivity> allActivities = dbContext.DojoActivities
                .Include(u => u.Creator)
                .Include(e => e.Attendees)
                .ThenInclude(u => u.User)
                .OrderBy(d => d.ActStart)
                .Where(d => d.ActStart > today)
                .ToList();

            ViewBag.allActivities = allActivities;

            return View();
        }

        // Add a new Activity
        [HttpGet("newActivity")]
        public IActionResult NewActivity()
        {
            int? LocalVar = HttpContext.Session.GetInt32("UserId");
            if (LocalVar == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        // Join an activity that looks fun
        [HttpGet("join/{userId}/{actId}")]
        public IActionResult JoinActivity(int userId, int actId)
        {
            Association joinAct = new Association();
            joinAct.UserId = userId;
            joinAct.ActivityId = actId;
            dbContext.Associations.Add(joinAct);
            dbContext.SaveChanges();
            return RedirectToAction("ActivityBoard");
        }
        
        // leave an activity that you're a part of
        [HttpGet("leave/{userId}/{actId}")]
        public IActionResult LeaveActivity(int userId, int actId)
        {
            Association leaveAct = dbContext.Associations.FirstOrDefault(u => u.ActivityId == actId && u.UserId == userId);
            dbContext.Associations.Remove(leaveAct);
            dbContext.SaveChanges();
            return RedirectToAction("ActivityBoard");

        }

        // delete an activity that you've created
        [HttpGet("delete/{actId}")]
        public IActionResult DeleteActivity(int actId)
        {
            DojoActivity act = dbContext.DojoActivities.FirstOrDefault(a => a.ActivityId == actId);
            dbContext.DojoActivities.Remove(act);
            dbContext.SaveChanges();
            return RedirectToAction("ActivityBoard");
        }

        [HttpGet("activity/{actId}")]
        public IActionResult ActivityDetails(int actId)
        {
            int? LocalVar = HttpContext.Session.GetInt32("UserId");
            if (LocalVar == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.user = dbContext.Users.FirstOrDefault(i => i.UserId == (int)LocalVar);

            ViewBag.activity = dbContext.DojoActivities
                .Include(c => c.Creator)
                .Include(g => g.Attendees)
                .ThenInclude(u => u.User)
                .FirstOrDefault(i => i.ActivityId == actId);

            return View();
        }




        // Logout Clears session and retuns user to login/register page
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // *********************
        // HTTPPosts
        // *********************

        // Checks to see if email is unique and adds user into database
        [HttpPost("CheckReg")]
        public IActionResult CheckReg(User user)
        {
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");                    
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToAction("ActivityBoard");
            }
            return View("Index");
        }

        // Checks the Login credentials vs the database
        [HttpPost("CheckLog")]
        public IActionResult CheckLogin(LogUser user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LogEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.LogPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("ActivityBoard");
            }
            return View("Index");
        }

        [HttpPost("createActivity")]
        public IActionResult CreateActivity(DojoActivity act)
        {
            int? LocalVar = HttpContext.Session.GetInt32("UserId");
            int creator = (int)LocalVar;

            if(ModelState.IsValid)
            {
                Console.WriteLine("****************************");
                Console.WriteLine($"ActStart {act.ActStart}");
                Console.WriteLine($"DurationNum {act.DurationNum}");
                Console.WriteLine($"DurationLen {act.DurationLen}");
                Console.WriteLine($"TimeStart {act.TimeStart}");
                // TimeSpan timediff = new TimeSpan();
                DateTime date = DateTime.Parse(act.TimeStart, System.Globalization.CultureInfo.CurrentCulture);
                Console.WriteLine($"Created Date {date}");
                act.ActStart = new DateTime(act.ActStart.Year, act.ActStart.Month, act.ActStart.Day,
                                            date.Hour, date.Minute, date.Second);
                Console.WriteLine($"possible new actstart datetime? {act.ActStart}");
                if(act.DurationLen == "Days")
                {
                    Console.WriteLine("IN DAYS EVAL");
                    act.ActEnd = act.ActStart.AddDays(act.DurationNum);

                } else if (act.DurationLen == "Hours")
                {
                    Console.WriteLine("IN HOURS EVAL");
                    act.ActEnd = act.ActStart.AddHours(act.DurationNum);

                } else if (act.DurationLen == "Minutes")
                {
                    Console.WriteLine("IN MINUTES EVAL");
                    act.ActEnd = act.ActStart.AddMinutes(act.DurationNum);
                }
                Console.WriteLine($"ActStart {act.ActStart}");
                Console.WriteLine($"ActEnd {act.ActEnd}");

                act.UserId = creator;

                dbContext.DojoActivities.Add(act);
                Association firstPerson = new Association();
                firstPerson.UserId = creator;
                firstPerson.ActivityId = act.ActivityId;
                dbContext.Associations.Add(firstPerson);
                dbContext.SaveChanges();
                

                return RedirectToAction("ActivityBoard");
            }
            return View("NewActivity");
        }
    }
}
