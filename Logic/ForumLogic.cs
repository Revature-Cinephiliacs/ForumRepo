using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobalModels;
using Repository;

namespace BusinessLogic
{
    /// <summary>
    /// Implements the interface IForumLogic.
    /// Methods are used to read and write objects in the repository.
    /// Return appropriate response to calling methods
    /// </summary>
    public class ForumLogic : Interfaces.IForumLogic
    {
        private readonly RepoLogic _repo;

        public ForumLogic(RepoLogic repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// To post/save comment 
        /// Takes Comments Objects as param
        /// Returns true if saved successfully.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<bool> CreateComment(NewComment comment)
        {
            var repoComment = Mapper.NewCommentToNewRepoComment(comment);
            return await _repo.AddComment(repoComment);
        }

        /// <summary>
        /// Method to Psot/save Duscussion 
        /// Takes Discussion Object as Pareameter
        /// Returns true if saved successfully.
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        public async Task<bool> CreateDiscussion(NewDiscussion discussion)
        {
            var repoDiscussion = Mapper.NewDiscussionToNewRepoDiscussion(discussion);
            Console.WriteLine(repoDiscussion.DiscussionId);
            var repoTopic = new Repository.Models.Topic();
            repoTopic.TopicName = discussion.Topic;
            return await _repo.AddDiscussion(repoDiscussion, repoTopic);
        }

        /// <summary>
        /// Method to get all comments
        /// Takes discussion id as param
        /// Returns List of Comments  
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public async Task<List<Comment>> GetComments(Guid discussionid)
        {
            List<Repository.Models.Comment> repoComments = await _repo.GetMovieComments(discussionid.ToString());
            if (repoComments == null)
            {
                Console.WriteLine("ForumLogic.GetComments() was called with a discussionid that doesn't exist.");
                return null;
            }

            List<Comment> comments = new List<Comment>();
            foreach (var repoComment in repoComments)
            {
                comments.Add(Mapper.RepoCommentToComment(repoComment));
            }
            return comments;
        }

        /// <summary>
        /// Method to get all comments/page
        /// Takes Duscussion id & page (int) as paremeter
        /// Returns list of comments  
        /// </summary>
        /// <param name="discussionid"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<List<Comment>> GetCommentsPage(Guid discussionid, int page)
        {
            if (page < 1)
            {
                Console.WriteLine("ForumLogic.GetCommentsPage() was called with a negative or zero page number.");
                return null;
            }

            Repository.Models.Setting pageSizeSetting = _repo.GetSetting("commentspagesize");
            int pageSize = 1;//pageSizeSetting.IntValue ?? default(int);
            if (pageSize < 1)
            {
                Console.WriteLine("ForumLogic.GetCommentsPage() was called but the commentspagesize is invalid");
                return null;
            }

            List<Repository.Models.Comment> repoComments = await _repo.GetMovieComments(discussionid.ToString());
            if (repoComments == null)
            {
                Console.WriteLine("ForumLogic.GetCommentsPage() was called with a discussionid that doesn't exist.");
                return null;
            }
            repoComments = repoComments.OrderByDescending(c => c.CreationTime).ToList<Repository.Models.Comment>();

            int numberOfComments = repoComments.Count;
            int startIndex = pageSize * (page - 1);

            if (startIndex > numberOfComments - 1)
            {
                Console.WriteLine("ForumLogic.GetCommentsPage() was called for a page number without comments.");
                return null;
            }

            int endIndex = startIndex + pageSize - 1;
            if (endIndex > numberOfComments - 1)
            {
                endIndex = numberOfComments - 1;
            }

            List<Comment> comments = new List<Comment>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                comments.Add(Mapper.RepoCommentToComment(repoComments[i]));
            }
            return comments;
        }

        /// <summary>
        /// Method to set comments 
        /// Takes number of pagesize as a parameter 
        /// Retun true if succefully saved.
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public async Task<bool> SetCommentsPageSize(int pagesize)
        {
            if (pagesize < 1 || pagesize > 100)
            {
                Console.WriteLine("ForumLogic.SetCommentsPageSize() was called with an invalid pagesize.");
                return false;
            }

            Repository.Models.Setting setting = new Repository.Models.Setting();
            setting.Setting1 = "commentspagesize";
            setting.IntValue = pagesize;
            return await _repo.SetSetting(setting);
        }

        /// <summary>
        /// Method for getting all dissussions for a specific movie 
        /// Takes movieid as a parameter 
        /// Returns the list if duscussion 
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public async Task<List<Discussion>> GetDiscussions(string movieid)
        {
            List<Repository.Models.Discussion> repoDiscussions = await _repo.GetMovieDiscussions(movieid);
            if (repoDiscussions == null)
            {
                Console.WriteLine("ForumLogic.GetDiscussions() was called with a movieid that doesn't exist.");
                return null;
            }

            List<Discussion> discussions = new List<Discussion>();
            foreach (var repoDiscussion in repoDiscussions)
            {
                // Get the topic associated with this discussion

                Repository.Models.Topic topic = _repo.GetDiscussionTopic(repoDiscussion.DiscussionId);
                if (topic == null)
                {
                    topic = new Repository.Models.Topic();
                    topic.TopicName = "None";
                }
                discussions.Add(Mapper.RepoDiscussionToDiscussion(repoDiscussion, topic));
            }
            return discussions;
        }

        /// <summary>
        /// Method to get discussion by id
        /// It takes discussion id as a pareameter send it to RepoLogic 
        /// If duscussion id is null returns null
        /// if RepoLogic returns Duscussion than returns that discussion.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public async Task<Discussion> GetDiscussion(Guid discussionid)
        {
            Repository.Models.Discussion repoDiscussion = await _repo.GetDiscussion(discussionid.ToString());
            if (repoDiscussion == null)
            {
                Console.WriteLine("ForumLogic.GetDiscussion() was called for an invalid discussionid.");
                return null;
            }

            // Get the topic associated with this discussion
            Repository.Models.Topic topic = _repo.GetDiscussionTopic(repoDiscussion.DiscussionId);
            if (topic == null)
            {
                topic = new Repository.Models.Topic();
                topic.TopicName = "None";
            }
            Discussion discussion = Mapper.RepoDiscussionToDiscussion(repoDiscussion, topic);
            return discussion;
        }

        /// <summary>
        /// Method to retrive all topics from the RepoLoic (DB)
        /// if topic is null then returns null
        /// </summary>
        /// <returns>List<Topic></returns>
        public async Task<List<string>> GetTopics()
        {
            var repoTopics = await _repo.GetTopics();
            if (repoTopics == null)
            {
                Console.WriteLine("ForumLogic.GetTopics() was called but there are no topics.");
                return null;
            }

            var topics = new List<string>();
            foreach (var repoTopic in repoTopics)
            {
                topics.Add(repoTopic.TopicName);
            }
            return topics;
        }
    }
}