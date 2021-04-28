using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using GlobalModels;

namespace BusinessLogic.Interfaces
{
    public interface IForumLogic
    {
        /// <summary>
        /// Method to retrieve all topics from the database
        /// If there are no topics, return null
        /// </summary>
        /// <returns>List &lt; Topic ></returns>
        public Task<List<Topic>> GetTopics();

        /// <summary>
        /// Method for getting all discussions for a specific movie 
        /// Takes movieid as a parameter 
        /// Returns the list of discussions 
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public Task<List<DiscussionT>> GetDiscussions(string movieid);
        

        /// <summary>
        /// Method for getting all discussions for a specific movie 
        /// * sort as specified by sortingOrder param.
        /// * paginate as specified by page (pagenumber).
        /// Takes movieid, page, and sortingOrder as a parameter 
        /// Returns the list of discussions 
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public Task<List<DiscussionT>> GetDiscussionsPage(string movieid, int page, string sortingOrder);

        /// <summary>
        /// Method to get a discussion by id
        /// It takes discussion id as a parameter
        /// If discussionid doesn't exist, return null
        /// If discussionid exists, return the discussion
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        Task<Discussion> GetDiscussion(Guid discussionid);

        /// <summary>
        /// Method to get all comments from a discussion
        /// Takes discussion id as param
        /// Returns List of Comments  
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public Task<List<Comment>> GetComments(Guid discussionid);

        /// <summary>
        /// Method to get all comments/page (pagination)
        /// Takes Duscussion id &amp; page (int) as paremeter
        /// Returns correct list of comments  
        /// </summary>
        /// <param name="discussionid"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Task<List<NestedComment>> GetCommentsPage(Guid discussionid, int page, string sortingOrder);

        /// <summary>
        /// Method to set comments 
        /// Takes number of pagesize as a parameter 
        /// Return true if sucessfully saved.
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public Task<bool> SetCommentsPageSize(int pagesize);

        /// <summary>
        /// Method to post/save a discussion 
        /// Takes Discussion Object as Pareameter
        /// Returns true if saved successfully.
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        public Task<bool> CreateDiscussion(NewDiscussion discussion);

        /// <summary>
        /// Method to post/save a comment
        /// Takes Comments Objects as param
        /// Returns true if saved successfully.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public Task<bool> CreateComment(NewComment comment);

        /// <summary>
        /// Gets a sorted list of Discussions based off number of comments.
        /// Parameter type determines is it's an ascending list, or descending
        /// type = "a" for ascending, type = "d" for descending
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<List<DiscussionT>> GetSortedDiscussionsByComments(string type);

        /// <summary>
        /// Gets a sorted list of Discussions based off number of comments.
        /// Parameter type determines is it's an ascending list, or descending
        /// type = "a" for ascending, type = "d" for descending
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<List<DiscussionT>> GetDiscussionsByTopicId(string type);

        /// <summary>
        /// Creates a new topic in the database 
        /// Returns true if successfully created
        /// Returns false if failed to create
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public Task<bool> CreateTopic(string topic);

        /// <summary>
        /// Changes a spoiler tag from true &lt; - > false
        /// Returns true if successful in switching spoilertag bool
        /// Returns false is failed
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public Task<bool> ChangeSpoiler(Guid commentid);

        /// <summary>
        /// Deletes a comment from the database
        /// Returns true if successful
        /// Returns false if failed
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public Task<bool> DeleteComment(Guid commentid);

        /// <summary>
        /// Deletes a discussion from the database
        /// Returns true if successful
        /// Returns false if failed
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public Task<bool> DeleteDiscussion(Guid discussionid);

        /// <summary>
        /// Deletes a topic from the database
        /// Returns true if successful
        /// Returns false if failed
        /// </summary>
        /// <param name="topicid"></param>
        /// <returns></returns>
        public Task<bool> DeleteTopic(Guid topicid);

        /// <summary>
        /// Adds a new user-discussion follow relationship
        /// Returns true if successful
        /// Returns false if failed
        /// </summary>
        /// <param name="discussionid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Task<bool> FollowDiscussion(Guid discussionid, string userid);

        /// <summary>
        /// Returns the list of discussions that a user is following
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Task<List<DiscussionT>> GetFollowDiscList(string userid);

        /// <summary>
        /// Increments a comment's likes
        /// Returns true is successful
        /// Returns fail is failed
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public Task<bool> LikeComment(Guid commentid, string userid);
    }
}