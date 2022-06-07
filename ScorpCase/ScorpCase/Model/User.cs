using System;
using System.Collections.Generic;
using System.Text;

namespace ScorpCase
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string profile_picture { get; set; }
        public bool followed { get; set; }
    }
}
