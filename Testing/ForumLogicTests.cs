using BusinessLogic;
using CineAPI.Controllers;
using Repository.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository;
using System;
using System.Threading.Tasks;
using Xunit;
using GlobalModels;
using Comment = Repository.Models.Comment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Testing
{
    public class ForumLogicTests
    {
        private Mock<IRepoLogic> repoStub = new Mock<IRepoLogic>();

        readonly DbContextOptions<Repository.Models.Cinephiliacs_ForumContext> dbOptions =
            new DbContextOptionsBuilder<Repository.Models.Cinephiliacs_ForumContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        
        readonly ILogger<ForumLogic> logicLogger = new ServiceCollection().AddLogging().BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger<ForumLogic>();
        readonly ILogger<RepoLogic> repoLogger = new ServiceCollection().AddLogging().BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger<RepoLogic>();

        [Fact]
        public void CreateComment_WithCommentObject_ReturnTrue()
        {
            // Arrange
            NewComment nc = new NewComment(Guid.NewGuid(), "def", "fgh", true, null);

            repoStub.Setup(repo => repo.AddComment(It.IsAny<Repository.Models.Comment>())).ReturnsAsync("true");
            var logic = new ForumLogic(repoStub.Object, logicLogger);

            // Act
            var result = logic.CreateComment(nc);

            //Assert
            Assert.True(result.Result);
        }

        [Fact]
        public void CreateDiscussion_WithDiscusssionObject_ReturnTrue()
        {
            // Arrange
            NewDiscussion nd = new NewDiscussion();
            nd.Movieid = "any_string";
            nd.Userid = "User Name";
            nd.Subject = "Subject";
            Repository.Models.Topic tp = new Repository.Models.Topic();
            tp.TopicName = "Any Topic";
            
            repoStub.Setup(repo => repo.AddDiscussion(It.IsAny<Repository.Models.Discussion>(), It.IsAny<Repository.Models.Topic>())).ReturnsAsync("true");

            var logic = new ForumLogic(repoStub.Object, logicLogger);

            // Act
            var result = logic.CreateDiscussion(nd);

            //Assert
            Assert.True(result.Result);
        }

        [Fact]
        public void GetDiscussions_WithUnExisintDiscussion_ReturnsNull()
        {
            // Arrange
            repoStub.Setup(repo => repo.GetDiscussion(It.IsAny<string>()))
                .ReturnsAsync((Repository.Models.Discussion) null);

            var logic = new ForumLogic(repoStub.Object, logicLogger);

            // Act
            var result = logic.GetDiscussion(Guid.NewGuid());

            // Assert
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task GetComments_ReturnsList()
        {
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
            using (var context = new Cinephiliacs_ForumContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add<Repository.Models.Discussion>(newDisc);
                context.Add<Comment>(newComment);
                context.Add<Comment>(newComment1);
                context.Add<Comment>(newComment2);
                context.SaveChanges();
            }   

            List<GlobalModels.Comment> listComments;
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureCreated();
                
                RepoLogic repo = new RepoLogic(context1, repoLogger);
                ForumLogic logic = new ForumLogic(repo, logicLogger);

                listComments = await logic.GetComments(Guid.Parse(newDisc.DiscussionId));
            }

            Assert.Equal(3, listComments.Count);
        }

        [Fact]
        public async Task GetComments_ReturnsNull()
        {
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
            using (var context = new Cinephiliacs_ForumContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add<Repository.Models.Discussion>(newDisc);
                context.Add<Comment>(newComment);
                context.Add<Comment>(newComment1);
                context.Add<Comment>(newComment2);
                context.SaveChanges();
            }   

            List<GlobalModels.Comment> listComments;
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureCreated();
                
                RepoLogic repo = new RepoLogic(context1, repoLogger);
                ForumLogic logic = new ForumLogic(repo, logicLogger);

                listComments = await logic.GetComments(Guid.NewGuid());
            }

            Assert.Null(listComments);
        }
    }
}
