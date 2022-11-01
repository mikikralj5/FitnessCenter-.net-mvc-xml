using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.HelperClasses;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class VlasnikController : Controller
    {

        public ActionResult VlasnikCenters()
        {
            ViewBag.vlasnikCenters = ((User)Session["LOGGEDIN"]).ValsnikCenters.Where(i => i.IsDelted == false).ToList();
            return View();
        }

        public ActionResult AddFitnessCenter()
        {

            return View();
        }

        public ActionResult ModifyCenter(string centerName)
        {
            ViewBag.center = ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == centerName && i.IsDelted == false);
            return View();
        }

        public ActionResult TrainersTable()
        {

            ViewBag.trainers = ((List<User>)HttpContext.Application["users"]).Where(i => i.UserRole == UserRole.TRENER && i.IsBlocked == false).ToList();
            return View();
        }

        public ActionResult PendingCommentRequests()
        {
            List<Comment> com = CommentData.GetAllPendingCommentsForAdminFc((User)Session["LOGGEDIN"]);
            ViewBag.comments = CommentData.GetAllPendingCommentsForAdminFc((User)Session["LOGGEDIN"]);
            return View();
        }


        [HttpPost]
        public ActionResult AddNewCenter(FormCollection fc)
        {
            if (FormValidator.IsEmpty(fc))
            {
                ViewBag.Error = "Populate all fields";
                return View("AddFitnessCenter");
            }

            FitnessCenter temp = FitnessCenterData.FindByName(fc.Get("name"), (List<User>)HttpContext.Application["users"]);
            if (temp.Name != null)
            {
                ViewBag.Error = "Center with this name already exists";
                return View("AddFitnessCenter");
            }


            FitnessCenter fitnessCenter = new FitnessCenter();
            fitnessCenter.Name = fc.Get("name");
            fitnessCenter.Adress = $"{fc.Get("street")} {fc.Get("snum")} {fc.Get("city")} {fc.Get("pcode")}";
            fitnessCenter.Year = fc.Get("year");
            fitnessCenter.YearlyPrice = Int32.Parse(fc.Get("yprice"));
            fitnessCenter.MonthlyPrice = Int32.Parse(fc.Get("mprice"));
            fitnessCenter.OneTrainingPrice = Int32.Parse(fc.Get("oneprice"));
            fitnessCenter.GroupTrainingPrice = Int32.Parse(fc.Get("gprice"));
            fitnessCenter.AssistantTrainingPrice = Int32.Parse(fc.Get("tprice"));
            fitnessCenter.VlasnikId = ((User)Session["LOGGEDIN"]).Name;
            fitnessCenter.Comments = new List<Comment>();

            List<User> users = (List<User>)HttpContext.Application["users"];
            users.Find(i => i.Username == ((User)Session["LOGGEDIN"]).Username && i.IsBlocked == false).
                ValsnikCenters.Add(fitnessCenter);

            HttpContext.Application["users"] = users;
            WriteXML.UsersWrite(users);

            return RedirectToAction("VlasnikCenters", "Vlasnik");
        }

        [HttpPost]
        public ActionResult ModifyCenterAction(FormCollection fc)
        {

            if (FormValidator.IsEmpty(fc))
            {
                ViewBag.Error = "Populate all fields";
                ViewBag.center = ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name"));
                return View("ModifyCenter");
            }

            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name") && i.IsDelted == false).Adress = $"{fc.Get("street")} {fc.Get("snum")} {fc.Get("city")} {fc.Get("pcode")}";
            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name") && i.IsDelted == false).Year = fc.Get("year");
            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name") && i.IsDelted == false).YearlyPrice = Int32.Parse(fc.Get("yprice"));
            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name") && i.IsDelted == false).MonthlyPrice = Int32.Parse(fc.Get("mprice"));
            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name") && i.IsDelted == false).OneTrainingPrice = Int32.Parse(fc.Get("oneprice"));
            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name") && i.IsDelted == false).GroupTrainingPrice = Int32.Parse(fc.Get("gprice"));
            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == fc.Get("Name") && i.IsDelted == false).AssistantTrainingPrice = Int32.Parse(fc.Get("tprice"));

            List<User> users = (List<User>)HttpContext.Application["users"];

            WriteXML.UsersWrite(users);

            return RedirectToAction("VlasnikCenters", "Vlasnik");
        }

        [HttpPost]
        public ActionResult DeleteCenter(string centerName)
        {
            List<GroupTraining> temp = GroupTrainingData.GetAllFutureTraining((List<User>)HttpContext.Application["users"]);
            foreach(var training in temp)
            {
                if(training.FCenterId == centerName)
                {
                    ViewBag.vlasnikCenters = ((User)Session["LOGGEDIN"]).ValsnikCenters.Where(i => i.IsDelted == false).ToList();
                    ViewBag.Error = "There are training in the future for this fitness center, so you can delete it";
                    return View("VlasnikCenters");
                }
            }


            ((User)Session["LOGGEDIN"]).ValsnikCenters.Find(i => i.Name == centerName).IsDelted = true;

            ((List<User>)HttpContext.Application["users"]).
                Where(i => i.UserRole == UserRole.TRENER && i.IsBlocked == false && i.TrenerCenter.Name == centerName)
                .ToList().ForEach(u => u.IsBlocked = true);

            List<User> users = (List<User>)HttpContext.Application["users"];

            WriteXML.UsersWrite(users);


            return RedirectToAction("VlasnikCenters", "Vlasnik");
        }

        [HttpPost]
        public ActionResult BlockTrainer(string username)
        {
            ((List<User>)HttpContext.Application["users"]).Find(i => i.Username == username).IsBlocked = true;

            List<User> users = (List<User>)HttpContext.Application["users"];
            WriteXML.UsersWrite(users);

            return RedirectToAction("TrainersTable", "Vlasnik");
        }

        [HttpPost]
        public ActionResult ChangeCommentState(string commentState, string centerName)
        {
            if(CommentState.ACCEPTED.ToString() == commentState)
            {
                (CommentData.GetAllPendingCommentsForAdminFc((User)Session["LOGGEDIN"]))
                     .Find(i => i.FCenterId == centerName).CommentState = CommentState.ACCEPTED;
            }
            else
            {
                (CommentData.GetAllPendingCommentsForAdminFc((User)Session["LOGGEDIN"]))
                    .Find(i => i.FCenterId == centerName).CommentState = CommentState.DECLINED;
            }


            List<User> users = (List<User>)HttpContext.Application["users"];
            WriteXML.UsersWrite(users);

            return RedirectToAction("PendingCommentRequests", "Vlasnik");
        }
    }
}