using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class GroupTraining
    {
        public string Name { get; set; }
        public string TrainingType { get; set; }
        public string FCenterId { get; set; }
        public int TrainingDuration { get; set; }
        public string TrainingDateTime { get; set; }
        public int MaxParticipants { get; set; }
        public bool IsDeleted { get; set; }
        public List<string> Participants { get; set; }

        public GroupTraining()
        {
            Participants = new List<string>();
        }
    }
}