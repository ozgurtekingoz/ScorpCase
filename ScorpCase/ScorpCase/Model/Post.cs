using System;
using System.Collections.Generic;
using System.Text;

namespace ScorpCase
{
    public class Post
    {
        public int id { get; set; }
        public string description { get; set; }
        public User owner { get; set; }
        public string image { get; set; }
        public int created_at { get; set; }
        public bool liked { get; set; }
    }
}
