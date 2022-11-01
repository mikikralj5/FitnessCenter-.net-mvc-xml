using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.HelperClasses;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TrenerController : Controller
    {
        public ActionResult AddGroupTraining()
        {
            return View();
        }

        public ActionResult TrainerGroupTrainings()
        {
            ViewBag.trainings = GroupTrainingData.GetFutureTrainingsForUser((User)Session["LOGGEDIN"]);
            return View();
        }

        public ActionResult TrainerPastTrainings()
        {
            HttpContext.Application["trainingsToShow"] = GroupTrainingData.GetPastTrainingForUser((User)Session["LOGGEDIN"]);
            ViewBag.trainings = GroupTrainingData.GetPastTrainingForUser((User)Session["LOGGEDIN"]);
            return View(); 
        }

        public ActionResult ModifyGroupTraining(string trainingName)
        {
            ViewBag.training = ((User)Session["LOGGEDIN"]).TrenerGroupTrainings.Find(i => i.Name == trainingName);
            return View();
        }

        public ActionResult ParticipantList(string trainingName)
        {
            ViewBag.participants = GroupTrainingData.GetParticipantsForTraining(trainingName, (List<User>)HttpContext.Application["users"]);
            return View();
        }

        [HttpPost]
        public ActionResult AddNewGroupTraining(FormCollection fc)
        {
            if (FormValidator.IsEmpty(fc))
            {
                ViewBag.Error = "Populate all fields";
                return View("AddGroupTraining");
            }

            if (DateTime.Parse(fc.Get("trDate")) < DateTime.Now.AddDays(2))
            {
                ViewBag.Error = "You need to set the date at least 3 days into the future";
                return View("AddGroupTraining");
            }

            GroupTraining temp = GroupTrainingData.GetByName(fc.Get("name"), (List<User>)HttpContext.Application["users"]);
            if(temp.Name != null)
            {

                ViewBag.Error = "Training with this name already exists";
                return View("AddGroupTraining");
            }
            

            GroupTraining newGroupTraining = new GroupTraining();
            newGroupTraining.Name = fc.Get("name");
            newGroupTraining.TrainingType = fc.Get("trType");
            string[] values = fc.Get("trDate").Split('-');
            string[] dateTime = values[2].Split('T');
            newGroupTraining.TrainingDuration = Int32.Parse(fc.Get("trDuration"));
            newGroupTraining.TrainingDateTime = $"{dateTime[0]}/{values[1]}/{values[0]} {dateTime[1]}";
            newGroupTraining.MaxParticipants = Int32.Parse(fc.Get("maxParticipants"));
            User us = ((User)Session["LOGGEDIN"]);
            newGroupTraining.FCenterId = us.TrenerCenter.Name;

            ((User)Session["LOGGEDIN"]).TrenerGroupTrainings.Add(newGroupTraining);

            WriteXML.UsersWrite((List<User>)HttpContext.Application["users"]);

            return RedirectToAction("TrainerGroupTrainings", "Trener");
        }

        [HttpPost]
        public ActionResult ModifyTrainingAction(FormCollection fc)
        {
            if (FormValidator.IsEmpty(fc))
            {
                ViewBag.Error = "Populate all fields";
                ViewBag.training = ((User)Session["LOGGEDIN"]).TrenerGroupTrainings.Find(i => i.Name == fc.Get("name"));
                return View("ModifyGroupTraining");
            }

            if (DateTime.Parse(fc.Get("trDate")) < DateTime.Now.AddDays(2))
            {
                ViewBag.Error = "You need to set the date at least 3 days into the future";
                ViewBag.training = ((User)Session["LOGGEDIN"]).TrenerGroupTrainings.Find(i => i.Name == fc.Get("name"));
                return View("ModifyGroupTraining");
            }


            GroupTraining trainingTemp = ((User)Session["LOGGEDIN"]).TrenerGroupTrainings.Find(i => i.Name == fc.Get("name"));

            trainingTemp.Name = fc.Get("name");
            trainingTemp.TrainingType = fc.Get("trType");
            string[] values = fc.Get("trDate").Split('-');
            string[] dateTime = values[2].Split('T');
            trainingTemp.TrainingDuration = Int32.Parse(fc.Get("trDuration"));
            trainingTemp.TrainingDateTime = $"{dateTime[0]}/{values[1]}/{values[0]} {dateTime[1]}";
            trainingTemp.MaxParticipants = Int32.Parse(fc.Get("maxParticipants"));
        
            WriteXML.UsersWrite((List<User>)HttpContext.Application["users"]);

            return RedirectToAction("TrainerGroupTrainings", "Trener");
        }


        [HttpPost]
        public ActionResult DeleteTraining(string trainingName)
        {
           if(((User)Session["LOGGEDIN"]).TrenerGroupTrainings.Find(i => i.Name == trainingName).Participants.Count > 0)
           {
                ViewBag.trainings = GroupTrainingData.GetFutureTrainingsForUser((User)Session["LOGGEDIN"]);
                ViewBag.Error = "You can't delete this training, because there are already people that have signed up for it";
                return View("TrainerGroupTrainings");
            }

            ((User)Session["LOGGEDIN"]).TrenerGroupTrainings.Find(i => i.Name == trainingName).IsDeleted = true;

            List<User> users = (List<User>)HttpContext.Application["users"];
            WriteXML.UsersWrite(users);

            return RedirectToAction("TrainerGroupTrainings", "Trener");
        }

        [HttpPost]
        public ActionResult SearchTrainings(FormCollection fc)
        {
            List<GroupTraining> searched = new List<GroupTraining>();
            bool matched;

            foreach(var training in GroupTrainingData.GetPastTrainingForUser((User)Session["LOGGEDIN"]))
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

                if (fc.Get("dateSearchMin").Trim() != String.Empty)
                {
                    DateTime trainingTime = DateTime.ParseExact(training.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                    if(trainingTime < DateTime.Parse(fc.Get("dateSearchMin")))
                        matched = false;
                }


                if (fc.Get("dateSearchMax").Trim() != String.Empty)
                {
                    DateTime trainingTime = DateTime.ParseExact(training.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                    if (trainingTime > DateTime.Parse(fc.Get("dateSearchMax")))
                        matched = false;
                }

                if (matched)
                    searched.Add(training);
            }

            HttpContext.Application["trainingsToShow"] = searched;
            ViewBag.trainings = (List<GroupTraining>)HttpContext.Application["trainingsToShow"];
            return View("TrainerPastTrainings");
        }


        [HttpPost]
        public ActionResult NameSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["trainingsToShow"])
                .OrderBy(i => i.Name).ToList();
            }
            else
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["trainingsToShow"])
               .OrderByDescending(i => i.Name).ToList();
            }

            
            return View("TrainerPastTrainings");
        }

        [HttpPost]
        public ActionResult DateSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["trainingsToShow"])
                .OrderBy(i => i.TrainingDateTime).ToList();
            }
            else
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["trainingsToShow"])
               .OrderByDescending(i => i.TrainingDateTime).ToList();
            }

            return View("TrainerPastTrainings");
        }


        [HttpPost]
        public ActionResult TypeSort(string param)
        {
            if (param == "ASCENDING")
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["trainingsToShow"])
                .OrderBy(i => i.TrainingType).ToList();
            }
            else
            {
                ViewBag.trainings = ((List<GroupTraining>)HttpContext.Application["trainingsToShow"])
               .OrderByDescending(i => i.TrainingType).ToList();
            }

            return View("TrainerPastTrainings");
        }
    }
}