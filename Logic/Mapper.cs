using System;
using GlobalModels;
using System.Collections.Generic;

namespace BusinessLogic
{
    public static class Mapper
    {
        /// <summary>
        /// Maps an instance of Repository.Models.Discussion and an instance of
        /// Repository.Models.Topic onto a new instance of GlobalModels.Discussion
        /// </summary>
        /// <param name="repoDiscussion"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static Discussion RepoDiscussionToDiscussion(Repository.Models.Discussion
            repoDiscussion, Repository.Models.Topic topic)
        {
            List<Comment> newComment = new List<Comment>();
            foreach (var item in repoDiscussion.Comments)
            {   
                if(item != null){
                    newComment.Add(RepoCommentToComment(item));
                }
            }
            var discussion = new Discussion(Guid.Parse(repoDiscussion.DiscussionId), repoDiscussion.MovieId,
                repoDiscussion.UserId, repoDiscussion.Subject, topic.TopicName, newComment);
            return discussion;
        }

        /// <summary>
        /// Maps an instance of Repository.Models.Comment onto a new instance of
        /// GlobalModels.Comment
        /// </summary>
        /// <param name="repoComment"></param>
        /// <returns></returns>
        public static Comment RepoCommentToComment(Repository.Models.Comment repoComment)
        {
            // var comment = new Comment(Guid.Parse(repoComment.CommentId), Guid.Parse(repoComment.DiscussionId),
            //     repoComment.UserId, repoComment.CommentText, repoComment.IsSpoiler);
            
            var comment = new Comment(Guid.Parse(repoComment.CommentId), Guid.Parse(repoComment.DiscussionId), repoComment.UserId,
                    repoComment.CommentText, repoComment.IsSpoiler, repoComment.ParentCommentid, (int)repoComment.Likes);
            return comment;
        }

        /// <summary>
        /// Maps an instance of Repository.Model.Comment onto a new instance of
        /// GlobalModels.NestedComment
        /// </summary>
        /// <param name="repoComment"></param>
        /// <returns></returns>
        public static NestedComment RepoCommentToNestedComment(Repository.Models.Comment repoComment)
        {
            var nestedComment = new NestedComment(Guid.Parse(repoComment.CommentId), Guid.Parse(repoComment.DiscussionId), repoComment.UserId,
                    repoComment.CommentText, repoComment.IsSpoiler, repoComment.ParentCommentid, (int)repoComment.Likes, repoComment.CreationTime);

            return nestedComment;
        }

        /// <summary>
        /// A helper recursive function that will take a list of discussion comments and a parent comment
        /// and the child replies to the parent commment
        /// </summary>
        /// <param name="repoComments"></param>
        /// <param name="parent"></param>
        public static void AddReplies(List<Repository.Models.Comment> repoComments, NestedComment parent)
        {
            for (int i = 0; i < repoComments.Count; i++)
            {
                System.Console.WriteLine("Add Replies");
                System.Console.WriteLine("Repo parent: " + repoComments[i].ParentCommentid);
                System.Console.WriteLine("Parent parent: " + parent.ParentCommentid);
                string parentId = parent.Commentid.ToString();
                if (repoComments[i].ParentCommentid == parentId)
                {
                    var nestedComment = RepoCommentToNestedComment(repoComments[i]);
                    parent.Replies.Add(nestedComment);
                    System.Console.WriteLine("added");

                    AddReplies(repoComments, nestedComment);
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

        public static DiscussionT RepoDiscussionToDiscussionT(Repository.Models.Discussion dis)
        {
            int totalLikes = 0;
            DiscussionT gdis = new();
            gdis.DiscussionId = dis.DiscussionId;
            gdis.MovieId = dis.MovieId;
            gdis.Userid = dis.UserId;
            gdis.Subject = dis.Subject;
            
            foreach (var ct in dis.Comments)
            {
                Comment nc = new Comment(Guid.Parse(ct.CommentId), Guid.Parse(ct.DiscussionId), ct.UserId, ct.CommentText, ct.IsSpoiler, ct.ParentCommentid, (int)ct.Likes);
                gdis.Comments.Add(nc);
                totalLikes += nc.Likes;
            }
            gdis.Likes = totalLikes;
            
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
    }
}
