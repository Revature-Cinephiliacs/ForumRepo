using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalModels;

namespace BusinessLogic.Interfaces
{
    public interface IForumLogic
    {
        /// <summary>
        /// Returns a list of every topic's name.
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> GetTopics();

        /// <summary>
        /// Returns a list of every Discussion object whose Movieid is equal to
        /// the movieid argument. Returns null if the movieid doesn't exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public Task<List<Discussion>> GetDiscussions(string movieid);

        /// <summary>
        /// Returns the Discussion object whose Discussion ID is equal to the
        /// discussionid argument.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        Task<Discussion> GetDiscussion(int discussionid);

        /// <summary>
        /// Returns a list of every Comment object whose Discussionid is equal to
        /// the discussionid argument. Returns null if the discussionid doesn't
        /// exist.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public Task<List<Comment>> GetComments(int discussionid);

        /// <summary>
        /// Returns Comments objects [n*(page-1), n*(page-1) + n] whose Discussionid
        /// is equal to the discussionid argument, where n is the current page size
        /// for comments. Returns null if the discussionid doesn't exist.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Task<List<Comment>> GetCommentsPage(int discussionid, int page);

        /// <summary>
        /// Sets the page size for comments.
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public Task<bool> SetCommentsPageSize(int pagesize);

        /// <summary>
        /// Adds a new Discussion Object to storage.
        /// Returns true if sucessful; false otherwise.
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        public Task<bool> CreateDiscussion(NewDiscussion discussion);

        /// <summary>
        /// Adds a new Comment Object to storage.
        /// Returns true if sucessful; false otherwise.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public Task<bool> CreateComment(NewComment comment);
    }
}