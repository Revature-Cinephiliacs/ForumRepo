﻿using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Repository
{
    public interface IRepoLogic
    {
        /// <summary>
        /// Save the comment into database
        /// Returns true is successful
        /// Returns false if username or discussion ID doesn't exist 
        /// </summary>
        /// <param name="repoComment"></param>
        /// <returns></returns>
        Task<bool> AddComment(Comment repoComment);

        /// <summary>
        /// Saving Discussion into database
        /// Return ture if saved succeffully 
        /// Return false if user or movie doesn't exist  
        /// </summary>
        /// <param name="repoDiscussion"></param>
        /// <param name="repoTopic"></param>
        /// <returns></returns>
        Task<bool> AddDiscussion(Discussion repoDiscussion, Topic repoTopic);

        /// <summary>
        /// Returns a list of all Comment objects from the database that match the discussion ID specified
        /// in the argument. Returns null if the discussion doesn't exist.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        Task<List<Comment>> GetMovieComments(string discussionid);

        /// <summary>
        /// Gets the value(s) of an existing setting in the database with a matching key string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Setting GetSetting(string key);

        /// <summary>
        /// Creates a new setting entry or updates the value(s) of an existing setting
        /// in the database.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        Task<bool> SetSetting(Setting setting);

        /// <summary>
        /// Returns a list of all Comment objects from the database that match the discussion ID specified
        /// Returns null if the discussion doesn't exist.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public Task<List<Discussion>> GetMovieDiscussions(string movieid);

        /// <summary>
        /// Returns the Topic object from the database that matches the discussionId specified
        /// in the argument. Returns null if the discussionid doesn't exist or that discussion
        /// has no topic.
        /// </summary>
        /// <param name="discussionId"></param>
        /// <returns></returns>
        public Topic GetDiscussionTopic(string discussionId);

        /// <summary>
        /// Returns the Discussion object that match the discussionid specified in the argument.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        public Task<Discussion> GetDiscussion(string discussionid);

        /// <summary>
        /// Returns a list of all Topic objects in the database.
        /// </summary>
        /// <returns></returns>
        public Task<List<Topic>> GetTopics();

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
        public Task<bool> AddDiscussionTopic(string discussionId, string topicName);

        /// <summary>
        /// Gets a list of discussions sorted by number of comments (ascending)
        /// </summary>
        /// <returns></returns>
        public Task<List<Discussion>> GetSortedDiscussionsDescending();

        /// <summary>
        /// Gets a list of dicussions sorted by number of comments (ascending)
        /// </summary>
        /// <returns></returns>
        public Task<List<Discussion>> GetSortedDiscussionsAscending();

        /// <summary>
        /// Get a list of dicussions sorted by most recent discussions
        /// </summary>
        /// <returns></returns>
        public Task<List<Discussion>> GetSortedDiscussionsRecent();

        /// <summary>
        /// Get a list of dicussions sorted by topic
        /// </summary>
        /// <returns></returns>
        public Task<List<Discussion>> GetDiscussionsByTopicId(string topicid);
        /// <summary>
        /// Saves the topic into the database
        /// Returns true if save is success
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public Task<bool> AddTopic(Topic topic);

        /// <summary>
        /// Changes a comment spoiler tag from true &lt; - > false
        /// Returns true is successful
        /// Returns false if failed
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public Task<bool> ChangeCommentSpoiler(string commentid);

        /// <summary>
        /// Deletes a comment from the database
        /// Returns true if successful
        /// Returns false if failed
        /// </summary>
        /// <param name="toString"></param>
        /// <returns></returns>
        public Task<bool> DeleteComment(string commentid);

        /// <summary>
        /// Deletes a discussion from the database
        /// First deletes all references in discussiontopics table
        /// Second deletes all references in comments table
        /// Lastly deletes the discussion itself
        /// Returns true if successful
        /// Returns false if failed
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Task<bool> DeleteDiscussion(string discussionid);

        /// <summary>
        /// Deletes a topic from the database
        /// First deletes all references of the topic
        /// Then deletes the topic itself
        /// Returns true if successful
        /// Return false if failed
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Task<bool> DeleteTopic(string topicid);

        /// <summary>
        /// Adds a new DiscussionFollow object into database
        /// Creates a user-discussion follow relationship
        /// Returns true if successful
        /// Returns false if failed
        /// </summary>
        /// <param name="newFollow"></param>
        /// <returns></returns>
        public Task<bool> FollowDiscussion(DiscussionFollow newFollow);

        /// <summary>
        /// Returns a list of followed discussions based on userid
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Task<List<DiscussionFollow>> GetFollowDiscussionList(string userid);

        /// <summary>
        /// Likes a comment
        /// Increment a comment's like value
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Task<bool> LikeComment(string commentid, string userid);
    }
}
