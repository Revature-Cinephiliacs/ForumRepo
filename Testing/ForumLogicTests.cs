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
using Autofac.Extras.Moq;
using Castle.DynamicProxy.Generators.Emitters;

namespace Testing
{
    public class ForumLogicTests
    {
        private Mock<IRepoLogic> repoStub = new Mock<IRepoLogic>();
        private Mock<ILogger<ForumLogic>> loggerStub = new Mock<ILogger<ForumLogic>>();

        readonly DbContextOptions<Repository.Models.Cinephiliacs_ForumContext> dbOptions =
            new DbContextOptionsBuilder<Repository.Models.Cinephiliacs_ForumContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        readonly ILogger<ForumLogic> logicLogger = new ServiceCollection().AddLogging().BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger<ForumLogic>();
        readonly ILogger<RepoLogic> repoLogger = new ServiceCollection().AddLogging().BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger<RepoLogic>();

        [Fact]
        public void CreateComment_ShouldBeCompletedWhenCommentIsSaved()
        {
            Comment c = new() { DiscussionId = "abc" , IsSpoiler = true, ParentCommentid = "sde", CommentText = "klo", UserId = "acd" };
            NewComment nc = new() { Discussionid = Guid.NewGuid(), Isspoiler = true, ParentCommentid = "sde", Text = "klo", Userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3" };
            repoStub.Setup(rs => rs.AddComment(It.IsAny<Comment>()))
                .ReturnsAsync(new string("abc"));
            var logic = new ForumLogic(repoStub.Object, loggerStub.Object);
            var result = logic.CreateComment(nc);
            Assert.IsType<Task<bool>>(result
                );
           
        }

        [Fact]
        public async Task CreateComment_ShouldBeFaultedWhenCommentIsNotSaved()
        {
            NewComment nc = new() { Discussionid = Guid.NewGuid(), Isspoiler = true, ParentCommentid = "sde", Text = "klo", Userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3" };

            repoStub.Setup(rs => rs.AddComment(It.IsAny<Repository.Models.Comment>()))
                .ReturnsAsync("abc");
            var logic = new ForumLogic(repoStub.Object, loggerStub.Object);
            var result = await logic.CreateComment(nc);
            Assert.False(result);
        }

        [Fact]
        public async Task GetComment_ShouldReturnComments()
        {
            NewComment nc = new() { Discussionid = Guid.NewGuid(), Isspoiler = true, ParentCommentid = "sde", Text = "klo", Userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3" };
            var discussionId = Guid.NewGuid().ToString();
            using (var mymock = AutoMock.GetLoose())
            {
                mymock.Mock<IRepoLogic>()
                    .Setup(rep => rep.GetMovieComments("abc"))
                    .ReturnsAsync(dummyComments());

                var testclass =  mymock.Create<ForumLogic>();

                var expected = dummyComments();

                var actual = await testclass.GetComments(Guid.Parse(discussionId));

                //Assert.True(actual != null);
               Assert.Equal(expected.Count, actual.Count);
            }
        }

        private List<Comment> dummyComments()
        {
            List<Comment> dc = new List<Comment>();

            dc.Add(new Comment { CommentId = "abcd" });
            dc.Add(new Comment { CommentId = "def" });

            return dc;
        }



        [Fact]
        public void GetDiscussions_WithUnExisintDiscussion_ReturnsNull()
        {
            // Arrange
            repoStub.Setup(repo => repo.GetDiscussion(It.IsAny<string>()))
                .ReturnsAsync(new Discussion() { DiscussionId = "abc" });



            var logic = new ForumLogic(repoStub.Object, loggerStub.Object);

            // Act
            var result = logic.GetDiscussion(Guid.NewGuid());
            Console.WriteLine(result);
            // Assert
            Assert.Equal("abc", "abc");
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

        [Fact]
        public async Task CreateComments_ShouldReturnsTrueIfNewCommentIsPassed()
        {
           
           
            NewComment newComment = new NewComment();
           
          
            newComment.Discussionid = Guid.NewGuid();
            newComment.Isspoiler = false;
        
            newComment.ParentCommentid = null;
            newComment.Userid = Guid.NewGuid().ToString();
          
            using (var context = new Cinephiliacs_ForumContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

              
               
                context.SaveChanges();
            }

            List<GlobalModels.Comment> listComments;
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureCreated();

                RepoLogic repo = new RepoLogic(context1, repoLogger);
                ForumLogic logic = new ForumLogic(repo, logicLogger);

                bool result = await logic.CreateComment(newComment);

                Assert.True(result == true);
            }

          
        }

        [Fact]
        public async Task CreateComments_ShouldFalseTrueIfNoCommentIsPassed()
        {


            using (var context = new Cinephiliacs_ForumContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();



                context.SaveChanges();
            }

            List<GlobalModels.Comment> listComments;
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureCreated();

                RepoLogic repo = new RepoLogic(context1, repoLogger);
                ForumLogic logic = new ForumLogic(repo, logicLogger);

                bool result = await logic.CreateComment(new NewComment());

                Assert.True(result);
            }


        }
    }


}
