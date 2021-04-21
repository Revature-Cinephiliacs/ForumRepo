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
            var discussion = new Discussion(repoDiscussion.DiscussionId, repoDiscussion.MovieId,
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
            var comment = new Comment(repoComment.CommentId, repoComment.DiscussionId,
                repoComment.Username, repoComment.CommentText, repoComment.IsSpoiler);
            return comment;
        }

        /// <summary>
        /// Maps an instance of Repository.Models.Review onto a new instance of
        /// GlobalModels.Review
        /// </summary>
        /// <param name="repoReview"></param>
        /// <returns></returns>
        public static Review RepoReviewToReview(Repository.Models.Review repoReview)
        {
            var review = new Review(repoReview.MovieId, repoReview.Username, repoReview.Rating,
                repoReview.Review1);
            return review;
        }

        /// <summary>
        /// Maps an instance of GlobalModels.Review onto a new instance of
        /// Repository.Models.Review. Sets Repository.Models.Review.CreationTime
        /// to the current time.
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        public static Repository.Models.Review ReviewToRepoReview(Review review)
        {
            var repoReview = new Repository.Models.Review();
            repoReview.Username = review.Username;
            repoReview.MovieId = review.Movieid;
            repoReview.Rating = review.Rating;
            repoReview.Review1 = review.Text;
            repoReview.CreationTime = DateTime.Now;

            return repoReview;
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
            repoComment.DiscussionId = comment.Discussionid;
            repoComment.Username = comment.Username;
            repoComment.CommentText = comment.Text;
            repoComment.CreationTime = DateTime.Now;
            repoComment.IsSpoiler = comment.Isspoiler;

            return repoComment;
        }
    }
}
