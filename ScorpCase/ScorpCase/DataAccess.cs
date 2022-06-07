using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ScorpCase
{
    public class DataAccess
    {
        private const string _db_posts_query = "SELECT * FROM POST WHERE ID IN (POST_IDS)";
        private const string _db_liked_query = "SELECT COUNT(1) FROM LIKE WHERE POST_ID = @POST_ID AND USER_ID = @USER_ID";
        private const string _db_user_query = "SELECT * FROM USER WHERE USER_ID = @USER_ID";
        private const string _db_follow_query = "SELECT * FROM FOLLOW WHERE FOLLOWER_ID = @FOLLOWER_ID AND FOLLOWING_ID=@USER_ID";

        private string _connectionString;
        public DataAccess(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("WebApiDatabase");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="post_ids"></param>
        /// <returns></returns>
        public List<Post> get_posts(int user_id, List<int> post_ids)
        {
            var listPost = new List<Post>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(_db_posts_query, con);
                    con.Open();

                    //get posts
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        #region User Info
                        User userDto = new User();
                        cmd = new SqlCommand(_db_user_query, con);
                        SqlParameter userIdParam = cmd.Parameters.Add("@USER_ID", SqlDbType.Int, 15);
                        userIdParam.Value = Convert.ToInt32(rdr["USER_ID"]);

                        SqlDataReader userReader = cmd.ExecuteReader();
                        while (userReader.Read())
                        {
                            cmd = new SqlCommand(_db_follow_query, con);
                            SqlParameter followingIdParam = cmd.Parameters.Add("@FOLLOWING_ID", SqlDbType.Int, 15);
                            followingIdParam.Value = user_id;
                            SqlParameter followerId = cmd.Parameters.Add("@FOLLOWER_ID", SqlDbType.Int, 15);
                            followingIdParam.Value = Convert.ToInt32(rdr["USER_ID"]);

                            userDto = new User()
                            {
                                id = Convert.ToInt32(userReader["USER_ID"]),
                                username = userReader["USERNAME"].ToString(),
                                full_name = userReader["FULLNAME"].ToString(),
                                profile_picture = userReader["PROFIL_PICTURE"].ToString(),
                                followed = Convert.ToInt32(cmd.ExecuteScalar()) == 1,
                            };
                        }
                        #endregion

                        #region Post Info
                        cmd = new SqlCommand(_db_liked_query, con);
                        SqlParameter postParam = cmd.Parameters.Add("@POST_ID", SqlDbType.Int, 15);
                        postParam.Value = Convert.ToInt32(rdr["ID"]);
                        userIdParam.Value = user_id;

                        //find index for same order
                        int index = post_ids.IndexOf(Convert.ToInt32(rdr["ID"]));
                        listPost.Insert(index, new Post
                        {
                            id = Convert.ToInt32(rdr["ID"]),
                            description = rdr["DESCRIPTION"].ToString(),
                            created_at = Convert.ToInt32(rdr["CREATED_AT"]),
                            image = rdr["IMAGE"].ToString(),
                            liked = Convert.ToInt32(cmd.ExecuteScalar()) == 1,
                            owner = userDto,
                        });
                        #endregion
                    }
                    con.Close();
                }

                //Empty object is added for non-existing posts.
                if (listPost.Count != post_ids.Count)
                {
                    post_ids.ForEach(x =>
                    {
                        var obj = listPost.Where(item => item.id == x);
                        if (obj == null)
                        {
                            int index = post_ids.IndexOf(x);
                            listPost.Insert(index, new Post());
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listPost;
        }
    }
}
