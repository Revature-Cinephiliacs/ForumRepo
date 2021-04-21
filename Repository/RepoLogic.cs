using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository
{
    public class RepoLogic
    {
        public Task<bool> AddComment(Comment repoComment)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddDiscussion(Discussion repoDiscussion, Topic repoTopic)
        {
            throw new NotImplementedException();
        }

        public Task<List<Comment>> GetMovieComments(int discussionid)
        {
            throw new NotImplementedException();
        }

        public Setting GetSetting(string v)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public Task<List<Discussion>> GetMovieDiscussions(string movieid)
        {
            throw new NotImplementedException();
        }

        public Topic GetDiscussionTopic(int discussionId)
        {
            throw new NotImplementedException();
        }

        public Task<Discussion> GetDiscussion(int discussionid)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Topic>> GetTopics()
        {
            throw new NotImplementedException();
        }
    }
}