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

namespace Testing
{
    public class ForumLogicTests
    {
        private Mock<IRepoLogic> repoStub = new Mock<IRepoLogic>();

        [Fact]
        public void CreateComment_WithCommentObject_ReturnTrue()
        {
            // Arrange
            NewComment nc = new NewComment(Guid.NewGuid(), "def", "fgh", true);

            repoStub.Setup(repo => repo.AddComment(It.IsAny<Repository.Models.Comment>())).ReturnsAsync(true);
            var logic = new ForumLogic(repoStub.Object);
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
            nd.Username = "User Name";
            nd.Subject = "Subject";
            Topic tp = new Topic();
            tp.TopicName = "Any Topic";
            

            repoStub.Setup(repo => repo.AddDiscussion(It.IsAny<Repository.Models.Discussion>(), It.IsAny<Topic>())).ReturnsAsync(true);

            var logic = new ForumLogic(repoStub.Object);
            // Act

            var result = logic.CreateDiscussion(nd);


            //Assert

            Assert.True(result.Result);
        }

        //[Fact]
        //public void GetComments_WithoutDiscussionId_ReturnCommentsList()
        //{
        //    // Arrange
        //    var discussionId = Guid.NewGuid();


        //    repoStub.Setup(repo => repo.GetMovieComments(It.IsAny<string>()))
        //        .Returns((Repository.Models.Comment) null);

        //    var logic = new ForumLogic(repoStub.Object);
        //    // Act

        //    var result = logic.GetComments(discussionId);


        //    //Assert

        //    Assert.Null(result.Result);
        //}




        [Fact]
        public void GetDiscussions_WithUnExisintDiscussion_ReturnsNull()
        {
            // Arrange

            repoStub.Setup(repo => repo.GetDiscussion(It.IsAny<string>()))
                .ReturnsAsync((Repository.Models.Discussion) null);

            var logic = new ForumLogic(repoStub.Object);

            // Act

            var result = logic.GetDiscussion(Guid.NewGuid());

            // Assert

            Assert.Null(result.Result);
        }
    }
}
