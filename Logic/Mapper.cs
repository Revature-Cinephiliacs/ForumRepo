using System;
using GlobalModels;

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
            var discussion = new Discussion(Guid.Parse(repoDiscussion.DiscussionId), repoDiscussion.MovieId,
                repoDiscussion.Username, repoDiscussion.Subject, topic.TopicName);
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
            var comment = new Comment(Guid.Parse(repoComment.CommentId), Guid.Parse(repoComment.DiscussionId),
                repoComment.Username, repoComment.CommentText, repoComment.IsSpoiler);
            return comment;
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
            repoDiscussion.Username = discussion.Username;
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
            repoComment.Username = comment.Username;
            repoComment.CommentText = comment.Text;
            repoComment.CreationTime = DateTime.Now;
            repoComment.IsSpoiler = comment.Isspoiler;

            return repoComment;
        }
    }
}
