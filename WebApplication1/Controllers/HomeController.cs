using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.HelperClasses;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.user = (User)Session["LOGGEDIN"];
            HttpContext.Application["centersToShow"] = FitnessCenterData.GetAll(((List<User>)HttpContext.Application["users"])).OrderBy(i => i.Name).ToList();
            ViewBag.centers = FitnessCenterData.GetAll(((List<User>)HttpContext.Application["users"])).OrderBy(i => i.Name).ToList();
            
            return View();
        }

        public ActionResult CenterDetails(string centerName)
        {
            ViewBag.center = FitnessCenterData.FindByName(centerName, (List<User>)HttpContext.Application["users"]);
            ViewBag.trainings = GroupTrainingData.GetAllFutureTraining((List<User>)HttpContext.Application["users"])
                .Where(i => i.FCenterId == centerName).ToList();
            List<GroupTraining> grp = GroupTrainingData.GetAllFutureTraining((List<User>)HttpContext.Application["users"]);
            ViewBag.user = (User)Session["LOGGEDIN"];
            ViewBag.comments = CommentData.GetAllAcceptedCommentsForFc(centerName, (List<User>)HttpContext.Application["users"]);

            return View();
        }

        [HttpPost]
        public ActionResult SearchCenters(FormCollection fc)
        {
            List<FitnessCenter> searched = new List<FitnessCenter>();
            bool matched;

            foreach (var center in FitnessCenterData.GetAll((List<User>)HttpContext.Application["users"]))
            {
                matched = true;

                string name = fc.Get("nameSearch");
                if (fc.Get("nameSearch").Trim() != String.Empty && center.Name != name)
                {
                    if(center.Name.Trim() != name.Trim())
                    {
                        matched = false;
                    }
                   
                }

                if (fc.Get("adrsSearch").Trim() != String.Empty)
                {
                    string[] values = center.Adress.Split(' ');
                    string addr = values[0] + " " + values[1];

                    if(addr != fc.Get("adrsSearch"))
                        matched = false;
                }

                if (fc.Get("yearSearchMin").Trim() != String.Empty)
                {
                    int min = Int32.Parse(fc.Get("yearSearchMin"));
                    int founded = Int32.Parse(center.Year);

                    if (founded < min)
                        matched = false;
                }


                if (fc.Get("yearSearchMax").Trim() != String.Empty)
                {
                    int max = Int32.Parse(fc.Get("yearSearchMax"));
                    int founded = Int32.Parse(center.Year);

                    if (founded > max)
                        matched = false;
                }

                if (matched)
                    searched.Add(center);
            }

            HttpContext.Application["centersToShow"] = searched.OrderBy(i => i.Name).ToList();
            ViewBag.centers = (List<FitnessCenter>)HttpContext.Application["centersToShow"];
            ViewBag.user = (User)Session["LOGGEDIN"];
            return View("Index");
        }

        [HttpPost]
        public ActionResult NameSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.centers = ((List<FitnessCenter>)HttpContext.Application["centersToShow"])
                .OrderBy(i => i.Name).ToList();
            }
            else
            {
                ViewBag.centers = ((List<FitnessCenter>)HttpContext.Application["centersToShow"])
               .OrderByDescending(i => i.Name).ToList();
            }

            ViewBag.user = (User)Session["LOGGEDIN"];
            return View("Index");
        }

        [HttpPost]
        public ActionResult AdressSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.centers = ((List<FitnessCenter>)HttpContext.Application["centersToShow"])
                .OrderBy(i => i.Adress).ToList();
            }
            else
            {
                ViewBag.centers = ((List<FitnessCenter>)HttpContext.Application["centersToShow"])
               .OrderByDescending(i => i.Adress).ToList();
            }

            ViewBag.user = (User)Session["LOGGEDIN"];
            return View("Index");
        }


        [HttpPost]
        public ActionResult YearSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.centers = ((List<FitnessCenter>)HttpContext.Application["centersToShow"])
                .OrderBy(i => i.Year).ToList();
            }
            else
            {
                ViewBag.centers = ((List<FitnessCenter>)HttpContext.Application["centersToShow"])
               .OrderByDescending(i => i.Year).ToList();
            }

            ViewBag.user = (User)Session["LOGGEDIN"];
            return View("Index");
        }

    }
}