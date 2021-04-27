using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobalModels;
using Microsoft.Extensions.Logging;
using Repository;

namespace BusinessLogic
{
    /// <summary>
    /// Implements the interface IForumLogic.
    /// Methods are used to read and write objects in the repository.
    /// Return appropriate response to calling methods
    /// Comments found in IForumLogic
    /// </summary>
    public class ForumLogic : Interfaces.IForumLogic
    {
        private readonly IRepoLogic _repo;
        private readonly ILogger<ForumLogic> _logger;

        public ForumLogic(IRepoLogic repo)
        {
            _repo = repo;
        }

        public ForumLogic(IRepoLogic repo, ILogger<ForumLogic> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<bool> CreateComment(NewComment comment)
        {
            var repoComment = Mapper.NewCommentToNewRepoComment(comment);
            return await _repo.AddComment(repoComment);
        }

        public async Task<bool> CreateDiscussion(NewDiscussion discussion)
        {
            var repoDiscussion = Mapper.NewDiscussionToNewRepoDiscussion(discussion);
            Console.WriteLine(repoDiscussion.DiscussionId);
            var repoTopic = new Repository.Models.Topic();
            repoTopic.TopicName = discussion.Topic;
            return await _repo.AddDiscussion(repoDiscussion, repoTopic);
        }

        public async Task<List<Comment>> GetComments(Guid discussionid)
        {
            List<Repository.Models.Comment> repoComments = await _repo.GetMovieComments(discussionid.ToString());
            if (repoComments == null)
            {
                _logger.LogWarning($"ForumLogic.GetComments() was called with a discussionid that doesn't exist {discussionid}.");
                return null;
            }

            List<Comment> comments = new List<Comment>();
            foreach (var repoComment in repoComments)
            {
                comments.Add(Mapper.RepoCommentToComment(repoComment));
            }
            return comments;
        }

        public async Task<List<Comment>> GetCommentsPage(Guid discussionid, int page)
        {
            if (page < 1)
            {
                _logger.LogWarning($"ForumLogic.GetCommentsPage() was called with a negative or zero page number {page}.");
                return null;
            }

            Repository.Models.Setting pageSizeSetting = _repo.GetSetting("commentspagesize");
            int pageSize = 1;//pageSizeSetting.IntValue ?? default(int);
            if (pageSize < 1)
            {
                _logger.LogWarning($"ForumLogic.GetCommentsPage() was called but the commentspagesize is invalid {pageSize}");
                return null;
            }

            List<Repository.Models.Comment> repoComments = await _repo.GetMovieComments(discussionid.ToString());
            if (repoComments == null)
            {
                _logger.LogWarning($"ForumLogic.GetCommentsPage() was called with a discussionid that doesn't exist {discussionid}.");
                return null;
            }
            repoComments = repoComments.OrderByDescending(c => c.CreationTime).ToList<Repository.Models.Comment>();

            int numberOfComments = repoComments.Count;
            int startIndex = pageSize * (page - 1);

            if (startIndex > numberOfComments - 1)
            {
                _logger.LogWarning($"ForumLogic.GetCommentsPage() was called for a page number without comments {numberOfComments}.");
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

        public async Task<bool> SetCommentsPageSize(int pagesize)
        {
            if (pagesize < 1 || pagesize > 100)
            {
                _logger.LogWarning($"ForumLogic.SetCommentsPageSize() was called with an invalid pagesize {pagesize}.");
                return false;
            }

            Repository.Models.Setting setting = new Repository.Models.Setting();
            setting.Setting1 = "commentspagesize";
            setting.IntValue = pagesize;
            return await _repo.SetSetting(setting);
        }

        public async Task<List<Discussion>> GetDiscussions(string movieid)
        {
            List<Repository.Models.Discussion> repoDiscussions = await _repo.GetMovieDiscussions(movieid);
            if (repoDiscussions == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussions() was called with a movieid that doesn't exist {movieid}.");
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

        public async Task<Discussion> GetDiscussion(Guid discussionid)
        {
            Repository.Models.Discussion repoDiscussion = await _repo.GetDiscussion(discussionid.ToString());
            if (repoDiscussion == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussion() was called for an invalid discussionid {discussionid}.");
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

        public async Task<List<string>> GetTopics()
        {
            var repoTopics = await _repo.GetTopics();
            if (repoTopics == null)
            {
                _logger.LogWarning($"ForumLogic.GetTopics() was called but there are no topics.");
                return null;
            }

            var topics = new List<string>();
            foreach (var repoTopic in repoTopics)
            {
                topics.Add(repoTopic.TopicName);
            }
            return topics;
        }

        public async Task<List<DiscussionT>> GetSortedDiscussionsByComments(string type)
        {
            List<Repository.Models.Discussion> repoDiscussions = new List<Repository.Models.Discussion>();
            if (type == "a")
            {
                repoDiscussions = await Task.Run(() => _repo.GetSortedDiscussionsAscending());
            }
            else if (type == "d")
            {
                repoDiscussions = await Task.Run(() => _repo.GetSortedDiscussionsDescending());
            }

            else if (type == "r")
            {
                repoDiscussions = await Task.Run(() => _repo.GetSortedDiscussionsRecent());
            }

            List<DiscussionT> globalDiscussions = new List<DiscussionT>();

            foreach (Repository.Models.Discussion dis in repoDiscussions)
            {
                DiscussionT gdis = new DiscussionT();

                gdis.DiscussionId = dis.DiscussionId;
                gdis.MovieId = dis.MovieId;
                gdis.Userid = dis.UserId;
                gdis.Subject = dis.Subject;
                foreach (var ct in dis.Comments)
                {
                    Comment nc = new Comment(Guid.Parse(ct.CommentId), Guid.Parse(ct.DiscussionId), ct.UserId, ct.CommentText, ct.IsSpoiler);
                    gdis.Comments.Add(nc);

                }
                foreach (var top in dis.DiscussionTopics)
                {
                    gdis.DiscussionTopics.Add(top.TopicId);
                }
                globalDiscussions.Add(gdis);
            }
            return globalDiscussions;
        }
        public async Task<bool> CreateTopic(string topic)
        {
            Repository.Models.Topic newTopic = Mapper.NewTopicToRepoTopic(topic);
            return await _repo.AddTopic(newTopic);
        }

        public async Task<List<DiscussionT>> GetDiscussionsByTopicId(string topicid)
        {
            List<Repository.Models.Discussion> repoDiscussions = new List<Repository.Models.Discussion>();

            repoDiscussions = await Task.Run(() => _repo.GetDiscussionsByTopicId(topicid));



            List<DiscussionT> globalDiscussions = new List<DiscussionT>();
            DiscussionT gdis;
            foreach (Repository.Models.Discussion dis in repoDiscussions)
            {
                gdis = new();
                gdis.DiscussionId = dis.DiscussionId;
                gdis.MovieId = dis.MovieId;
                gdis.Userid = dis.UserId;
                gdis.Subject = dis.Subject;
                foreach (var ct in dis.Comments)
                {
                    Comment nc = new Comment(Guid.Parse(ct.CommentId), Guid.Parse(ct.DiscussionId), ct.UserId, ct.CommentText, ct.IsSpoiler);
                    gdis.Comments.Add(nc);

                }

                foreach (var top in dis.DiscussionTopics)
                {
                    gdis.DiscussionTopics.Add(top.TopicId);
                    if (top.TopicId.Equals(topicid))
                        globalDiscussions.Add(gdis);
                }
            }

            return globalDiscussions;

        }
    }
}