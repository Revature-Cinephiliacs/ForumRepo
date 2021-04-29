using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Models;

namespace Repository
{
    /// <summary>
    /// Class Contains all the methods to perfrom CRUD operation for ForumAPL
    /// </summary>
    public class RepoLogic : IRepoLogic
    {
        private readonly Cinephiliacs_ForumContext _dbContext;
        private readonly ILogger<RepoLogic> _logger;

        public RepoLogic(Cinephiliacs_ForumContext dbContext)
        {
            _dbContext = dbContext;
        }

        public RepoLogic(Cinephiliacs_ForumContext dbContext, ILogger<RepoLogic> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> AddComment(Comment repoComment)
        {
            var userExists = UserExists(repoComment.UserId);
            if (!userExists)
            {
                _logger.LogWarning($"RepoLogic.AddComment() was called for a user that doesn't exist {repoComment.UserId}.");
                return false;
            }
            var discussionExists = DiscussionExists(repoComment.DiscussionId);
            if (!discussionExists)
            {
                _logger.LogWarning($"RepoLogic.AddComment() was called for a discussion that doesn't exist {repoComment.DiscussionId}.");
                return false;
            }

            await _dbContext.Comments.AddAsync(repoComment);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddTopic(Topic topic)
        {
            Topic getTopic = await _dbContext.Topics.FirstOrDefaultAsync(x => x.TopicName == topic.TopicName);
            if(getTopic != null)
            {
                return false;
            }
            await _dbContext.Topics.AddAsync(topic);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddDiscussion(Discussion repoDiscussion, Topic repoTopic)
        {
            var userExists = UserExists(repoDiscussion.UserId);
            if (!userExists || repoDiscussion.UserId == null)
            {

                _logger.LogWarning($"RepoLogic.AddDiscussion() was called for a user that doesn't exist {repoDiscussion.UserId}.");
                return false;
            }
            var movieExists = MovieExists(repoDiscussion.MovieId);
            if (!movieExists)
            {
                _logger.LogWarning($"RepoLogic.AddDiscussion() was called for a movie that doesn't exist {repoDiscussion.MovieId}.");
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

        public async Task<bool> DeleteComment(string commentid)
        {
            Comment delComment = await _dbContext.Comments.FirstOrDefaultAsync(x => x.CommentId == commentid);
            if(delComment == null)
            {
                _logger.LogWarning($"RepoLogic.DeleteComment() was called for a comment that doesn't exist {commentid}.");
                return false;
            }

            delComment.CommentText = "removed";
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteDiscussion(string discussionid)
        {
            Discussion delDisc = await _dbContext.Discussions.FirstOrDefaultAsync(x => x.DiscussionId == discussionid);
            if(delDisc == null)
            {
                _logger.LogWarning($"RepoLogic.DeleteDiscussion() was called for a comment that doesn't exist {discussionid}.");
                return false;
            }
            List<DiscussionTopic> discTopics = await _dbContext.DiscussionTopics.Where(x => x.DiscussionId == discussionid).ToListAsync();
            _dbContext.DiscussionTopics.RemoveRange(discTopics);
            await _dbContext.SaveChangesAsync();

            List<Comment> discComments = await _dbContext.Comments.Where(x => x.DiscussionId == discussionid).ToListAsync();
            _dbContext.Comments.RemoveRange(discComments);
            await _dbContext.SaveChangesAsync();

            List<DiscussionFollow> discFollows = await _dbContext.DiscussionFollows.Where(x => x.DiscussionId == discussionid).ToListAsync();
            _dbContext.DiscussionFollows.RemoveRange(discFollows);
            await _dbContext.SaveChangesAsync();

            _dbContext.Discussions.Remove(delDisc);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTopic(string topicid)
        {
            Topic delTopic = await _dbContext.Topics.FirstOrDefaultAsync(x => x.TopicId == topicid);
            if(delTopic == null)
            {
                _logger.LogWarning($"RepoLogic.DeleteTopic() was called for a comment that doesn't exist {topicid}.");
                return false;
            }
            List<DiscussionTopic> topicsRef = await _dbContext.DiscussionTopics.Where(x => x.TopicId == topicid).ToListAsync();
            _dbContext.DiscussionTopics.RemoveRange(topicsRef);
            await _dbContext.SaveChangesAsync();

            _dbContext.Topics.Remove(delTopic);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FollowDiscussion(DiscussionFollow newFollow)
        {
            if(!DiscussionExists(newFollow.DiscussionId))
            {
                _logger.LogWarning($"RepoLogic.FollowDiscussion() was called for a comment that doesn't exist {newFollow.DiscussionId}.");
                return false;
            }

            await _dbContext.AddAsync<DiscussionFollow>(newFollow);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<DiscussionFollow>> GetFollowDiscussionList(string userid)
        {
            return await _dbContext.DiscussionFollows.Include(x => x.Discussion).Where(x => x.UserId == userid).ToListAsync();
        }

        public async Task<List<Comment>> GetMovieComments(string discussionid)
        {
            var discussionExists = DiscussionExists(discussionid);
            if (!discussionExists)
            {
                _logger.LogWarning($"RepoLogic.GetMovieComments() was called for a discussion that doesn't exist {discussionid}.");
                return null;
            }
            var commentList = await _dbContext.Comments.Where(c => c.DiscussionId == discussionid).OrderByDescending(c => c.CreationTime).ToListAsync();
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
                _logger.LogWarning($"RepoLogic.SetSetting() was called with a null or invalid setting {setting.Setting1}.");
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
                _logger.LogWarning($"RepoLogic.GetMovieDiscussions() was called for a movie that doesn't exist {movieid}.");
                return null;
            }
            return await _dbContext.Discussions.Where(d => d.MovieId == movieid).ToListAsync();
        }

        public Topic GetDiscussionTopic(string discussionId)
        {
            var discussionExists = DiscussionExists(discussionId);
            if (!discussionExists)
            {
                _logger.LogWarning($"RepoLogic.GetDiscussionTopic() was called for a discussion that doesn't exist {discussionId}.");
                return null;
            }
            return _dbContext.Topics.Where(t => t.TopicId == _dbContext.DiscussionTopics
                .Where(d => d.DiscussionId == discussionId).FirstOrDefault<DiscussionTopic>().TopicId)
                .FirstOrDefault<Topic>();
        }

        public async Task<Discussion> GetDiscussion(string discussionid)
        {
            return await _dbContext.Discussions.Where(d => d.DiscussionId == discussionid).Include(dis => dis.DiscussionTopics).FirstOrDefaultAsync<Discussion>();
            //return await _dbContext.Discussions.Where(d => d.DiscussionId == discussionid).FirstOrDefaultAsync<Discussion>();
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
                _logger.LogWarning($"RepoLogic.AddDiscussionTopic() was called for a discussion id that doesn't exist {discussionId}.");
                return false;
            }
            var topicExists = TopicExists(topicid);
            if (!topicExists)
            {
                _logger.LogWarning($"RepoLogic.AddDiscussionTopic() was called for a topic that doesn't exist {topicid}.");
                return false;
            }
            var discussionTopic = new DiscussionTopic();
            discussionTopic.DiscussionId = discussionId;
            discussionTopic.TopicId = topicid;

            await _dbContext.DiscussionTopics.AddAsync(discussionTopic);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeCommentSpoiler(string commentid)
        {
            Comment getComment = await _dbContext.Comments.FirstOrDefaultAsync(x => x.CommentId == commentid);
            if(getComment == null)
            {
                _logger.LogWarning($"RepoLogic.ChangeCommentSpoiler() was called for a comment that doesn't exist {commentid}.");
                return false;
            }
            if(getComment.IsSpoiler)
            {
                getComment.IsSpoiler = false;
            }
            else
            {
                getComment.IsSpoiler = true;
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikeComment(UserLike newLike)
        {
            Comment getComment = await _dbContext.Comments.FirstOrDefaultAsync(x => x.CommentId == newLike.CommentId);
            if(getComment == null)
            {
                _logger.LogWarning($"RepoLogic.LikeComment() was called for a comment that doesn't exist {newLike.CommentId}.");
                return false;
            }
            Discussion getDiscussion = await _dbContext.Discussions.FirstOrDefaultAsync(x => x.DiscussionId == getComment.DiscussionId);
            if(getDiscussion == null)
            {
                _logger.LogWarning($"RepoLogic.LikeComment() was called for a comment that doesn't exist {getComment.DiscussionId}.");
                return false;
            }
            UserLike getLikes = await _dbContext.UserLikes.Where(x => x.UserId == newLike.UserId).FirstOrDefaultAsync(x => x.CommentId == newLike.CommentId);
            if(getLikes != null)
            {
                _logger.LogWarning($"RepoLogic.LikeComment(), but {newLike.UserId} already liked {newLike.CommentId}.");
                return false;
            }
            await _dbContext.AddAsync<UserLike>(newLike);
            getComment.Likes++;
            getDiscussion.Totalikes++;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Topic> GetTopicById(string topicid)
        {
            return await _dbContext.Topics.FirstOrDefaultAsync(x => x.TopicId == topicid);
        }

        public async Task<Discussion> GetDiscussionsById(string discid)
        {
            return await _dbContext.Discussions.FirstOrDefaultAsync(x => x.DiscussionId == discid);
        }

        public async Task<Comment> GetCommentById(string commentid)
        {
            return await _dbContext.Comments.FirstOrDefaultAsync(x => x.CommentId == commentid);
        }

        public async Task<List<Comment>> GetCommentReportList(List<string> idList)
        {
            return await _dbContext.Comments.Where(u => idList.Contains(u.CommentId)).ToListAsync();
        }

        public async Task<List<Discussion>> GetDiscussionReportList(List<string> idList)
        {
            return await _dbContext.Discussions.Where(u => idList.Contains(u.DiscussionId)).ToListAsync();
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
        /// Returns true iff the comment id, specified in the argument, exists in the database's Comments table.
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        private bool CommentExists(string commentid)
        {
            return (_dbContext.Comments.Where(d => d.CommentId == commentid).FirstOrDefault<Comment>() != null);
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

        public async Task<List<Discussion>> GetSortedDiscussionsRecent()
        {
            return await _dbContext.Discussions.Include(d => d.Comments).OrderBy(x => x.CreationTime).ToListAsync<Discussion>();
        }

        public async Task<List<DiscussionTopic>> GetDiscussionsByTopicId(string topicid)
        {
            return await _dbContext.DiscussionTopics.Include(dis => dis.Discussion).Where(x => x.TopicId == topicid).ToListAsync<DiscussionTopic>();
        }
    }
}