using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.HelperClasses
{
    public class CommentData
    {
        public static List<Comment> GetAllPendingCommentsForAdminFc(User user)
        {
            try
            {

                List<Comment> com = new List<Comment>();
                foreach(var item in user.ValsnikCenters)
                {
                    foreach(var comment in item.Comments)
                    {
                        if(comment.CommentState == CommentState.PENDING)
                        {
                            com.Add(comment);
                        }
                    }
                }

                return com;

                //return user.ValsnikCenters.Select(i => i.Comments
                //.Where(x => x.CommentState == CommentState.PENDING).ToList()).ToList()[0];
            }
            catch
            {
                return new List<Comment>();
            }
            
        }

        public static List<Comment> GetAllAcceptedCommentsForFc(string centerName, List<User> users)
        {
            try
            {
                List<Comment> com = new List<Comment>();

                foreach (var user in users)
                {
                    if(user.UserRole == UserRole.VLASNIK)
                    {
                        foreach(var item in user.ValsnikCenters)
                        {
                            foreach(var comment in item.Comments)
                            {
                                if(comment.CommentState == CommentState.ACCEPTED && comment.FCenterId == centerName)
                                {
                                    com.Add(comment);
                                }
                            }
                        }
                    }
                }


                return com;
                //return users.Where(i => i.UserRole == UserRole.VLASNIK).ToList()
                //        .Select(i => i
                //        .ValsnikCenters.Select(x => x
                //        .Comments.Where(y => y.CommentState == CommentState.ACCEPTED).ToList()).ToList()[0]).ToList()[0];
            }
            catch
            {
                return new List<Comment>();
            }
         
        }
    }
}