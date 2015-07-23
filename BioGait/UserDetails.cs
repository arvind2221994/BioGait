using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioGait
{
    public class UserDetails
    {
        public string Name;
        public string Occupation;
        public int Age;
        public float Height;
        public float Weight;
        public string Blood_group;

        public UserDetails()
        {
            Name = "";
            Occupation = "";
            Age = 0;
            Height = 0;
            Weight = 0;
            Blood_group = "A+";
        }

        public UserDetails(string name, string occupation, int age, float height, float weight, string blood_group) 
        {
            Name = name;
            Occupation = occupation;
            Age = age;
            Height = height;
            Weight = weight;
            Blood_group = blood_group;

        }
    }
    
}
