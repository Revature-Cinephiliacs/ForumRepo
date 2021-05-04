using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
            var userExist = Mapper.GetUsernameFromAPI(comment.Userid);
            if(userExist == null)
            {
                _logger.LogWarning($"ForumLogic.CreateDiscussion() was called with a userid that does not exist");
                return false;
            }

            var repoComment = Mapper.NewCommentToNewRepoComment(comment);
            string commentId = await _repo.AddComment(repoComment);
            var listDfollow = _repo.GetFollowDiscussionListByDiscussionId(repoComment.DiscussionId).Result;
            List<string> followers = new List<string>();
            foreach(var follower in listDfollow)
            {
                followers.Add(follower.UserId);
            }
            if (commentId != null)
            {
                await Mapper.SendCommentNotification(repoComment, followers);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CreateDiscussion(NewDiscussion discussion)
        {
            var userExist = Mapper.GetUsernameFromAPI(discussion.Userid);
            if(userExist == null)
            {
                _logger.LogWarning($"ForumLogic.CreateDiscussion() was called with a userid that does not exist");
                return false;
            }

            var repoDiscussion = Mapper.NewDiscussionToNewRepoDiscussion(discussion);
            var repoTopic = await _repo.GetTopicById(discussion.Topic);
            var discussionId = await _repo.AddDiscussion(repoDiscussion, repoTopic);

            if (discussionId != null)
            {
                await Mapper.SendDiscussionNotification(repoDiscussion, discussionId);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<GlobalModels.Comment>> GetComments(Guid discussionid)
        {
            List<Repository.Models.Comment> repoComments = await _repo.GetMovieComments(discussionid.ToString());
            if (repoComments == null)
            {
                _logger.LogWarning($"ForumLogic.GetComments() was called with a discussionid that doesn't exist {discussionid}.");
                return null;
            }

            List<Comment> comments = new List<Comment>();
            List<Task<Comment>> tasks = new List<Task<Comment>>();
            foreach (var repoComment in repoComments)
            {
                tasks.Add(Task.Run(() => Mapper.RepoCommentToComment(repoComment)));
            }
            var results = await Task.WhenAll(tasks);
            foreach (var item in results)
            {
                comments.Add(item);
            }
            return comments;
        }

        public async Task<List<NestedComment>> GetCommentsPage(Guid discussionid, int page, string sortingOrder)
        {
           if (page < 1)
            {
                _logger.LogWarning($"ForumLogic.GetCommentsPage() was called with a negative or zero page number {page}.");
                return null;
            }

            Repository.Models.Setting pageSizeSetting = _repo.GetSetting("commentspagesize");
            int pageSize = pageSizeSetting.IntValue ?? default(int);
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

            List<NestedComment> tempComments = new List<NestedComment>();
            List<Task<NestedComment>> tasksNC = new List<Task<NestedComment>>();
            foreach (Repository.Models.Comment rc in repoComments)
            {
                tasksNC.Add(Task.Run(() => Mapper.RepoCommentToNestedComment(rc)));
            }
            var results = await Task.WhenAll(tasksNC);
            foreach (var item in results)
            {
                tempComments.Add(item);
            }

            //Create array of comments to store parent comments
            List<NestedComment> parentComments = new List<NestedComment>();
            foreach (NestedComment tc in tempComments)
            {
                if (tc.ParentCommentid == null)
                {
                    parentComments.Add(tc);
                }
            }

            // Sort the list of comments
            switch (sortingOrder)
            {
                case "likesA":
                    parentComments = SortByLikesAsc(parentComments);
                break;
                case "likesD":
                    parentComments = SortByLikesDes(parentComments);
                break;
                case "timeA":
                    parentComments = sortByTimeCreationA(parentComments);
                    break;
                case "timeD":
                    parentComments = sortByTimeCreationD(parentComments);
                    break;
            }

            //Change to number of parent comments
            int numberOfComments = parentComments.Count;

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

            List<NestedComment> comments = new List<NestedComment>();
            
            //if the sorting order was by num of nested comments 
            if(sortingOrder.Equals("comments"))
            {
                List<NestedComment> commentsTemp = new List<NestedComment>();

                foreach (var item in parentComments)
                {
                    commentsTemp.Add(item);
                }

                 //call a recursive function to send repoComments and add the children to the comments in page comments
                foreach (NestedComment nc in commentsTemp)
                {
                    Mapper.AddReplies(tempComments, nc);
                }

                commentsTemp = sortByComments(commentsTemp);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    comments.Add(commentsTemp[i]);
                }

            }else{
                for (int i = startIndex; i <= endIndex; i++)
                {
                    comments.Add(parentComments[i]);
                }

                //call a recursive function to send repoComments and add the children to the comments in page comments
                foreach (NestedComment nc in comments)
                {
                    Mapper.AddReplies(tempComments, nc);
                }
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

        public async Task<List<DiscussionT>> GetDiscussions(string movieid)
        {
            var movieExist = Mapper.GetMovieFromAPI(movieid);

            if(movieExist == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussions() was called with a movie id that does not exist {movieid}");
                return null;
            }
            List<Repository.Models.Discussion> repoDiscussions = await _repo.GetMovieDiscussions(movieid);
            if (repoDiscussions == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussions() was called with a movieid that doesn't exist {movieid}.");
                return null;
            }

            foreach (var item in repoDiscussions)
            {
                item.Comments = await _repo.GetMovieComments(item.DiscussionId);
            }

            List<DiscussionT> discussions = new List<DiscussionT>();
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach (var repoDiscussion in repoDiscussions)
            {
                // Get the topic associated with this discussion
                Repository.Models.Topic topic = _repo.GetDiscussionTopic(repoDiscussion.DiscussionId);
                if (topic == null)
                {
                    topic = new Repository.Models.Topic();
                    topic.TopicName = "None";
                }
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(repoDiscussion)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var items in results)
            {
                discussions.Add(items);
            }
            return discussions;
        }

        public async Task<List<DiscussionT>> GetDiscussionsPage(string movieid, int page, string sortingOrder)
        {
            var movieExist = Mapper.GetMovieFromAPI(movieid);

            if(movieExist == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussions() was called with a movie id that does not exist {movieid}");
                return null;
            }
            
            if(page < 1)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussionsPage() was called with a negative or zero page number {page}.");
                return null;
            }

            Repository.Models.Setting pageSizeSetting = _repo.GetSetting("Discussionpagesize");
            
            int pageSize = pageSizeSetting.IntValue ?? default(int);
            
            if(pageSize < 1)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussionsPage() was called with a negative or zero page number.");
                return null;
            }

            List<Repository.Models.Discussion> repoDiscussions = await _repo.GetMovieDiscussions(movieid);
            if (repoDiscussions == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussions() was called with a movieid that doesn't exist {movieid}.");
                return null;
            }

            foreach (var item in repoDiscussions)
            {
                item.Comments = await _repo.GetMovieComments(item.DiscussionId);
            }

            // Sort the list of Discussion according to sorting string
            switch (sortingOrder)
            {
                case "likeD":
                    repoDiscussions = sortByNumOfLikes(repoDiscussions);
                break;
                case "likeA":
                    repoDiscussions = sortByNumOfLikesAsc(repoDiscussions);
                break;
                case "commentsA":
                    repoDiscussions = sortByNumOfCommentsAsce(repoDiscussions);
                break;
                case "commentsD":
                    repoDiscussions = sortByNumOfCommentsDesc(repoDiscussions);
                break;
                case "timeA":
                    repoDiscussions = sortByCreationTimeAsce(repoDiscussions);
                break;
                case "timeD":
                    repoDiscussions = sortByCreationTimeDesc(repoDiscussions);
                break;
                case "recentD":
                    repoDiscussions = sortByRecentDesc(repoDiscussions);
                break;
                case "recentA":
                    repoDiscussions = sortByRecentAsc(repoDiscussions);
                break;
            }

            int numOfDiscussion = repoDiscussions.Count;
            int start = pageSize * (page -1);
            if(start > numOfDiscussion - 1)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussionsPage() was called for a page number that does not exist.");
                return null;
            }

            int end = start + pageSize-1;
            if(end > numOfDiscussion - 1)
            {
                end = numOfDiscussion - 1;
            }

            List<Repository.Models.Discussion> pageDiscussions = new List<Repository.Models.Discussion>();
            for(int i = start; i <= end; i++){

                pageDiscussions.Add(repoDiscussions[i]);
            }

            List<DiscussionT> discussions = new List<DiscussionT>();
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach (var repoDiscussion in pageDiscussions)
            {
                
                // Get the topic associated with this discussion
                Repository.Models.Topic topic = _repo.GetDiscussionTopic(repoDiscussion.DiscussionId);
                if (topic == null)
                {
                    topic = new Repository.Models.Topic();
                    topic.TopicName = "None";
                }
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(repoDiscussion)));
                
            }
            var results = await Task.WhenAll(tasks);
            foreach(var items in results)
            {
                discussions.Add(items);
            }
            return discussions;
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
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach (Repository.Models.Discussion dis in repoDiscussions)
            {
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(dis)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var items in results)
            {
                globalDiscussions.Add(items);
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
            List<Repository.Models.DiscussionTopic> repoDiscussionTopics = new List<Repository.Models.DiscussionTopic>();

            repoDiscussionTopics = await Task.Run(() => _repo.GetDiscussionsByTopicId(topicid));

            List<Repository.Models.Discussion> globalDiscussions = new List<Repository.Models.Discussion>();

            foreach(Repository.Models.DiscussionTopic dt in repoDiscussionTopics)
            {
                globalDiscussions.Add(dt.Discussion);
            }

            List<DiscussionT> newDiscussions = new List<DiscussionT>();
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach (Repository.Models.Discussion dis in globalDiscussions)
            {
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(dis)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var items in results)
            {
                newDiscussions.Add(items);
            }
            return newDiscussions;
        }

        public async Task<bool> ChangeSpoiler(Guid commentid)
        {
            return await _repo.ChangeCommentSpoiler(commentid.ToString());
        }

        public async Task<bool> DeleteComment(Guid commentid)
        {
            return await _repo.DeleteComment(commentid.ToString());
        }

        public async Task<bool> DeleteDiscussion(Guid discussionid)
        {
            return await _repo.DeleteDiscussion(discussionid.ToString());
        }

        public async Task<bool> DeleteTopic(Guid topicid)
        {
            return await _repo.DeleteTopic(topicid.ToString());
        }

        public async Task<bool> FollowDiscussion(Guid discussionid, string userid)
        {
            Repository.Models.DiscussionFollow newFollow = new Repository.Models.DiscussionFollow();
            newFollow.DiscussionId = discussionid.ToString();
            newFollow.UserId = userid;
            return await _repo.FollowDiscussion(newFollow);
        }

        public async Task<List<DiscussionT>> GetFollowDiscList(string userid)
        {
            List<Repository.Models.Discussion> repoFollow = await _repo.GetFollowDiscussionList(userid);
            if(repoFollow == null)
            {
                return null;
            }
            List<DiscussionT> allDisc = new List<DiscussionT>();
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach(Repository.Models.Discussion disc in repoFollow)
            {
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(disc)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var item in results)
            {
                allDisc.Add(item);
            }
            return allDisc;
        }

        public async Task<bool> LikeComment(Guid commentid, string userid)
        {
            Repository.Models.UserLike newLike = new Repository.Models.UserLike();
            newLike.CommentId = commentid.ToString();
            newLike.UserId = userid;
            return await _repo.LikeComment(newLike);
        }

        public async Task<List<GlobalModels.Comment>> GetCommentReports(List<string> idList)
        {
            List<Repository.Models.Comment> repoComments = await _repo.GetCommentReportList(idList);
            List<Comment> listComments = new List<Comment>();
            List<Task<Comment>> tasks = new List<Task<Comment>>();
            foreach(Repository.Models.Comment item in repoComments)
            {
                tasks.Add(Task.Run(() => Mapper.RepoCommentToComment(item)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var item in results)
            {
                listComments.Add(item);
            }
            return listComments;
        }

        public async Task<List<DiscussionT>> GetDiscucssionReports(List<string> idList)
        {
            List<Repository.Models.Discussion> repoDisc = await _repo.GetDiscussionReportList(idList);
            List<DiscussionT> listDisc = new List<DiscussionT>();
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach(Repository.Models.Discussion item in repoDisc)
            {
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(item)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var item in results)
            {
                listDisc.Add(item);
            }
            return listDisc;
        }

        public async Task<GlobalModels.Topic> GetTopicById(Guid topicid)
        {
            Repository.Models.Topic topic = await _repo.GetTopicById(topicid.ToString());
            if(topic == null)
            {
                return null;
            }
            return Mapper.RepoTopicToTopic(topic);
        }

        public async Task<DiscussionT> GetDiscussionById(Guid discId)
        {
            Repository.Models.Discussion repoDisc = await _repo.GetDiscussionsById(discId.ToString());
            if(repoDisc == null)
            {
                return null;
            }
            return await Mapper.RepoDiscussionToDiscussionT(repoDisc);
        }

        public async Task<GlobalModels.Comment> GetCommentById(Guid commentid)
        {
            Repository.Models.Comment repoComment = await _repo.GetCommentById(commentid.ToString());
            if(repoComment == null)
            {
                return null;
            }
            return await Mapper.RepoCommentToComment(repoComment);
        }

        public async Task<bool> AddDiscussionTopic(string discussionid, string topicid)
        {
            return await _repo.AddDiscussionTopic(discussionid, topicid);
        }

        public async Task<DiscussionT> GetDiscussion(Guid discussionid)
        {
            Repository.Models.Discussion repoDiscussion = await _repo.GetDiscussion(discussionid.ToString());
            if (repoDiscussion == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussion() was called for an invalid discussionid {discussionid}.");
                return null;
            }

            DiscussionT discussion = await Task.Run(() => Mapper.RepoDiscussionToDiscussionT(repoDiscussion));

            return discussion;
        }

        public async Task<List<GlobalModels.Topic>> GetTopics()
        {
            var repoTopics = await _repo.GetTopics();
            if (repoTopics == null)
            {
                _logger.LogWarning($"ForumLogic.GetTopics() was called but there are no topics.");
                return null;
            }

            var topics = new List<GlobalModels.Topic>();
            foreach (var repoTopic in repoTopics)
            {
                var newTopic = new GlobalModels.Topic(repoTopic.TopicId, repoTopic.TopicName);
                topics.Add(newTopic);
            }
            return topics;
        }

         public async Task<List<DiscussionT>> GetDiscussionByUserId(string userId){

            List<Repository.Models.Discussion> discussions = await _repo.GetDiscussionsByUserId(userId);
            List<DiscussionT> newDiscussionList = new List<DiscussionT>();
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach (var item in discussions)
            {
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(item)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var item in results)
            {
                newDiscussionList.Add(item);
            }
            return newDiscussionList;
        }

        public async Task<List<GlobalModels.Comment>> GetCommentsByUserId(string userId){
            List<Repository.Models.Comment> comments = await _repo.GetCommentByUserId(userId);
            List<Comment> newComments = new List<Comment>();
            List<Task<Comment>> tasks = new List<Task<Comment>>();
            foreach (var item in comments)
            {
                tasks.Add(Task.Run(() => Mapper.RepoCommentToComment(item)));
            }
            var results = await Task.WhenAll(tasks);
            foreach (var item in results)
            {
                newComments.Add(item);
            }
            return newComments;
        }

        /// <summary>
        /// Sorts comments by number of likes (descending)
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<NestedComment> SortByLikesDes(List<NestedComment> comments)
        {
            return comments.OrderByDescending(r => r.Likes).ToList<NestedComment>();
        }

        /// <summary>
        /// Sort comments by number of likes (ascending)
        /// </summary>
        /// <param name="li"></param>
        /// <returns></returns>
        private List<NestedComment> SortByLikesAsc(List<NestedComment> li)
        {
            Comparison<NestedComment> likes = new Comparison<NestedComment>(NestedComment.CompareLikes);
            li.Sort(likes);
            return li;
        }

        /// <summary>
        /// Sorts comments by number of nested comments
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<NestedComment> sortByComments(List<NestedComment> comments)
        {
            return comments.OrderByDescending(r => r.Replies.Count).ToList<NestedComment>();
        }

        /// <summary>
        /// Sorts comments by creation time (ascending)
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<NestedComment> sortByTimeCreationA(List<NestedComment> comments)
        {
            return comments.OrderBy(r => r.CreationTime).ToList<NestedComment>();
        }

        /// <summary>
        /// Sorts comments by creation time (descending)
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<NestedComment> sortByTimeCreationD(List<NestedComment> comments)
        {
            return comments.OrderByDescending(r => r.CreationTime).ToList<NestedComment>();
        }

        /// <summary>
        /// Sorts discussions by number of total likes (descending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByNumOfLikes( List<Repository.Models.Discussion> discussions)
        {
            var newDiscussions = discussions.OrderByDescending(d => d.Totalikes).ToList<Repository.Models.Discussion>();
            return newDiscussions;
         }

        /// <summary>
        /// Sorts discussions by number of total likes (ascending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
         private List<Repository.Models.Discussion> sortByNumOfLikesAsc( List<Repository.Models.Discussion> discussions)
         {            
            var newDiscussions = discussions.OrderBy(d => d.Totalikes).ToList<Repository.Models.Discussion>();
            return newDiscussions;
         }

        /// <summary>
        /// Sorts discussions by number of comments (ascending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByNumOfCommentsAsce( List<Repository.Models.Discussion> discussions)
        {
            return discussions.OrderBy(r => r.Comments.Count).ToList<Repository.Models.Discussion>();
        }

        /// <summary>
        /// Sorts discussions by number of comments (descending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByNumOfCommentsDesc( List<Repository.Models.Discussion> discussions)
        {
            return discussions.OrderByDescending(r => r.Comments.Count).ToList<Repository.Models.Discussion>();
        }

        /// <summary>
        /// Sorts disucssions by creation time (ascending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByCreationTimeAsce( List<Repository.Models.Discussion> discussions)
        {
            return discussions.OrderBy(r => r.CreationTime).ToList<Repository.Models.Discussion>();
        }

        /// <summary>
        /// Sorts disucssions by creation time (descending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByCreationTimeDesc( List<Repository.Models.Discussion> discussions)
        {
            return discussions.OrderByDescending(r => r.CreationTime).ToList<Repository.Models.Discussion>();
        }

        /// <summary>
        /// Sorts the discussion list by recent activity (descending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByRecentDesc( List<Repository.Models.Discussion> discussions)
        {
            List<Repository.Models.Discussion> DiscussionwithComments = new List<Repository.Models.Discussion>();
            List<Repository.Models.Discussion> DiscussionWithNoComments = new List<Repository.Models.Discussion>();
            HashSet<Repository.Models.Discussion> tempDiscussions = new HashSet<Repository.Models.Discussion>();
            foreach (var item in discussions)
            {
                if(item.Comments.Count != 0)
                {
                    DiscussionwithComments.Add(item);
                }
                else
                {
                    DiscussionWithNoComments.Add(item);
                }
            }
            DiscussionwithComments = DiscussionwithComments.OrderByDescending(r => r.Comments.First().CreationTime).ToList<Repository.Models.Discussion>();
            DiscussionWithNoComments = DiscussionWithNoComments.OrderByDescending(r => r.CreationTime).ToList<Repository.Models.Discussion>();

            foreach(var item in DiscussionWithNoComments)
            {
               foreach (var d in DiscussionwithComments)
               { 
                    if(DateTime.Compare(item.CreationTime, d.Comments.First().CreationTime) < 0)
                    {
                        tempDiscussions.Add(d);
                    } 
                }
                tempDiscussions.Add(item);
            }  

            discussions.Clear();
            foreach (var item in tempDiscussions)
            {   
                discussions = tempDiscussions.ToList();
            }
            return discussions;
        }
        
        /// <summary>
        /// Sorts the discussion list by recent activity (ascending)
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByRecentAsc( List<Repository.Models.Discussion> discussions)
        {
            List<Repository.Models.Discussion> DiscussionwithComments = new List<Repository.Models.Discussion>();
            List<Repository.Models.Discussion> DiscussionWithNoComments = new List<Repository.Models.Discussion>();
            HashSet<Repository.Models.Discussion> tempDiscussions = new HashSet<Repository.Models.Discussion>();
            foreach (var item in discussions)
            {
                if(item.Comments.Count != 0)
                {
                    DiscussionwithComments.Add(item);
                }
                else
                {
                    DiscussionWithNoComments.Add(item);
                }
            }
            DiscussionwithComments = DiscussionwithComments.OrderBy(r => r.Comments.First().CreationTime).ToList<Repository.Models.Discussion>();
            DiscussionWithNoComments = DiscussionWithNoComments.OrderBy(r => r.CreationTime).ToList<Repository.Models.Discussion>();

            foreach(var item in DiscussionWithNoComments)
            {
               foreach (var d in DiscussionwithComments)
               { 
                    if(DateTime.Compare(item.CreationTime, d.Comments.First().CreationTime) < 0)
                    {
                        tempDiscussions.Add(d);
                    } 
                }
                tempDiscussions.Add(item);
            }  

            discussions.Clear();
            foreach (var item in tempDiscussions)
            {   
                discussions = tempDiscussions.ToList();
            }
            return discussions;
        }
    }   
}