using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    public enum UserRole
    {
        POSETILAC,
        TRENER,
        VLASNIK
    }


    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public UserRole UserRole { get; set; }
        public bool IsBlocked { get; set; }
        public List<FitnessCenter> ValsnikCenters { get; set; }
        public FitnessCenter TrenerCenter { get; set; }
        public List<GroupTraining> PosetiocGroupTrainings { get; set; }
        public List<GroupTraining> TrenerGroupTrainings { get; set; }

        public User()
        {
            ValsnikCenters = new List<FitnessCenter>();
            TrenerCenter = new FitnessCenter();
            PosetiocGroupTrainings = new List<GroupTraining>();
            TrenerGroupTrainings = new List<GroupTraining>();
        }
    }
}