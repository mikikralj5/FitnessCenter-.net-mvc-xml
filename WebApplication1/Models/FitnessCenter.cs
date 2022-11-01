using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class FitnessCenter
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Year { get; set; }
        public string VlasnikId { get; set; }
        public int MonthlyPrice { get; set; }
        public int YearlyPrice { get; set; }
        public int OneTrainingPrice { get; set; }
        public int GroupTrainingPrice { get; set; }
        public int AssistantTrainingPrice { get; set; }
        public bool IsDelted { get; set; }
        public List<Comment> Comments { get; set; }

        public FitnessCenter()
        {
            Comments = new List<Comment>();
        }
    }
}