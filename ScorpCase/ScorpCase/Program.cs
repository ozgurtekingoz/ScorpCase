using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScorpCase
{
    class Program
    {
        private static IConfiguration _iconfiguration;
        static void Main(string[] args)
        {
            #region First question
            DataAccess ins = new DataAccess(_iconfiguration);
            ins.get_posts(1, new List<int>() { 1, 2, 3, 4, 5 });
            GetAppSettingsFile();

            #endregion

            #region Second Question


            List<Post> postList1 = new List<Post>() {
                                new Post() { id = 1, description = "test1" },
                                new Post() { id = 2, description = "test2" },
                                new Post() { id = 3, description = "test3" },
                                new Post() { id = 4, description = "test4" } };

            List<Post> postList2 = new List<Post>() {
                                new Post() { id = 1, description = "test1" },
                                new Post() { id = 2, description = "test2" },
                                new Post() { id = 5, description = "test5" },
                                new Post() { id = 6, description = "test6" } };

            List<Post> postList3 = new List<Post>() {
                                new Post() { id = 1, description = "test1" },
                                new Post() { id = 7, description = "test7" },
                                new Post() { id = 8, description = "test8" },
                                new Post() { id = 5, description = "test5" } };

            List<List<Post>> finalList = new List<List<Post>>();
            finalList.Add(postList1);
            finalList.Add(postList2);
            finalList.Add(postList3);

            List<Post> returnList = merge_posts(finalList);
            #endregion
        }

        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }

        public static List<Post> merge_posts(List<List<Post>> posts)
        {
            #region First Solution
            List<Post> returnList = new List<Post>();

            foreach (List<Post> items in posts)
            {
                foreach (Post postItem in items)
                {
                    var existItem = returnList.Find(item => item.id == postItem.id);
                    if (existItem == null)
                    {
                        returnList.Add(postItem);
                    }
                }
            }
            #endregion

            #region Second Solution
            int length = posts.Count;
            List<Post> returnList = posts[0];

            int x = 1;
            int i = 0;
            while (i < posts[x].Count)
            {
                var existItem = returnList.Find(item => item.id == posts[x][i].id);
                if (existItem == null)
                    returnList.Add(posts[x][i]);

                if (i == posts[x].Count - 1)
                {
                    i = 0; x++;
                }
                else
                    i++;

                if (length == x)
                    break;
            }
            #endregion
            returnList.OrderBy(c => c.created_at).ThenBy(c => c.id);
            return returnList;
        }
    }
}
