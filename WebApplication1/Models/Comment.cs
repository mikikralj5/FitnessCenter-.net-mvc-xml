using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public enum CommentState
    {
        PENDING,
        ACCEPTED,
        DECLINED
    }
    

    public class Comment
    {
        public int Id { get; set; }
        public string CreatorUsername { get; set; }
        public string FCenterId { get; set; }
        public string CommentText { get; set; }
        public int Rating { get; set; }
        public CommentState CommentState { get; set; }
        
    }
}