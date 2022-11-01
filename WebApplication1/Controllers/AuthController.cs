using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.HelperClasses;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            if ((User)Session["LOGGEDIN"] != null)
            {
                ViewBag.user = (User)Session["LOGGEDIN"];
            }
            if ((User)Session["LOGGEDIN"] != null && ((User)Session["LOGGEDIN"]).UserRole == UserRole.VLASNIK)
            {
                ViewBag.centers = ((User)Session["LOGGEDIN"]).ValsnikCenters.Where(i => i.IsDelted == false).ToList();
            }
            return View();
        }

        public ActionResult ModifyUser()
        {
            ViewBag.user = ((User)Session["LOGGEDIN"]);
            return View();
        }


        [HttpPost]
        public ActionResult LoginUser(FormCollection fc)
        {

            List<User> users = (List<User>)HttpContext.Application["users"];
            User user = users.Find(ss => ss.Username.Equals(fc.Get("username")) && ss.IsBlocked == false && ss.Password.Equals(fc.Get("password")));
            if (user != null)
            {
                Session["LOGGEDIN"] = user;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "User does not exist";
            return View("Login");

        }

        [HttpPost]
        public ActionResult RegisterUser(FormCollection fc, UserRole role)
        {

            List<User> users = (List<User>)HttpContext.Application["users"];


            if (FormValidator.IsEmpty(fc))
            {
                ViewBag.Error = "Populate all fields";
                return View("Register");
            }

            User u = users.Find(i => i.Username == fc.Get("username"));
            if (u != null)
            {
                ViewBag.Error = "User already exists";
                if ((User)Session["LOGGEDIN"] != null)
                {
                    ViewBag.user = (User)Session["LOGGEDIN"];
                }
                if ((User)Session["LOGGEDIN"] != null && ((User)Session["LOGGEDIN"]).UserRole == UserRole.VLASNIK)
                {
                    ViewBag.centers = ((User)Session["LOGGEDIN"]).ValsnikCenters.Where(i => i.IsDelted == false).ToList();
                }
                return View("Register");
            }

            User newUser = new User();
            newUser.Username = fc.Get("username");
            newUser.Password = fc.Get("password");
            newUser.Name = fc.Get("name");
            newUser.LastName = fc.Get("lastName");
            newUser.Email = fc.Get("email");
            newUser.Gender = fc.Get("gender");
            string[] values = fc.Get("dateOfBirth").Split('-');
            newUser.DateOfBirth = values[2] + "/" + values[1] + "/" + values[0];
            newUser.UserRole = role;
            if (((User)Session["LOGGEDIN"]) != null && ((User)Session["LOGGEDIN"]).UserRole == UserRole.VLASNIK)
            {
                newUser.TrenerCenter = ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("center"));
            }

            users.Add(newUser);
            WriteXML.UsersWrite(users);
            HttpContext.Application["users"] = users;

            if (((User)Session["LOGGEDIN"]) != null && ((User)Session["LOGGEDIN"]).UserRole == UserRole.VLASNIK)
            {
                return RedirectToAction("TrainersTable", "Vlasnik");
            }

            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public ActionResult ModifyUserAction(FormCollection fc)
        {

            if (FormValidator.IsEmpty(fc))
            {
                ViewBag.Error = "Populate all fields";
                ViewBag.user = ((User)Session["LOGGEDIN"]);
                return View("ModifyUser");
            }

            ((User)Session["LOGGEDIN"]).Password = fc.Get("password");
            ((User)Session["LOGGEDIN"]).Name = fc.Get("name");
            ((User)Session["LOGGEDIN"]).LastName = fc.Get("lastName");
            ((User)Session["LOGGEDIN"]).Email = fc.Get("email");
            ((User)Session["LOGGEDIN"]).Gender = fc.Get("gender");
            string[] values = fc.Get("dateOfBirth").Split('-');
            ((User)Session["LOGGEDIN"]).DateOfBirth = values[2] + "/" + values[1] + "/" + values[0];

            List<User> users = (List<User>)HttpContext.Application["users"];
            WriteXML.UsersWrite(users);

            return RedirectToAction("ModifyUser", "Auth");
        }

        public ActionResult Logout()
        {
            Session["LOGGEDIN"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}