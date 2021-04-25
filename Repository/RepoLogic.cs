using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository
{
    /// <summary>
    /// Class Contains all the methods to perfrom CRUD operation for ForumAPL
    /// </summary>
    public class RepoLogic : IRepoLogic
    {
        private readonly Cinephiliacs_ForumContext _dbContext;

        public RepoLogic(Cinephiliacs_ForumContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddComment(Comment repoComment)
        {
            var userExists = UserExists(repoComment.UserId);
            if (!userExists)
            {
                Console.WriteLine("RepoLogic.AddComment() was called for a user that doesn't exist.");
                return false;
            }
            var discussionExists = DiscussionExists(repoComment.DiscussionId);
            if (!discussionExists)
            {
                Console.WriteLine("RepoLogic.AddComment() was called for a discussion that doesn't exist.");
                return false;
            }

            await _dbContext.Comments.AddAsync(repoComment);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddTopic(string topic){
            
            Topic newTopic = new Topic();
            newTopic.TopicId = Guid.NewGuid().ToString();
            newTopic.TopicName = topic;

            await _dbContext.Topics.AddAsync(newTopic);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddDiscussion(Discussion repoDiscussion, Topic repoTopic)
        {
            var userExists = UserExists(repoDiscussion.UserId);
            if (!userExists || repoDiscussion.UserId == null)
            {

                Console.WriteLine("RepoLogic.AddDiscussion() was called for a user that doesn't exist.");
                return false;
            }
            var movieExists = MovieExists(repoDiscussion.MovieId);
            if (!movieExists)
            {
                Console.WriteLine("RepoLogic.AddDiscussion() was called for a movie that doesn't exist.");
                return false;
            }

            await _dbContext.Discussions.AddAsync(repoDiscussion);

            var topicExists = TopicExists(repoTopic.TopicId);
            if (topicExists)
            {
                await _dbContext.SaveChangesAsync();
                Discussion discussion;
                if ((discussion = _dbContext.Discussions.Where(d => d.MovieId == repoDiscussion.MovieId
                     && d.UserId == repoDiscussion.UserId && d.Subject == repoDiscussion.Subject)
                    .FirstOrDefault<Discussion>()) == null)
                {
                    return true;
                }
                await AddDiscussionTopic(discussion.DiscussionId, repoTopic.TopicId);
                return true;
            }
            else
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<Comment>> GetMovieComments(string discussionid)
        {
            var discussionExists = DiscussionExists(discussionid);
            if (!discussionExists)
            {
                Console.WriteLine("RepoLogic.GetMovieComments() was called for a discussion that doesn't exist.");
                return null;
            }
            var commentList = await _dbContext.Comments.Where(c => c.DiscussionId == discussionid).ToListAsync();
            return commentList;
        }

        public Setting GetSetting(string key)
        {
            return _dbContext.Settings.Where(s => s.Setting1 == key).FirstOrDefault<Setting>();
        }

        public async Task<bool> SetSetting(Setting setting)
        {
            if (setting == null || setting.Setting1.Length < 1)
            {
                Console.WriteLine("RepoLogic.SetSetting() was called with a null or invalid setting.");
                return false;
            }
            if (SettingExists(setting.Setting1))
            {
                Setting existentSetting = await _dbContext.Settings.Where(
                    s => s.Setting1 == setting.Setting1).FirstOrDefaultAsync<Setting>();
                if (setting.IntValue != null)
                {
                    existentSetting.IntValue = setting.IntValue;
                }
                if (setting.StringValue != null)
                {
                    existentSetting.StringValue = setting.StringValue;
                }
            }
            else
            {
                await _dbContext.Settings.AddAsync(setting);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Discussion>> GetMovieDiscussions(string movieid)
        {
            var movieExists = MovieExists(movieid);
            if (!movieExists)
            {
                Console.WriteLine("RepoLogic.GetMovieDiscussions() was called for a movie that doesn't exist.");
                return null;
            }
            return await _dbContext.Discussions.Where(d => d.MovieId == movieid).ToListAsync();
        }

        public Topic GetDiscussionTopic(string discussionId)
        {
            var discussionExists = DiscussionExists(discussionId);
            if (!discussionExists)
            {
                Console.WriteLine("RepoLogic.GetDiscussionTopic() was called for a discussion that doesn't exist.");
                return null;
            }
            return _dbContext.Topics.Where(t => t.TopicId == _dbContext.DiscussionTopics
                .Where(d => d.DiscussionId == discussionId).FirstOrDefault<DiscussionTopic>().TopicId)
                .FirstOrDefault<Topic>();
        }

        public async Task<Discussion> GetDiscussion(string discussionid)
        {
            return await _dbContext.Discussions.Where(d => d.DiscussionId == discussionid).FirstOrDefaultAsync<Discussion>();
        }

        public async Task<List<Topic>> GetTopics()
        {
            return await _dbContext.Topics.ToListAsync();
        }

        public async Task<bool> AddDiscussionTopic(string discussionId, string topicid)
        {
            var discussionExists = DiscussionExists(discussionId);
            if (!discussionExists)
            {
                Console.WriteLine("RepoLogic.AddDiscussionTopic() was called for a discussion id that doesn't exist.");
                return false;
            }
            var topicExists = TopicExists(topicid);
            if (!topicExists)
            {
                Console.WriteLine("RepoLogic.AddDiscussionTopic() was called for a topic that doesn't exist.");
                return false;
            }
            var discussionTopic = new DiscussionTopic();
            discussionTopic.DiscussionId = discussionId;
            discussionTopic.TopicId = topicid;

            await _dbContext.DiscussionTopics.AddAsync(discussionTopic);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Discussion>> GetSortedDiscussionsDescending()
        {
            return await _dbContext.Discussions.Include(d => d.Comments).OrderByDescending(x => x.Comments.Count).ToListAsync<Discussion>();
        }
        public async Task<List<Discussion>> GetSortedDiscussionsAscending()
        {
            return await _dbContext.Discussions.Include(d => d.Comments).OrderBy(x => x.Comments.Count).ToListAsync<Discussion>();
        }

        /// <summary>
        /// Returns true iff the setting key, specified in the argument, exists in the database's Settings table.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool SettingExists(string key)
        {
            return (_dbContext.Settings.Where(s => s.Setting1 == key).FirstOrDefault<Setting>() != null);
        }

        /// Returns true iff the username, specified in the argument, exists in the database's Users table.
        /// ---------------
        /// Placeholder until services set up
        /// ---------------
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private bool UserExists(string username)
        {
            //return (_dbContext.Users.Where(u => u.Username == username).FirstOrDefault<User>() != null);
            return true;
        }

        /// <summary>
        /// Returns true iff the discussion ID, specified in the argument, exists in the database's Discussions table.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        private bool DiscussionExists(string discussionid)
        {
            return (_dbContext.Discussions.Where(d => d.DiscussionId == discussionid).FirstOrDefault<Discussion>() != null);
        }

        /// <summary>
        /// Returns true iff the Topic name, specified in the argument, exists in the database's Topics table.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        private bool TopicExists(string topicid)
        {
            return (_dbContext.Topics.Where(t => t.TopicId == topicid).FirstOrDefault<Topic>() != null);
        }

        /// <summary>
        /// Returns true iff the movie ID, specified in the argument, exists in the database's Movies table.
        /// ---------------
        /// Placeholder until services set up
        /// ---------------
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        private bool MovieExists(string movieid)
        {
            //return (_dbContext.Movies.Where(m => m.MovieId == movieid).FirstOrDefault<Movie>() != null);
            return true;
        }
    }
}