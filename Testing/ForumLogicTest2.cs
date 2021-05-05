﻿using BusinessLogic;
using GlobalModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Repository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Testing
{
    public class ForumLogicTest2
    {
        private Mock<IRepoLogic> repoStub = new Mock<IRepoLogic>();
        private Mock<ILogger<ForumLogic>> loggerStub = new Mock<ILogger<ForumLogic>>();
        private readonly ForumLogic _sut;

        public ForumLogicTest2()
        {
            _sut = new ForumLogic(repoStub.Object, loggerStub.Object);
        }

        [Fact]
        public async Task GetTopics_ShouldReturnListofTopics()
        {
            //Arrange
            var topiclist = new List<Repository.Models.Topic>
            {
                new Repository.Models.Topic { TopicId = Guid.NewGuid().ToString(), TopicName = "cde" },
                new Repository.Models.Topic { TopicId = Guid.NewGuid().ToString(), TopicName = "cde2" }

            };
            //Act
            repoStub.Setup(repo => repo.GetTopics())
                .ReturnsAsync(topiclist);

            var actual = await _sut.GetTopics();
            var expected = topiclist;
            //Assert

            Assert.Equal(actual.Count, expected.Count);
        }

        [Fact]
        public async Task GetComment_ShouldReturnCommentsIfDiscussionIdisPassed()
        {
            NewComment nc = new() { Discussionid = Guid.NewGuid(), Isspoiler = true, ParentCommentid = "sde", Text = "klo", Userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3" };
            var discussionId = Guid.NewGuid();
            repoStub.Setup(rep => rep.GetMovieComments(discussionId.ToString()))
             .Returns(dummyComments());

            var expected = await dummyComments();

            var actual = await _sut.GetComments(discussionId);

            //Assert.True(actual != null);
            Assert.Equal(expected.Count, actual.Count);

        }

        private async Task<List<Repository.Models.Comment>> dummyComments()
        {
            List<Repository.Models.Comment> dc = new();
            Repository.Models.Discussion newDisc = new Repository.Models.Discussion();
            newDisc.CreationTime = DateTime.Now;
            newDisc.DiscussionId = Guid.NewGuid().ToString();
            newDisc.MovieId = Guid.NewGuid().ToString();
            newDisc.Subject = "Disucssions1";
            newDisc.UserId = Guid.NewGuid().ToString();
            Repository.Models.Comment newComment = new Repository.Models.Comment();
            newComment.CommentId = Guid.NewGuid().ToString();
            newComment.CommentText = "Comment1";
            newComment.CreationTime = DateTime.Now;
            newComment.DiscussionId = newDisc.DiscussionId;
            newComment.IsSpoiler = false;
            newComment.Likes = 1;
            newComment.ParentCommentid = null;
            newComment.UserId = Guid.NewGuid().ToString();
            Repository.Models.Comment newComment1 = new Repository.Models.Comment();
            newComment1.CommentId = Guid.NewGuid().ToString();
            newComment1.CommentText = "Comment2";
            newComment1.CreationTime = DateTime.Now;
            newComment1.DiscussionId = newDisc.DiscussionId;
            newComment1.IsSpoiler = false;
            newComment1.Likes = 2;
            newComment1.ParentCommentid = null;
            newComment1.UserId = Guid.NewGuid().ToString();
            Repository.Models.Comment newComment2 = new Repository.Models.Comment();
            newComment2.CommentId = Guid.NewGuid().ToString();
            newComment2.CommentText = "Comment3";
            newComment2.CreationTime = DateTime.Now;
            newComment2.DiscussionId = newDisc.DiscussionId;
            newComment2.IsSpoiler = false;
            newComment2.Likes = 3;
            newComment2.ParentCommentid = null;
            newComment2.UserId = Guid.NewGuid().ToString();
            dc.Add(newComment);
            dc.Add(newComment1);
            dc.Add(newComment2);
            return dc;
        }


        [Fact]
        public async Task GetCommentPage_ShouldReturnNullifPageSizeislessthanone()
        {
            var discussionId = Guid.NewGuid();


            var actual = await _sut.GetCommentsPage(discussionId, -1, "abc");

            //Assert.True(actual != null);
            Assert.Null(actual);

        }

        [Fact]
        public async Task GetCommentPage_ShouldReturnNullifpageiszero()
        {


            var discussionId = Guid.NewGuid();
            //repoStub.Setup(rep => rep.GetMovieComments(discussionId.ToString()))
            //    .ReturnsAsync((List<Repository.Models.Comment>)null);

            var actual = await _sut.GetCommentsPage(discussionId, 0, discussionId.ToString());

            //Assert.True(actual != null);
            Assert.Null(actual);

        }

        [Fact]
        public async Task CreateTopic_ShouldReturnTrue()
        {
            //Arrange
            var topiclist = new List<Repository.Models.Topic>
            {
                new Repository.Models.Topic { TopicId = Guid.NewGuid().ToString(), TopicName = "cde" },
                new Repository.Models.Topic { TopicId = Guid.NewGuid().ToString(), TopicName = "cde2" }

            };
            //Act
            repoStub.Setup(repo => repo.AddTopic(It.IsAny<Repository.Models.Topic>()))
                .ReturnsAsync(true);

            var actual = await _sut.CreateTopic(topiclist[0].TopicName);

            //Assert

            Assert.True(actual);
        }

        [Fact]
        public async Task GetDiscussionsByTopicId_ShouldFalse()
        {
            //Arrange
            var topiclist = new List<Repository.Models.Topic>
            {
                new Repository.Models.Topic { TopicId = Guid.NewGuid().ToString(), TopicName = "cde" },
                new Repository.Models.Topic { TopicId = Guid.NewGuid().ToString(), TopicName = "cde2" }

            };
            var topid = topiclist[0].TopicId;
            //Act
            repoStub.Setup(repo => repo.GetDiscussionsByTopicId(topid))
                .Verifiable();

            var actual = await _sut.CreateTopic(topiclist[0].TopicId);

            //Assert

            Assert.False(actual); 
        }

        private List<DiscussionT> dummyDiscussionT()
        {
            var dt = new List<DiscussionT>
            {
                new DiscussionT
                {
                    DiscussionId = "5ad44605-d029-45b1-8f45-201a8299b634",
                    MovieId = "tt0848228",
                    Userid = "TestUser",
                    CreationTime = DateTime.Now,
                    Subject = "Running out of Topics",
                    Likes = 0,
                    Comments = { },

                    DiscussionTopics = { "09ca52db-382d-495e-9a7c-42ad8d6743b7" },

                },
                new DiscussionT
                {
                    DiscussionId = "4ad44605-d029-45b1-8f45-201a8299b634",
                    MovieId = "tt0844228",
                    Userid = "TestUse4r",
                    CreationTime = DateTime.Now,
                    Subject = "Running4 out of Topics",
                    Likes = 0,
                    Comments = { },

                    DiscussionTopics =
                    { "09ca52db-382d-495e-9a7c-42ad8d6743b7"
                    }

                },

            };

            return dt;
        }

    }
}