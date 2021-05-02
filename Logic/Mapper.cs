using System;
using GlobalModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace BusinessLogic
{
    public static class Mapper
    {
        private static readonly string _userapi = "http://20.45.2.119/user/";
        private static readonly string _movieapi = "http://20.94.153.81/movie/";

        /// <summary>
        /// Maps an instance of Repository.Models.Comment onto a new instance of
        /// GlobalModels.Comment
        /// </summary>
        /// <param name="repoComment"></param>
        /// <returns></returns>
        public static async Task<Comment> RepoCommentToComment(Repository.Models.Comment repoComment)
        {
            string username = await Task.Run(() => GetUsernameFromAPI(repoComment.UserId));

            var comment = new Comment(Guid.Parse(repoComment.CommentId), Guid.Parse(repoComment.DiscussionId), username,
                    repoComment.CommentText, repoComment.IsSpoiler, repoComment.ParentCommentid, (int)repoComment.Likes);
            return comment;
        }

        /// <summary>
        /// Maps an instance of Repository.Model.Comment onto a new instance of
        /// GlobalModels.NestedComment
        /// </summary>
        /// <param name="repoComment"></param>
        /// <returns></returns>
        public static async Task<NestedComment> RepoCommentToNestedComment(Repository.Models.Comment repoComment)
        {
            string username = await Task.Run(() => GetUsernameFromAPI(repoComment.UserId));

            var nestedComment = new NestedComment(Guid.Parse(repoComment.CommentId), Guid.Parse(repoComment.DiscussionId), username,
                    repoComment.CommentText, repoComment.IsSpoiler, repoComment.ParentCommentid, (int)repoComment.Likes, repoComment.CreationTime);

            return nestedComment;
        }

        /// <summary>
        /// A helper recursive function that will take a list of discussion comments and a parent comment
        /// and the child replies to the parent commment
        /// </summary>
        /// <param name="repoComments"></param>
        /// <param name="parent"></param>
        public static void AddReplies(List<NestedComment> repoComments, NestedComment parent)
        {
            for (int i = 0; i < repoComments.Count; i++)
            {
                string parentId = parent.Commentid.ToString();
                if (repoComments[i].ParentCommentid == parentId)
                {
                    parent.Replies.Add(repoComments[i]);
                    AddReplies(repoComments, repoComments[i]);
                }
            }
        }

        /// <summary>
        /// Maps an instance of GlobalModels.NewDiscussion onto a new instance of
        /// Repository.Models.Discussion. Sets Repository.Models.Review.CreationTime
        /// to the current time.
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        public static Repository.Models.Discussion NewDiscussionToNewRepoDiscussion(
            NewDiscussion discussion)
        {
            var repoDiscussion = new Repository.Models.Discussion();
            repoDiscussion.DiscussionId = Guid.NewGuid().ToString();
            repoDiscussion.MovieId = discussion.Movieid;
            repoDiscussion.UserId = discussion.Userid;
            repoDiscussion.Subject = discussion.Subject;
            repoDiscussion.CreationTime = DateTime.Now;

            return repoDiscussion;
        }

        /// <summary>
        /// Maps an instance of GlobalModels.NewComment onto a new instance of
        /// Repository.Models.Comment. Sets Repository.Models.Review.CreationTime
        /// to the current time.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static Repository.Models.Comment NewCommentToNewRepoComment(NewComment comment)
        {
            var repoComment = new Repository.Models.Comment();
            repoComment.CommentId = Guid.NewGuid().ToString();
            repoComment.DiscussionId = comment.Discussionid.ToString();
            repoComment.UserId = comment.Userid;
            repoComment.CommentText = comment.Text;
            repoComment.CreationTime = DateTime.Now;
            repoComment.IsSpoiler = comment.Isspoiler;
            repoComment.ParentCommentid = comment.ParentCommentid;
            return repoComment;
        }

        /// <summary>
        /// Convert a Repository Dicussion object to a frontend DiscussionT
        /// </summary>
        /// <param name="dis"></param>
        /// <returns></returns>
        public static async Task<DiscussionT> RepoDiscussionToDiscussionT(Repository.Models.Discussion dis)
        {
            string username = await Task.Run(() => GetUsernameFromAPI(dis.UserId));
            DiscussionT gdis = new();
            gdis.DiscussionId = dis.DiscussionId;
            gdis.MovieId = dis.MovieId;
            gdis.Userid = username;
            gdis.Subject = dis.Subject;
            gdis.CreationTime = dis.CreationTime;
            gdis.Likes = (int)dis.Totalikes;
            foreach (var ct in dis.Comments)
            {
                username = await Task.Run(() => GetUsernameFromAPI(ct.UserId));
                Comment nc = new Comment(Guid.Parse(ct.CommentId), Guid.Parse(ct.DiscussionId), username, ct.CommentText, ct.IsSpoiler, ct.ParentCommentid, (int)ct.Likes);
                gdis.Comments.Add(nc);
            }
            
            foreach (var top in dis.DiscussionTopics)
            {
                gdis.DiscussionTopics.Add(top.TopicId);
            }

            return gdis;
        }

        /// <summary>
        /// Maps an instance of GlobalModels.Topic onto a new intance of Repository.Models.Topic.
        /// Creates a new Guid and converts it to a string
        /// Assigns the new topic name
        /// Returns the new Repository.Models.Topic
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        internal static Repository.Models.Topic NewTopicToRepoTopic(string topic)
        {
            Repository.Models.Topic newTopic = new Repository.Models.Topic();
            newTopic.TopicId = Guid.NewGuid().ToString();
            newTopic.TopicName = topic;
            return newTopic;
        }

        /// <summary>
        /// Converts a RepoTopic object into a DTOTopic object
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        internal static Topic RepoTopicToTopic(Repository.Models.Topic topic)
        {
            return new Topic(topic.TopicId, topic.TopicName);
        }

        /// <summary>
        /// Sends a notification to the movie api
        /// </summary>
        /// <param name="repoDisc"></param>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public static async Task<bool> SendNotification(Repository.Models.Discussion repoDisc, string discussionid)
        {
            DiscussionNotification dn = new DiscussionNotification(repoDisc.MovieId, repoDisc.UserId, discussionid);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsJsonAsync($"{_movieapi}notification/discussion", dn);
            response.EnsureSuccessStatusCode();
            return true;
        }

        /// <summary>
        /// Gets the username of the user from the userapi using userid
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static async Task<string> GetUsernameFromAPI(string userid)
        {
            HttpClient client = new HttpClient();
            string path = _userapi + "username/" + userid;
            HttpResponseMessage response = await client.GetAsync(path);
            if(response.IsSuccessStatusCode)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonContent);
                // JObject json = JObject.Parse(jsonContent);
                // string username = json.ToString();
                return jsonContent;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the movie title from the movie id
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public static async Task<string> GetMovieFromAPI(string movieid)
        {
            HttpClient client = new HttpClient();
            string path = _movieapi + movieid;
            HttpResponseMessage response = await client.GetAsync(path);
            if(response.IsSuccessStatusCode)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonContent);
                string moviename = json["Title"].ToString();
                return moviename;
            }
            else
            {
                return null;
            }
        }
    }
}
