using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HobbyHut.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HobbyHut.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            { 
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View("index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword( newUser, newUser.Password);
                
                _context.Users.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                int id = newUser.UserId;

                return Redirect("/dashboard");

            }
            return View("index");
        }
        
        [HttpPost("/login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("index");
                }
                else{
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                    if(result ==0)
                    {
                        ModelState.AddModelError("Email", "Invalid Email/Password");
                        return View("index");
                    }
                    else{
                        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                        return Redirect("/dashboard");
                    }
                }
            }
            return View("index");
        }

        [HttpGet("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            List<Hobby> allHobbies = _context.Hobbies.Include( h => h.HobbyUser).ThenInclude( a => a.UserA).ToList();

            return View("dashboard", allHobbies);
        }

        [HttpGet("/new")]
        public IActionResult NewHobby()
        {
            int? CurUserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.CurUserId = CurUserId;
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine(CurUserId);
            return View("newhobby");
        }

        [HttpPost("/create")]
        public IActionResult CreateHobby(Hobby newHobby)
        {
            if(ModelState.IsValid)
            {
                 if(_context.Hobbies.Any(u => u.HobbyName == newHobby.HobbyName))
                {
                    ModelState.AddModelError("HobbyName", "Hobby already exsists.");
                    return View("newhobby");
                }
                _context.Hobbies.Add(newHobby);
                _context.SaveChanges();
                return Redirect("/dashboard");
            }
            return View("NewHobby");
        }

        [HttpGet("/{HobbyID}")]
        public IActionResult ViewHobby(int HobbyID)
        {
            int? CurUserId = HttpContext.Session.GetInt32("UserId");
            if(CurUserId != null)
            {
                ViewBag.CurUserId = (int)CurUserId;
                Hobby getOne = _context.Hobbies.FirstOrDefault( h => h.HobbyId == HobbyID);
                // List<Association> Users = _context.Associations.Include( a => a.UserA).ThenInclude( )
                ViewBag.Users = _context.Associations.Where( a => a.HobbyA.HobbyId == HobbyID).Include( r => r.UserA).ToList();
                return View("ViewHobby", getOne);
            }
            return Redirect("/");
        }

        [HttpGet("/join/{HID}")]
        public IActionResult JoinHobby( int HID)
        {
            int? CurUserId = HttpContext.Session.GetInt32("UserId");
        //     Hobby getOne = _context.Hobbies.FirstOrDefault( h => h.HobbyId == HID);
        //     List<Association> hobbyCheck = _context.Associations.Where( a => a.HobbyA.HobbyId == HID).Include( h => h.UserA).ToList();
            if( CurUserId == null)
            {
                return Redirect("/");
            }
            else
            {
   

                Association association = new Association();
                association.UserId = (int)CurUserId;
                association.HobbyId = HID;
                _context.Associations.Add(association);
                _context.SaveChanges();
                return Redirect("/dashboard");
                }
            // }
        }

        [HttpGet("/edit/{HID}")]
        public IActionResult ViewHobbyEdit(int HID)
        {
            Hobby hobby = _context.Hobbies.FirstOrDefault( h => h.HobbyId == HID);
            return View("edit", hobby);
        }

        [HttpGet("/update/{ID}")]
        public  IActionResult EditHobby( Hobby hobby, int ID)

        {
            Hobby newHobby = new Hobby();
            newHobby = hobby;
            Console.WriteLine("__________________________________");
            Console.WriteLine(newHobby.HobbyName);
            Hobby editHobby = _context.Hobbies.FirstOrDefault( h => h.HobbyId == ID);
            // Hobby edit = new Hobby();
            // edit = editHobby;
            if(ModelState.IsValid)
            {
                editHobby.HobbyName = newHobby.HobbyName;
                editHobby.Description = newHobby.Description;
                editHobby.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return Redirect($"/{ID}");
            }
            return Redirect($"/{ID}");

        }
   }
}
