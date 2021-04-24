using System;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

using GlobalModels;
using BusinessLogic;
using Repository;

namespace Testing
{
    public class RepoLocigTesting
    {
        readonly DbContextOptions<Repository.Models.Cinephiliacs_ForumContext> dbOptions =
            new DbContextOptionsBuilder<Repository.Models.Cinephiliacs_ForumContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        /// <summary>
        /// Testing SetSetting
        /// SetSetting should retun false if setting.setting1 length is less than 1 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetSettingWithOutSettingTest()
        {
            bool result; 
                var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions);
                RepoLogic repoLogic = new RepoLogic(context);
                Repository.Models.Setting setting = new Repository.Models.Setting();
                setting.StringValue = "one";
                setting.Setting1 = "";
                setting.IntValue = 1;
                result = await repoLogic.SetSetting(setting);

            Assert.False(result);
        }

        /// <summary>
        /// Testing GetSetting function
        /// GetSetting should return Setting 
        /// However it shoud return null if it doesn't find setting with given key 
        /// </summary>
        [Fact]
        public void GetSettingWithNoKeyTest()
        {
            Repository.Models.Setting result; 
                var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions);
                RepoLogic repoLogic = new RepoLogic(context);
                
                result =  repoLogic.GetSetting(" ");

            Assert.Null(result);
        }

        /// <summary>
        /// Testing AddDiscussion function in RepoLogic 
        /// if username is null the AddDiscussion should not presist the data 
        /// and return false so this test is for chekging that.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NoUserAddDiscussionTest()
        {
            bool result;

            NewDiscussion dataSetA = new NewDiscussion();
            dataSetA.Movieid = "string movieid";
            dataSetA.Subject = "stringsubject";
            dataSetA.Topic = "string";
            

            Repository.Models.Topic newTopic = new Repository.Models.Topic();

            using(var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions))
            {              
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                RepoLogic repoLogic = new RepoLogic(context);

                Repository.Models.Discussion inputGMUser = BusinessLogic.Mapper.NewDiscussionToNewRepoDiscussion(dataSetA);

                // Test AddDiscussion() without User dependency
                result = await repoLogic.AddDiscussion(inputGMUser, newTopic);
            }

            Assert.False(result);
        }

        /// <summary>
        /// Testing AddComment() from RepoLgic
        /// AddConnet should return false if the discussion is null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NoDiscussionAddComment()
        {
            bool result;

            NewComment dataSetA = new NewComment();
            dataSetA.Username = "username1";
            dataSetA.Isspoiler = true;
            dataSetA.Text = "this is a text";
            

            Repository.Models.Topic newTopic = new Repository.Models.Topic();

            using(var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions))
            {              
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                RepoLogic repoLogic = new RepoLogic(context);

                Repository.Models.Comment inputGMUser = BusinessLogic.Mapper.NewCommentToNewRepoComment(dataSetA);

                result = await repoLogic.AddComment(inputGMUser);
            }

            Assert.False(result);
        }

        /// <summary>
        /// Testing GetDiscussion from repologic
        /// GetDiscussion should return null if it doesn't find the discussion in DB
        /// Passing random string.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDiscussionWithOutIdTest()
        {
            Repository.Models.Discussion result;

            using(var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions))
            {              
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                RepoLogic repoLogic = new RepoLogic(context);

                result = await repoLogic.GetDiscussion("what's up?");
            }

            Assert.Null(result);
        }

        /// <summary>
        /// Testing GetMovieDiscussion from repologic
        /// GetMovieDiscussion should return empty list if it doesn't find the discussion in DB
        /// Passing random string.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetMovieDiscussionWithOutMovieTest()
        {
            List<Repository.Models.Discussion> result; //= new List<Repository.Models.Discussion>();

            using(var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions))
            {              
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                RepoLogic repoLogic = new RepoLogic(context);

                result = await repoLogic.GetMovieDiscussions("movieIb?");
            }

            Assert.Empty(result);
        }

    }
}
