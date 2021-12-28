# Scorp
Scorp Case Study


program.cs file contains methods

get_posts(int user_id, List<int> post_ids)

merge_posts(List<List<post>> list_of_posts)

//Hypothetical tables stored in database.
//These are supposed to be data access objects to the database and called on demand.
public static List<post> existing_posts = new List<post>();
public static List<like> existing_likes = new List<like>();    
public static List<user> existing_users = new List<user>();    
public static List<follow> existing_follow = new List<follow>();

