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


            //Create array of comments to store parent comments
            List<Repository.Models.Comment> parentComments = new List<Repository.Models.Comment>();

            foreach (Repository.Models.Comment rc in repoComments)
            {
                System.Console.WriteLine(rc.CommentText);
                System.Console.WriteLine(rc.ParentCommentid);
                if (rc.ParentCommentid == null)
                {
                    parentComments.Add(rc);
                }
            }

            System.Console.WriteLine("Parent Comment Check");
            foreach (Repository.Models.Comment pc in parentComments)
            {
                System.Console.WriteLine(pc.CommentText);
                System.Console.WriteLine(pc.ParentCommentid);
            }

                       // Sort the list of comments
            switch (sortingOrder)
            {
                case "likes":
                    parentComments = sortByLikes(parentComments);
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
            if(sortingOrder.Equals("comments")){
                List<NestedComment> commentsTemp = new List<NestedComment>();

                foreach (var item in parentComments)
                {
                    commentsTemp.Add(Mapper.RepoCommentToNestedComment(item));
                }

                 //call a recursive function to send repoComments and add the children to the comments in page comments
                foreach (NestedComment nc in commentsTemp)
                {
                    //System.Console.WriteLine("for each: -------------------");
                    Mapper.AddReplies(repoComments, nc);
                    //System.Console.WriteLine(nc.Text);
                }

                commentsTemp = sortByComments(commentsTemp);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    comments.Add(commentsTemp[i]);
                }

            }else{
                
                System.Console.WriteLine("Start Index: " + startIndex);
                System.Console.WriteLine("Emd Index: " + endIndex);
                for (int i = startIndex; i <= endIndex; i++)
                {
                    //change to mapper to a different DTO that has replies[]
                    comments.Add(Mapper.RepoCommentToNestedComment(parentComments[i]));
                    //System.Console.WriteLine(comments[i].Text);
                }

                //call a recursive function to send repoComments and add the children to the comments in page comments
                foreach (NestedComment nc in comments)
                {
                    System.Console.WriteLine("for each: -------------------");
                    Mapper.AddReplies(repoComments, nc);
                    System.Console.WriteLine(nc.Text);
                }
            }

            return comments;
        }

        /// <summary>
        /// soring comments by number of likes
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<Repository.Models.Comment> sortByLikes(List<Repository.Models.Comment> comments){
            return comments.OrderByDescending(r => r.Likes).ToList<Repository.Models.Comment>();
        }

        /// <summary>
        /// sorting comments by number of nested comments
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<NestedComment> sortByComments(List<NestedComment> comments){
            
            return comments.OrderByDescending(r => r.Replies.Count).ToList<NestedComment>();
        }

        /// <summary>
        /// sorting comments by creation time Asc 
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<Repository.Models.Comment> sortByTimeCreationA(List<Repository.Models.Comment> comments){
            return comments.OrderBy(r => r.CreationTime).ToList<Repository.Models.Comment>();
        }

        /// <summary>
        /// sorting comments by creation time Desc
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private List<Repository.Models.Comment> sortByTimeCreationD(List<Repository.Models.Comment> comments){
            return comments.OrderByDescending(r => r.CreationTime).ToList<Repository.Models.Comment>();
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
            foreach (var repoDiscussion in repoDiscussions)
            {
                // Get the topic associated with this discussion

                Repository.Models.Topic topic = _repo.GetDiscussionTopic(repoDiscussion.DiscussionId);
                if (topic == null)
                {
                    topic = new Repository.Models.Topic();
                    topic.TopicName = "None";
                }
                discussions.Add(Mapper.RepoDiscussionToDiscussionT(repoDiscussion));
            }
            return discussions;
        }

        public async Task<List<DiscussionT>> GetDiscussionsPage(string movieid, int page, string sortingOrder)
        {
            if(page < 1)
            {
                Console.WriteLine("ForumLogic.GetDiscussionsPage() was called with a negative or zero page number.");
                return null;
            }

            Repository.Models.Setting pageSizeSetting = _repo.GetSetting("Discussionpagesize");
            
            int pageSize = pageSizeSetting.IntValue ?? default(int);
            
            if(pageSize < 1)
            {
                Console.WriteLine("ForumLogic.GetDiscussionsPage() was called but the Duscussionspagesize is invalid");
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
            Console.WriteLine(sortingOrder);
            // Sort the list of Discussion according to sorting string
            switch (sortingOrder)
            {
                case "like":
                    repoDiscussions = sortByNumOfLikes(repoDiscussions);
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
                case "recent":
                    repoDiscussions = sortByRecent(repoDiscussions);
                break;
            }

            int numOfDiscussion = repoDiscussions.Count;
            int start = pageSize * (page -1);
            if(start > numOfDiscussion - 1)
            {
                Console.WriteLine("ForumLogic.GetDiscussionsPage() was called for a page number without reviews.");
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
            foreach (var repoDiscussion in pageDiscussions)
            {
                
                // Get the topic associated with this discussion
                Repository.Models.Topic topic = _repo.GetDiscussionTopic(repoDiscussion.DiscussionId);
                if (topic == null)
                {
                    topic = new Repository.Models.Topic();
                    topic.TopicName = "None";
                }
                discussions.Add(Mapper.RepoDiscussionToDiscussionT(repoDiscussion));
                
            }
            return discussions;
        }

        /// <summary>
        /// sorting the Discussion list by number of likes
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByNumOfLikes( List<Repository.Models.Discussion> discussions){
            List<Repository.Models.Discussion> DiscussionwithComments = new List<Repository.Models.Discussion>();
            List<Repository.Models.Discussion> DiscussionWithNoComments = new List<Repository.Models.Discussion>();
            foreach (var item in discussions)
            {
                if(item.Comments.Count != 0){
                    DiscussionwithComments.Add(item);
                }else{
                    DiscussionWithNoComments.Add(item);
                }
            }
            DiscussionwithComments.OrderByDescending(r => r.Comments.First().Likes).ToList<Repository.Models.Discussion>();
            DiscussionwithComments.AddRange(DiscussionWithNoComments);
            return DiscussionwithComments;
         }

        /// <summary>
        /// sorting the Discussion list by number of comments in Ascending order
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByNumOfCommentsAsce( List<Repository.Models.Discussion> discussions){
            return discussions.OrderBy(r => r.Comments.Count).ToList<Repository.Models.Discussion>();
         }

        /// <summary>
        /// sorting the Discussion list by number of comments in Descending order
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByNumOfCommentsDesc( List<Repository.Models.Discussion> discussions){
            return discussions.OrderByDescending(r => r.Comments.Count).ToList<Repository.Models.Discussion>();
        }

        /// <summary>
        /// sorting the Discussion lsit by time created in ascending order  
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByCreationTimeAsce( List<Repository.Models.Discussion> discussions){
            return discussions.OrderBy(r => r.CreationTime).ToList<Repository.Models.Discussion>();
        }

        /// <summary>
        /// sorting the Discussion list by time it was created in Descending order
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByCreationTimeDesc( List<Repository.Models.Discussion> discussions){
            return discussions.OrderByDescending(r => r.CreationTime).ToList<Repository.Models.Discussion>();
        }

        /// <summary>
        /// sorting the Discussion lsit by recent activity
        /// </summary>
        /// <param name="discussions"></param>
        /// <returns></returns>
        private List<Repository.Models.Discussion> sortByRecent( List<Repository.Models.Discussion> discussions){
            List<Repository.Models.Discussion> DiscussionwithComments = new List<Repository.Models.Discussion>();
            List<Repository.Models.Discussion> DiscussionWithNoComments = new List<Repository.Models.Discussion>();
            HashSet<Repository.Models.Discussion> tempDiscussions = new HashSet<Repository.Models.Discussion>();
            foreach (var item in discussions)
            {
                if(item.Comments.Count != 0){
                    DiscussionwithComments.Add(item);
                    
                }else{
                    DiscussionWithNoComments.Add(item);
                }
            }
            DiscussionwithComments = DiscussionwithComments.OrderByDescending(r => r.Comments.First().CreationTime).ToList<Repository.Models.Discussion>();
            DiscussionWithNoComments = DiscussionWithNoComments.OrderByDescending(r => r.CreationTime).ToList<Repository.Models.Discussion>();

            foreach(var item in DiscussionWithNoComments)
            {
               foreach (var d in DiscussionwithComments)
               { 
                       if(DateTime.Compare(item.CreationTime, d.Comments.First().CreationTime) < 0){
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
        public async Task<DiscussionT> GetDiscussion(Guid discussionid)
        {
            Repository.Models.Discussion repoDiscussion = await _repo.GetDiscussion(discussionid.ToString());
            if (repoDiscussion == null)
            {
                _logger.LogWarning($"ForumLogic.GetDiscussion() was called for an invalid discussionid {discussionid}.");
                return null;
            }

            // Get the topic associated with this discussion
            // Repository.Models.Topic topic = _repo.GetDiscussionTopic(repoDiscussion.DiscussionId);
            // if (topic == null)
            // {
            //     topic = new Repository.Models.Topic();
            //     topic.TopicName = "None";
            // }
            DiscussionT discussion = Mapper.RepoDiscussionToDiscussionT(repoDiscussion);
            return discussion;
        }

        public async Task<List<Topic>> GetTopics()
        {
            var repoTopics = await _repo.GetTopics();
            if (repoTopics == null)
            {
                _logger.LogWarning($"ForumLogic.GetTopics() was called but there are no topics.");
                return null;
            }

            var topics = new List<Topic>();
            foreach (var repoTopic in repoTopics)
            {
                var newTopic = new Topic(repoTopic.TopicId, repoTopic.TopicName);
                topics.Add(newTopic);
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
                globalDiscussions.Add(Mapper.RepoDiscussionToDiscussionT(dis));
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
                    Comment nc = new Comment(Guid.Parse(ct.CommentId), Guid.Parse(ct.DiscussionId), ct.UserId, ct.CommentText, ct.IsSpoiler, ct.ParentCommentid, (int)ct.Likes);
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
            List<Repository.Models.DiscussionFollow> repoFollow = await _repo.GetFollowDiscussionList(userid);
            if(repoFollow == null)
            {
                return null;
            }
            List<DiscussionT> allDisc = new List<DiscussionT>();
            List<Task<DiscussionT>> tasks = new List<Task<DiscussionT>>();
            foreach(Repository.Models.DiscussionFollow disc in repoFollow)
            {
                tasks.Add(Task.Run(() => Mapper.RepoDiscussionToDiscussionT(disc.Discussion)));
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

        private List<Comment> SortByLikes(List<Comment> li)
        {
            Comparison<Comment> likes = new Comparison<Comment>(Comment.CompareLikes);
            li.Sort(likes);
            return li;
        }

        public async Task<bool> AddDiscussionTopic(string discussionid, string topicid)
        {
            return await _repo.AddDiscussionTopic(discussionid, topicid);
        }

      
    }
}