using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static Scorpo.Dtos;
using static Scorpo.Entities;

namespace Scorpo
{
    public class Entities
    {
        public class user
        {
            public int id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string full_name { get; set; }
            public string profile_picture { get; set; }
            public string bio { get; set; }
            public int created_at { get; set; }
        }
        public class post
        {
            public int id { get; set; }
            public string description { get; set; }

            [ForeignKey("user_id")]
            public int user_id { get; set; }
            public virtual user user { get; set; }
            public string image { get; set; }
            public int created_at { get; set; }
        }
        public class like
        {
            public int id { get; set; }

            [ForeignKey("post_id")]
            public int post_id { get; set; }
            public virtual post post { get; set; }

            [ForeignKey("user_id")]
            public int user_id { get; set; }
            public virtual user user { get; set; }
            public int created_at { get; set; }
        }

        public class follow
        {
            [ForeignKey("follower_id")]
            public int follower_id { get; set; }
            public virtual user follower { get; set; }

            [ForeignKey("following_id")]
            public int following_id { get; set; }
            public virtual user following { get; set; }

            public int created_at { get; set; }
        }
    }

    public class Dtos
    {
        public class UserModel
        {
            public int id { get; set; }
            public string username { get; set; }
            public string full_name { get; set; }
            public string profile_picture { get; set; }
            public bool followed { get; set; }
        }

        public class get_posts_result
        {
            public int id { get; set; }
            public string description { get; set; }
            public UserModel owner { get; set; }
            public string image { get; set; }
            public int created_at { get; set; }
            public bool liked { get; set; }
        }

        public class merge_posts_result
        {
            public int id { get; set; }
            public string description { get; set; }
            public string image { get; set; }
            public int created_at { get; set; }
        }
    }
    public class Program
    {
        //Hypothetical tables stored in database.
        public static List<post> existing_posts = new List<post>();
        public static List<like> existing_likes = new List<like>();    
        public static List<user> existing_users = new List<user>();    
        public static List<follow> existing_follow = new List<follow>();    
        public static List<get_posts_result> get_posts(int user_id, List<int> post_ids)
        {
            List<get_posts_result> result = new List<get_posts_result>(); 
            bool user_liked = false;
            bool user_followed = false;
            for (int i = 0; i < post_ids.Count(); i++)
            {
                //execute single query against post table for post_ids[i]
                var existing_post = existing_posts.FirstOrDefault(c => c.id == post_ids[i]);
                if(existing_post != null)
                {
                    //execute single query against user table for owner of post_ids[i] if exists
                    var owner = existing_users.FirstOrDefault(c => c.id == existing_post.user_id);
                    //if requesting_user is valid
                    if (user_id != 0)
                    {
                        //execute single query against like table if requesting_user liked post_ids[i]
                        var liked_by_id = existing_likes.FirstOrDefault(c=>c.user_id == user_id && c.post_id == post_ids[i])?.id;
                        //execute single query against follow table if requesting_user follows owner of post_ids[i]
                        var follow_by_following_id = existing_follow.FirstOrDefault(c=>c.follower_id == user_id && c.following_id == existing_post.user_id)?.following_id;
                        user_liked = liked_by_id != null; 
                        user_followed = follow_by_following_id != null;   
                    }
                    //add result to main body. And return in same order of post_ids
                    result.Add(new get_posts_result
                    {
                        created_at = existing_post.created_at,
                        description = existing_post.description,
                        id = existing_post.id,
                        image = existing_post.image,
                        liked = user_liked,
                        owner = owner is null ? null : new UserModel
                        {
                            id = owner.id,
                            followed = user_followed,
                            full_name = owner.full_name,
                            profile_picture = owner.profile_picture,
                            username = owner.username
                        }
                    });
                }
            }
            return result;  
        }
        public static List<merge_posts_result> merge_posts(List<List<post>> list_of_posts)
        {
            //initialize result
            List<merge_posts_result> result = new List<merge_posts_result>(); 
            //dictionary to hold distinct post_ids
            Dictionary<int, post> dict = new Dictionary<int, post>();

            for (int i = 0; i < list_of_posts.Count(); i++)
            {
                for (int j = 0; j < list_of_posts[i].Count(); j++)
                {
                    //if dictionary does not contain a post_id, add to dictionary and list of result
                    if (!dict.ContainsKey(list_of_posts[i][j].id))
                    {
                        dict.Add(list_of_posts[i][j].id, list_of_posts[i][j]);
                        result.Add(new merge_posts_result
                        {
                            description = list_of_posts[i][j].description,
                            created_at = list_of_posts[i][j].created_at,
                            image = list_of_posts[i][j].image,
                            id = list_of_posts[i][j].id
                        });
                    }
                    else
                    {
                        //do not add to result list
                    }
                }
            }
            //order by created_at descending and id descending
            return result.OrderByDescending(c=>c.created_at).OrderByDescending(c=>c.id).ToList();    
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
