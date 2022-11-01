using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.HelperClasses
{
    public class FitnessCenterData
    {
        public static List<FitnessCenter> GetAll(List<User> users)
        {
            List<FitnessCenter> fc = new List<FitnessCenter>();

            foreach(var user in users)
            {
                if(user.UserRole == UserRole.VLASNIK)
                {
                    foreach(var center in user.ValsnikCenters)
                    {
                        if(center.IsDelted == false)
                        {
                            fc.Add(center);
                        }
                    }
                }
            }

            return fc;

            //try{
            //    return users.
            //         Where(i => i.UserRole == UserRole.VLASNIK).ToList().
            //         Select(i => i.ValsnikCenters.Where(x => x.IsDelted == false).ToList()).ToList()[0];
            //}
            //catch (Exception e)
            //{
            //    return new List<FitnessCenter>();
            //}

        }


        public static FitnessCenter FindByName(string centerName, List<User> users)
        {
            FitnessCenter fc = new FitnessCenter();

            foreach(var user in users)
            {
                if(user.UserRole == UserRole.VLASNIK)
                {
                    foreach(var center in user.ValsnikCenters)
                    {
                        if(center.Name == centerName && center.IsDelted == false)
                        {
                            fc = center;
                        }

                    }
                }
            }

            return fc;

            //try
            //{
            //    return users.
            //        Where(i => i.UserRole == UserRole.VLASNIK).ToList().
            //        Select(i => i.ValsnikCenters.Where(x => x.IsDelted == false && x.Name == centerName).ToList()).FirstOrDefault()[0];
            //}
            //catch
            //{
            //    return new FitnessCenter();
            //}

        }
    }

}