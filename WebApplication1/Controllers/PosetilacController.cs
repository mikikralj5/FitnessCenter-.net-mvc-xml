using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.HelperClasses;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PosetilacController : Controller
    {

        public ActionResult PosetilacPastTrainings()
        {
            HttpContext.Application["posetilacTrainingsToShow"] = GroupTrainingData.GetPastTrainingForUser((User)Session["LOGGEDIN"]);
            ViewBag.trainings = GroupTrainingData.GetPastTrainingForUser((User)Session["LOGGEDIN"]);
            return View();
        }

        public ActionResult AddComment(string centerName)
        {
            FitnessCenter fitnessCenter = FitnessCenterData.FindByName(centerName, (List<User>)HttpContext.Application["users"]);
            GroupTraining temp = ((User)Session["LOGGEDIN"]).PosetiocGroupTrainings.Find(i => i.FCenterId == centerName);
            if(temp == null)
            {
                ViewBag.canAddComment = false;
            }
            else
            {
                ViewBag.canAddComment = true;
            }

            ViewBag.centerName = centerName;         
            return View();
        }

        [HttpPost]
        public ActionResult SignUpForTraining(string trainingName)
        {
            GroupTraining training = GroupTrainingData.GetByName(trainingName, (List<User>)HttpContext.Application["users"]);
            ((User)Session["LOGGEDIN"]).PosetiocGroupTrainings.Add(training);
            training.Participants.Add(((User)Session["LOGGEDIN"]).Username);
            WriteXML.UsersWrite((List<User>)HttpContext.Application["users"]);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AddCommentAction(FormCollection fc)
        {
            if (FormValidator.IsEmpty(fc))
            {
                ViewBag.Error = "Populate all fields";
                return View("AddGroupTraining");
            }

            string centerName = fc.Get("centerName");
            Comment newComment = new Comment();
            newComment.CommentText = fc.Get("cmntText");
            newComment.Rating = Int32.Parse(fc.Get("cmntRating"));
            newComment.CreatorUsername = ((User)Session["LOGGEDIN"]).Username;
            newComment.FCenterId = centerName;
            newComment.CommentState = CommentState.PENDING;
          
            FitnessCenter fitnessCenter = FitnessCenterData.FindByName(centerName, (List<User>)HttpContext.Application["users"]);
            newComment.Id = fitnessCenter.Comments.Count + 1;
            fitnessCenter.Comments.Add(newComment);

            WriteXML.UsersWrite((List<User>)HttpContext.Application["users"]);

         
            ViewBag.center = FitnessCenterData.FindByName(centerName, (List<User>)HttpContext.Application["users"]);
            ViewBag.trainings = GroupTrainingData.GetAllFutureTraining((List<User>)HttpContext.Application["users"])
                .Where(i => i.FCenterId == centerName).ToList();
            ViewBag.user = (User)Session["LOGGEDIN"];
            ViewBag.comments = CommentData.GetAllAcceptedCommentsForFc(centerName, (List<User>)HttpContext.Application["users"]);

            return View("../Home/CenterDetails");
        }


        [HttpPost]
        public ActionResult SearchTrainings(FormCollection fc)
        {
            List<GroupTraining> searched = new List<GroupTraining>();
            bool matched;

            foreach (var training in GroupTrainingData.GetPastTrainingForUser((User)Session["LOGGEDIN"]))
            {
                matched = true;

                if (fc.Get("nameSearch").Trim() != String.Empty && training.Name != fc.Get("nameSearch"))
                {
                    matched = false;
                }

                if (fc.Get("typeSearch").Trim() != String.Empty && training.TrainingType != fc.Get("typeSearch"))
                {
                    matched = false;
                }

                if (fc.Get("fcSearch").Trim() != String.Empty && training.FCenterId != fc.Get("fcSearch"))
                {
                    matched = false;
                }


                if (matched)
                    searched.Add(training);
            }

            HttpContext.Application["posetilacTrainingsToShow"] = searched;
            ViewBag.trainings = (List<GroupTraining>)HttpContext.Application["posetilacTrainingsToShow"];
            return View("PosetilacPastTrainings");
        }


        [HttpPost]
        public ActionResult NameSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["posetilacTrainingsToShow"])
                .OrderBy(i => i.Name).ToList();
            }
            else
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["posetilacTrainingsToShow"])
               .OrderByDescending(i => i.Name).ToList();
            }


            return View("PosetilacPastTrainings");
        }

        [HttpPost]
        public ActionResult DateSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["posetilacTrainingsToShow"])
                .OrderBy(i => i.TrainingDateTime).ToList();
            }
            else
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["posetilacTrainingsToShow"])
               .OrderByDescending(i => i.TrainingDateTime).ToList();
            }

            return View("PosetilacPastTrainings");
        }


        [HttpPost]
        public ActionResult TypeSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["posetilacTrainingsToShow"])
                .OrderBy(i => i.TrainingType).ToList();
            }
            else
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["posetilacTrainingsToShow"])
               .OrderByDescending(i => i.TrainingType).ToList();
            }

            return View("PosetilacPastTrainings");
        }
    }
}