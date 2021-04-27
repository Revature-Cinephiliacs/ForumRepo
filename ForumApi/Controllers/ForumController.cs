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
        public async Task<ActionResult<List<string>>> GetTopics()
        {
            List<string> topics = await _forumLogic.GetTopics();

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
        public async Task<ActionResult<List<Discussion>>> GetDiscussions(string movieid)
        {
            Console.WriteLine(movieid);
            List<Discussion> discussions = await _forumLogic.GetDiscussions(movieid);
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
        /// Returns the Discussion object with the provided discussion ID.
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        [HttpGet("discussion/{discussionid}")]
        public async Task<ActionResult<Discussion>> GetDiscussion(Guid discussionid)
        {
            Discussion discussion = await _forumLogic.GetDiscussion(discussionid);
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
        [HttpGet("comments/{discussionid}/{page}")]
        public async Task<ActionResult<List<Comment>>> GetCommentsPage(Guid discussionid, int page)
        {
            List<Comment> comments = await _forumLogic.GetCommentsPage(discussionid, page);
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
    }
}
