using System;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using GlobalModels;
using BusinessLogic;
using Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Testing
{
    public class RepoLogicTesting
    {
        readonly DbContextOptions<Repository.Models.Cinephiliacs_ForumContext> dbOptions =
            new DbContextOptionsBuilder<Repository.Models.Cinephiliacs_ForumContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        readonly ILogger<RepoLogic> repoLogger = new ServiceCollection().AddLogging().BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger<RepoLogic>();

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
                RepoLogic repoLogic = new RepoLogic(context, repoLogger);
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
                RepoLogic repoLogic = new RepoLogic(context, repoLogger);
                
                result =  repoLogic.GetSetting(" ");

            Assert.Null(result);
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
            dataSetA.Userid = "username1";
            dataSetA.Isspoiler = true;
            dataSetA.Text = "this is a text";
            

            Repository.Models.Topic newTopic = new Repository.Models.Topic();

            using(var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions))
            {              
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                RepoLogic repoLogic = new RepoLogic(context, repoLogger);

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

                RepoLogic repoLogic = new RepoLogic(context, repoLogger);

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

                RepoLogic repoLogic = new RepoLogic(context, repoLogger);

                result = await repoLogic.GetMovieDiscussions("movieIb?");
            }

            Assert.Empty(result);
        }

    }
}
