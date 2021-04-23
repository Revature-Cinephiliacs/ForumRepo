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
    public class RepoLogic
    {
        private readonly Cinephiliacs_ForumContext _dbContext;

        public RepoLogic(Cinephiliacs_ForumContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// To save the comment into database
        /// and Retun True is successfully save the comments 
        /// Retunrs false if username or discussion ID doesn't exist 
        /// </summary>
        /// <param name="repoComment"></param>
        /// <returns></returns>
        public async Task<bool> AddComment(Comment repoComment)
        {
            var userExists = UserExists(repoComment.Username);
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

        /// <summary>
        /// Saving Discussion into database
        /// Return ture if saved succeffully 
        /// Return false if user or movie doesn't exist  
        /// </summary>
        /// <param name="repoDiscussion"></param>
        /// <param name="repoTopic"></param>
        /// <returns></returns>
        public async Task<bool> AddDiscussion(Discussion repoDiscussion, Topic repoTopic)
        {
            var userExists = UserExists(repoDiscussion.Username);
            if (!userExists || repoDiscussion.Username == null)
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

            var topicExists = TopicExists(repoTopic.TopicName);
            if (topicExists)
            {
                await _dbContext.SaveChangesAsync();
                Discussion discussion;
                if ((discussion = _dbContext.Discussions.Where(d => d.MovieId == repoDiscussion.MovieId
                     && d.Username == repoDiscussion.Username && d.Subject == repoDiscussion.Subject)
                    .FirstOrDefault<Discussion>()) == null)
                {
                    return true;
                }
                await AddDiscussionTopic(discussion.DiscussionId, repoTopic.TopicName);
                return true;
            }
            else
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        /// <summary>
        /// Returns a list of all Comment objects from the database that match the discussion ID specified
        /// in the argument. Returns null if the discussion doesn't exist.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the value(s) of an existing setting in the database with a matching key string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Setting GetSetting(string key)
        {
            return _dbContext.Settings.Where(s => s.Setting1 == key).FirstOrDefault<Setting>();
        }

        /// <summary>
        /// Creates a new setting entry or updates the value(s) of an existing setting
        /// in the database.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a list of all Discussion objects from the database that match the movie ID specified
        /// in the argument. Returns null if the movie doesn't exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the Topic object from the database that matches the discussionId specified
        /// in the argument. Returns null if the discussionid doesn't exist or that discussion
        /// has no topic.
        /// </summary>
        /// <param name="discussionId"></param>
        /// <returns></returns>
        public Topic GetDiscussionTopic(string discussionId)
        {
            var discussionExists = DiscussionExists(discussionId);
            if (!discussionExists)
            {
                Console.WriteLine("RepoLogic.GetDiscussionTopic() was called for a discussion that doesn't exist.");
                return null;
            }
            return _dbContext.Topics.Where(t => t.TopicName == _dbContext.DiscussionTopics
                .Where(d => d.DiscussionId == discussionId).FirstOrDefault<DiscussionTopic>().TopicName)
                .FirstOrDefault<Topic>();
        }

        /// <summary>
        /// Returns the Discussion object that match the discussionid specified in the argument.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public async Task<Discussion> GetDiscussion(string discussionid)
        {
            return await _dbContext.Discussions.Where(d => d.DiscussionId == discussionid).FirstOrDefaultAsync<Discussion>();
        }

        /// <summary>
        /// Returns a list of all Topic objects in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Topic>> GetTopics()
        {
            return await _dbContext.Topics.ToListAsync();
        }


        /// <summary>
        /// Adds the DiscussionTopic defined by the discussion Id and topic name arguments
        /// to the database.
        /// Returns true iff successful.
        /// Returns false if the Discussion with the specified discussionId or the Topic with
        /// the specified topicName referenced do not already exist in their respective
        /// database tables.
        /// </summary>
        /// <param name="discussionId"></param>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public async Task<bool> AddDiscussionTopic(string discussionId, string topicName)
        {
            var discussionExists = DiscussionExists(discussionId);
            if (!discussionExists)
            {
                Console.WriteLine("RepoLogic.AddDiscussionTopic() was called for a discussion id that doesn't exist.");
                return false;
            }
            var topicExists = TopicExists(topicName);
            if (!topicExists)
            {
                Console.WriteLine("RepoLogic.AddDiscussionTopic() was called for a topic that doesn't exist.");
                return false;
            }
            var discussionTopic = new DiscussionTopic();
            discussionTopic.DiscussionId = discussionId;
            discussionTopic.TopicName = topicName;

            await _dbContext.DiscussionTopics.AddAsync(discussionTopic);

            await _dbContext.SaveChangesAsync();
            return true;
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
        private bool TopicExists(string topicName)
        {
            return (_dbContext.Topics.Where(t => t.TopicName == topicName).FirstOrDefault<Topic>() != null);
        }

        /// <summary>
        /// Returns true iff the movie ID, specified in the argument, exists in the database's Movies table.
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