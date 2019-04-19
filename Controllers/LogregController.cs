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
using System.Text.RegularExpressions;

namespace belt.Controllers
{
    public class LogregController : Controller
    {
        private DojoContext dbContext;

        public LogregController(DojoContext context)
        {
            dbContext = context;
        }


        [Route("login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        public IActionResult LoginProcess(LoginForm sub)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();

                User userQueried = dbContext.Users.FirstOrDefault(user => user.Email == sub.Email1);
                if(userQueried == null)
                {
                    ModelState.AddModelError("Email1", "Email and password don't match or exist.");
                    return View("Index");
                }
                PasswordVerificationResult matchPw = Hasher.VerifyHashedPassword(userQueried, userQueried.PwHash, sub.Password1);
                System.Console.WriteLine(matchPw);

                if(matchPw.ToString() == "Success")
                {
                    HttpContext.Session.SetInt32("userID", userQueried.Id);
                    return RedirectToAction("Wall", controllerName: "Home");
                }
                else
                {
                    ModelState.AddModelError("Email1", "Email and password don't match or exist.");
                    return View("Index");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [Route("signup")]
        [HttpPost]
        public IActionResult RegProcess(RegisterForm sub)
        {
            if(dbContext.Users.FirstOrDefault(user => user.Email == sub.Email) != null)
            {
                ModelState.AddModelError("Email", "This email is already in use!");
            }
            if(sub.Password != sub.ConfirmPw)
            {
                ModelState.AddModelError("Password", "Passwords must match.");
            }
            if(sub.Password != null)
            {
                Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)");
                Match match = regex.Match(sub.Password);
                if(!match.Success)
                {
                    ModelState.AddModelError("Password", "Password must contain an uppercase, lowercase, numeric, and special character!");
                }
            }

            if(ModelState.IsValid)
            {
                User newUser = new User();
                newUser.Name = sub.Name;
                newUser.Email = sub.Email;

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.PwHash = Hasher.HashPassword(newUser, sub.Password);
                
                System.Console.WriteLine("Id before db submit "+newUser.Id);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("userID", newUser.Id);
                System.Console.WriteLine("Id after db submit "+newUser.Id);

                return RedirectToAction("Wall", controllerName: "Home");
            }
            else
            {
                return View("Index");
            }
        }     


        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", controllerName: "Home");
        }

    }
}
