using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GlobalModels;
using Microsoft.Extensions.Logging;

namespace CineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForumController : ControllerBase
    {
        private readonly IForumLogic _forumLogic;
        private readonly ILogger<ForumController> _logger;
        public ForumController(IForumLogic forumLogic, ILogger<ForumController> logger)
        {
            _forumLogic = forumLogic;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of Topic objects that includes every Topic.
        /// </summary>
        /// <returns></returns>
        [HttpGet("topics")]
        public async Task<ActionResult<List<Topic>>> GetTopics()
        {
            List<Topic> topics = await _forumLogic.GetTopics();

            if (topics.Count == 0)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return topics;
        }

        /// <summary>
        /// Returns a list of all Discussion objects that are associated with
        /// the provided movie ID.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        [HttpGet("discussions/{movieid}")]
        public async Task<ActionResult<List<DiscussionT>>> GetDiscussions(string movieid)
        {
            Console.WriteLine(movieid);
            List<DiscussionT> discussions = await _forumLogic.GetDiscussions(movieid);
            if (discussions == null)
            {
                return StatusCode(404);
            }
            if (discussions.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return discussions;
        }

        /// <summary>
        /// Returns a list of all Discussion objects that are associated with
        /// the provided movie ID.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        [HttpGet("discussions/{movieid}/{page}/{sortingOrder}")]
        public async Task<ActionResult<List<DiscussionT>>> GetDiscussionsPage(string movieid, int page, string sortingOrder)
        {
            Console.WriteLine(movieid);
            List<DiscussionT> discussions = await _forumLogic.GetDiscussionsPage(movieid, page, sortingOrder);
            if (discussions == null)
            {
                return StatusCode(404);
            }
            if (discussions.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            Console.WriteLine(discussions[0].Comments.Count);
            return discussions;
        }

        /// <summary>
        /// Returns the Discussion object with the provided discussion ID.
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        [HttpGet("discussion/{discussionid}")]
        public async Task<ActionResult<DiscussionT>> GetDiscussion(Guid discussionid)
        {
            DiscussionT discussion = await _forumLogic.GetDiscussion(discussionid);
            if (discussion == null)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return discussion;
        }

        /// <summary>
        /// Returns a list of all Comment objects that are associated with
        /// the provided discussion ID.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        [HttpGet("comments/{discussionid}")]
        public async Task<ActionResult<List<Comment>>> GetComments(Guid discussionid)
        {
            List<Comment> comments = await _forumLogic.GetComments(discussionid);
            if (comments == null)
            {
                return StatusCode(404);
            }
            if (comments.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return comments;
        }

        /// <summary>
        /// Returns Comments objects [n*(page-1), n*(page-1) + n] that are associated with the
        /// provided discussion ID. Where n is the current page size for comments.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("comments/{discussionid}/{page}/{sortingOrder}")]
        public async Task<ActionResult<List<NestedComment>>> GetCommentsPage(Guid discussionid, int page, string sortingOrder)
        {
            List<NestedComment> comments = await _forumLogic.GetCommentsPage(discussionid, page, sortingOrder);
            if (comments == null)
            {
                return StatusCode(404);
            }
            if (comments.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return comments;
        }

        /// <summary>
        /// Sets the page size for comments
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpPost("comments/page/{pagesize}")]
        public async Task<ActionResult> SetCommentsPageSize(int pagesize)
        {
            if (await _forumLogic.SetCommentsPageSize(pagesize))
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Adds a new Discussion based on the information provided.
        /// Returns a 400 status code if creation fails.
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        [HttpPost("discussion")]
        public async Task<ActionResult> CreateDiscussion([FromBody] NewDiscussion discussion)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.CreateDiscussion() was called with invalid body data.");
                return StatusCode(400);
            }
            //Console.WriteLine(discussion.Discussionid);
            if (await _forumLogic.CreateDiscussion(discussion))
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Adds a new Comment based on the information provided.
        /// Returns a 400 status code if creation fails.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost("comment")]
        public async Task<ActionResult> CreateComment([FromBody] NewComment comment)
        {
            System.Console.WriteLine("Form Controller: " + comment);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.CreateComment() was called with invalid body data.");
                return StatusCode(400);
            }

            if (await _forumLogic.CreateComment(comment))
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(400);
            }
        }
        
        /// <summary>
        /// Returns a list of sorted discussions based off number of comments (ascending)
        /// </summary>
        /// <returns></returns>
        [HttpGet("discussion/sort/comment/ascend")]
        public async Task<ActionResult<List<DiscussionT>>> GetSortedDiscussionsCommentsAscending()
        {
            List<DiscussionT> discussions = await _forumLogic.GetSortedDiscussionsByComments("a");
            if (discussions == null)
            {
                return StatusCode(404);
            }
            if (discussions.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return discussions;
        }

        /// <summary>
        /// Returns a list of sorted discussions based off number of comments (descending)
        /// </summary>
        /// <returns></returns>
        [HttpGet("discussion/sort/comment/descend")]
        public async Task<ActionResult<List<DiscussionT>>> GetSortedDiscussionsCommentsDescending()
        {
            List<DiscussionT> discussions = await _forumLogic.GetSortedDiscussionsByComments("d");
            if (discussions == null)
            {
                return StatusCode(404);
            }
            if (discussions.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return discussions;
        }
            
        /// <summary>
        /// Adds a new topic to the database
        /// Returns 201 if successful
        /// Returns 400 if failed
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpPost("topic/{topic}")]
        public async Task<ActionResult> CreateTopic(string topic)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.CreateComment() was called with invalid body data.");
                return StatusCode(400);
            }
            if (await _forumLogic.CreateTopic(topic))
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Returns a list of sorted discussions based on recent comments (ascending)
        /// </summary>
        /// <returns></returns>
        [HttpGet("discussion/sort/comment/recent")]
        public async Task<ActionResult<List<DiscussionT>>> GetDiscussionsCommentsRecent()
        {
            List<DiscussionT> discussions = await _forumLogic.GetSortedDiscussionsByComments("r");
            if (discussions == null)
            {
                return StatusCode(404);
            }
            if (discussions.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return discussions;
        }

        /// <summary>
        /// Returns a list of all Discussion objects that are associated with
        /// the provided movie ID.
        /// </summary>
        /// <returns></returns>
        [HttpGet("discussions/topic/{topicid}")]
        public async Task<ActionResult<List<DiscussionT>>> GetDiscussionsByTopicId(string topicid)
        {
            List<DiscussionT> discussions = await _forumLogic.GetDiscussionsByTopicId(topicid);
            if (discussions == null)
            {
                return StatusCode(404);
            }
            if (discussions.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return discussions;
        }

        /// <summary>
        /// Adds a user as a new follower to a discussion
        /// Returns 400 if couldn't model bind the id guid.
        /// Returns 404 if unable to find commentid.
        /// Returns 200 if successful.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPost("discussions/follow/{discussionid}/{userid}")]
        public async Task<ActionResult<bool>> FollowDiscussion(Guid discussionid, string userid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.FollowDiscussion() was called with invalid body data.");
                return StatusCode(400);
            }
            bool addSuccess = await _forumLogic.FollowDiscussion(discussionid, userid);
            if(!addSuccess)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return addSuccess;
        }

        /// <summary>
        /// Gets a list of dicussions followed by a user
        /// Returns 400 if invalid modelbinding
        /// Returns 204 if list is empty
        /// Returns 200 with complete list if user is following something
        /// </summary>
        /// <param name="discussionid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet("discussions/follow/{userid}")]
        public async Task<ActionResult<List<DiscussionT>>> FollowDiscussionList(string userid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.FollowDiscussionList() was called with invalid body data.");
                return StatusCode(400);
            }
            List<DiscussionT> followList = await _forumLogic.GetFollowDiscList(userid);
            if(followList == null)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return followList;
        }

        /// <summary>
        /// Changes a spoiler tag from true &lt; - > false
        /// Returns 400 if couldn't model bind the id guid.
        /// Returns 404 if unable to find commentid.
        /// Returns 200 if successful.
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        [HttpGet("spoiler/change/{commentid}")]
        public async Task<ActionResult<bool>> ChangeSpoiler(Guid commentid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.ChangeSpoiler() was called with invalid body data.");
                return StatusCode(400);
            }
            bool changedComment = await _forumLogic.ChangeSpoiler(commentid);
            if(!changedComment)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return changedComment;
        }

        /// <summary>
        /// Deletes a comment from the database.
        /// Returns 400 if couldn't model bind the id guid.
        /// Returns 404 if unable to find commentid.
        /// Returns 200 if successful.
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        [HttpDelete("comment/{commentid}")]
        public async Task<ActionResult<bool>> DeleteComment(Guid commentid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.DeleteComment() was called with invalid body data.");
                return StatusCode(400);
            }
            bool delSuccess = await _forumLogic.DeleteComment(commentid);
            if(!delSuccess)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return delSuccess;
        }

        /// <summary>
        /// Deletes a discussion from the database.
        /// Returns 400 if couldn't model bind the id guid.
        /// Returns 404 if unable to find discussionid.
        /// Returns 200 if successful.
        /// </summary>
        /// <param name="discussionid"></param>
        /// <returns></returns>
        [HttpDelete("discussion/{discussionid}")]
        public async Task<ActionResult<bool>> DeleteDiscussion(Guid discussionid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.DeleteDiscussion() was called with invalid body data.");
                return StatusCode(400);
            }
            bool delSuccess = await _forumLogic.DeleteDiscussion(discussionid);
            if(!delSuccess)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return delSuccess;
        }

        /// <summary>
        /// Deletes a topic from the database.
        /// Returns 400 if couldn't model bind the id guid.
        /// Returns 404 if unable to find topicid.
        /// Returns 200 if successful.
        /// </summary>
        /// <param name="topicid"></param>
        /// <returns></returns>
        [HttpDelete("topic/{topicid}")]
        public async Task<ActionResult<bool>> DeleteTopic(Guid topicid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.DeleteTopic() was called with invalid body data.");
                return StatusCode(400);
            }
            bool delSuccess = await _forumLogic.DeleteTopic(topicid);
            if(!delSuccess)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return delSuccess;
        }

        /// <summary>
        /// Likes a comment. Increments comment likes by one
        /// Returns 400 if couldn't model bind the id guid.
        /// Returns 404 if unable to find topicid.
        /// Returns 200 if successful.
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        [HttpPost("comment/like/{commentid}/{userid}")]
        public async Task<ActionResult<bool>> LikeComment(Guid commentid, string userid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.LikeComment() was called with invalid body data.");
                return StatusCode(400);
            }
            bool success = await _forumLogic.LikeComment(commentid, userid);
            if(!success)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return success;
        }

        [HttpPost("discussion/topic/{discussionid}/{topicid}")]
        public async Task<ActionResult<bool>> AddTopicToDiscussion(string discussionid, string topicid)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ForumController.AddTopicToDiscussion() was called with invalid body data.");
                return StatusCode(400);
            }

            bool success = await _forumLogic.AddDiscussionTopic(discussionid, topicid);
            if(!success)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return success;
        }
    }
}
