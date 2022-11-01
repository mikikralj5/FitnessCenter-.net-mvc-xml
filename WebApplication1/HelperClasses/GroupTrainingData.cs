using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.HelperClasses
{
    public class GroupTrainingData
    {
        public static List<GroupTraining> GetAllFutureTraining(List<User> users)
        {

            List<GroupTraining> grp = new List<GroupTraining>();
            foreach(var user in users)
            {
                if(user.UserRole == UserRole.TRENER && user.IsBlocked==false)
                {
                    foreach(var trening in user.TrenerGroupTrainings)
                    {
                        if(trening.IsDeleted == false && DateTime.ParseExact(trening.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) > DateTime.Now)
                        {
                            grp.Add(trening);
                        }
                    }
                }
            }

           
            return grp;
        }

        public static List<GroupTraining> GetFutureTrainingsForUser(User user)
        {

            try
            {
                return user.TrenerGroupTrainings
                       .Where(i => i.IsDeleted == false && DateTime.ParseExact(i.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) > DateTime.Now).ToList();

            }
            catch
            {
                return new List<GroupTraining>();
            }
           
        }

        public static List<GroupTraining> GetPastTrainingForUser(User user)
        {
            List<GroupTraining> grp = new List<GroupTraining>();

                if (user.UserRole == UserRole.TRENER)
                {
                    foreach(var training in user.TrenerGroupTrainings)
                    {
                        if(training.IsDeleted == false && DateTime.ParseExact(training.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) < DateTime.Now)
                        {
                             grp.Add(training);
                        }
                    }

                    //return user.TrenerGroupTrainings
                    //        .Where(i => i.IsDeleted == false && DateTime.ParseExact(i.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) < DateTime.Now).ToList();
                }
                else
                {

                    foreach (var training in user.PosetiocGroupTrainings)
                    {
                        if (training.IsDeleted == false && DateTime.ParseExact(training.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) < DateTime.Now)
                        {
                            grp.Add(training);
                        }
                    }

                //return user.PosetiocGroupTrainings
                //            .Where(i => i.IsDeleted == false && DateTime.ParseExact(i.TrainingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) < DateTime.Now).ToList();
                }
            return grp;        
        }

        public static GroupTraining GetByName(string trainingName, List<User> users)
        {
            GroupTraining grp = new GroupTraining();
            foreach(var user in users)
            {
                if(user.UserRole == UserRole.TRENER && user.IsBlocked == false)
                {
                    foreach(var training in user.TrenerGroupTrainings)
                    {
                        if(training.Name == trainingName && training.IsDeleted == false)
                        {
                            grp = training;
                        }
                    }
                }
            }

            return grp;
            //try
            //{
            //    return users.Where(i => i.UserRole == UserRole.TRENER && i.IsBlocked == false).ToList()
            //                  .Select(i => i.TrenerGroupTrainings.Where(x => x.IsDeleted == false && x.Name == trainingName).ToList())
            //                  .FirstOrDefault()[0];
            //}
            //catch
            //{
            //    return new GroupTraining();
            //}          
        }
        
        public static List<string> GetParticipantsForTraining(string trainingName, List<User> users)
        {
            try
            {
                GroupTraining gr = GetByName(trainingName, users);

                return gr.Participants;
            }
            catch
            {
                return new List<string>();
            }

        }

    }
    
}