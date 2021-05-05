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
using Repository.Models;
using Comment = GlobalModels.Comment;
using Topic = GlobalModels.Topic;

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
            string result;

            NewComment dataSetA = new NewComment();
            dataSetA.Userid = "username1";
            dataSetA.Isspoiler = true;
            dataSetA.Text = "this is a text";
            

            Repository.Models.Topic newTopic = new Repository.Models.Topic();
            Repository.Models.Comment inputGMUser = BusinessLogic.Mapper.NewCommentToNewRepoComment(dataSetA);
            using(var context = new Repository.Models.Cinephiliacs_ForumContext(dbOptions))
            {              
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                RepoLogic repoLogic = new RepoLogic(context, repoLogger);

                

                result = await repoLogic.AddComment(inputGMUser);
            }

            Assert.Equal("false", result);
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

        [Fact]
        public async Task TestAddTopic()
        {
            var sut = new Repository.Models.Topic() { TopicId = "12345",TopicName = "Anis"};

            bool result;
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureDeleted();
                var msr = new RepoLogic(context1, repoLogger);
                 result = await msr.AddTopic(sut);
            }
            Assert.True(result);
        }
        [Fact]
        public async Task TestAddTopicBadPath()
        {
            var sut = new Repository.Models.Topic() {TopicId = "1234"};

            bool result = false;
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureDeleted();

                var msr = new RepoLogic(context1, repoLogger);
                
                Repository.Models.Topic getTopic =
                    await context1.Topics.FirstOrDefaultAsync(x => x.TopicName == sut.TopicName);
                if (getTopic != null)
                {
                    result = await msr.AddTopic(sut);
                }
            }
            Assert.False(result);
        }


        [Fact]
        public async Task TestdeleteComment()
        {
            Repository.Models.Comment comment = new Repository.Models.Comment()
                {CommentId = "12345", CommentText = "cool bro"};
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Comments.Add(comment);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.DeleteComment(comment.CommentId);
            }
            Assert.True(result);
        }
        [Fact]
        public async Task TestdeleteCommentBadPath()
        {
            Repository.Models.Comment comment = new Repository.Models.Comment()
                {CommentId = "12345", CommentText = "cool bro"};
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.DeleteComment(comment.CommentId);
            }
            Assert.False(result);
        }
        [Fact]
        public async Task TestdeleteDiscussion()
        {
            Discussion discussion = new Discussion()
                {DiscussionId = "12345", Totalikes = 23};
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Discussions.Add(discussion);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.DeleteDiscussion(discussion.DiscussionId);
            }
            Assert.True(result);
        }
        [Fact]
        public async Task TestdeleteDiscussionBadPath()
        {
            Repository.Models.Discussion discution = new Repository.Models.Discussion()
                {DiscussionId = "12345", Totalikes = 23};
            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.DeleteDiscussion(discution.DiscussionId);
            }
            Assert.False(result);
        }
        [Fact]
        public async Task TestdeleteTpic()
        {
            Repository.Models.Topic topic = new Repository.Models.Topic()
                {TopicId = "12345", TopicName = "Great"};
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Topics.Add(topic);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.DeleteTopic(topic.TopicId);
            }
            Assert.True(result);
        }
        [Fact]
        public async Task TestdeleteTopicBadPath()
        {
            Repository.Models.Topic discution = new Repository.Models.Topic()
                {TopicId = "12345", TopicName = "Great"};
            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.DeleteTopic(discution.TopicId);
            }
            Assert.False(result);
        }
        [Fact]
        public async Task TestFollowDiscussion()
        {
            Discussion discussion1 = new Discussion()
                {DiscussionId = "123", Totalikes = 23};

            DiscussionFollow discussion = new DiscussionFollow()
                {DiscussionId = "12345",UserId = "Anis",Discussion = discussion1};
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Discussions.Add(discussion1);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.FollowDiscussion(discussion);
            }
            Assert.False(result);
        }

        [Fact]
        public async Task TestGetFollowDiscussionByDiscussionId()
        {
            var disc = new Discussion() {DiscussionId = "123", Subject = "Great"};
            var sut1 = new DiscussionFollow() {DiscussionId = "1234", UserId = "12",Discussion = disc};
            
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.DiscussionFollows.Add(sut1);
                context1.SaveChanges();
            }

            List<DiscussionFollow> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetFollowDiscussionListByDiscussionId("123");
            }

            Assert.Single(result);
        }

        [Fact]
        public async Task GetMovieComment()
        {
            var comment = new Repository.Models.Comment() {CommentText = "Great", CommentId = "123"};
            var comment1 = new Repository.Models.Comment() {CommentText = "Great", CommentId = "12345"};
            List<Repository.Models.Comment> comments = new List<Repository.Models.Comment>();
            comments.Add(comment1);
            comments.Add(comment);
            
            var discution = new Discussion() {DiscussionId = "4444", Subject = "Great",Comments = comments};
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Comments.Add(comment1);
                context1.Comments.Add(comment);
                context1.Discussions.Add(discution);
                context1.SaveChanges();
            }

            List<Repository.Models.Comment> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetMovieComments(discution.DiscussionId);
            }
            Assert.Equal(2,result.Count);
        }
        [Fact]
        public async Task GetMovieCommentBadPath()
        {
            var comment = new Repository.Models.Comment() {CommentText = "Great", CommentId = "123"};
            var comment1 = new Repository.Models.Comment() {CommentText = "Great", CommentId = "12345"};
            var discution = new Discussion() {DiscussionId = "4444", Subject = "Great"};
            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Comments.Add(comment1);
                context1.Comments.Add(comment);
                context1.SaveChanges();
            }

            List<Repository.Models.Comment> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetMovieComments(discution.DiscussionId);
            }
            Assert.Null(result);
        }

        [Fact]
        public async Task TestGetTopic()
        {
            var sut = new Repository.Models.Topic() {TopicId = "432", TopicName = "America"};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.SaveChanges();
            }

            List<Repository.Models.Topic> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetTopics();
            }

            Assert.Single(result);
        }
        [Fact]
        public async Task TestDiscussionTopic()
        {
            var sut = new Repository.Models.Topic() {TopicId = "432", TopicName = "America"};
            var sut1 = new Discussion() {DiscussionId = "34234", Subject = "Great"};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(sut1);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.AddDiscussionTopic(sut1.DiscussionId,sut.TopicId);
            }

            Assert.True(result);
        }
        [Fact]
        public async Task TestDiscussionTopicNoTopic()
        {
           
            var sut1 = new Discussion() {DiscussionId = "34234", Subject = "Great"};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                
                context1.Add(sut1);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.AddDiscussionTopic(sut1.DiscussionId,"sdgsv");
            }

            Assert.False(result);
        }
        [Fact]
        public async Task TestDiscussionTopicNoDisccusion()
        {
            var sut = new Repository.Models.Topic() {TopicId = "432", TopicName = "America"};
            

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
               
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.AddDiscussionTopic("fsgvsfg",sut.TopicId);
            }

            Assert.False(result);
        }
        [Fact]
        public async Task TestChangeCommentSpoiler()
        {
            var sut = new Repository.Models.Comment() {CommentId = "432", CommentText = "America",IsSpoiler = true};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.ChangeCommentSpoiler(sut.CommentId);
            }

            Assert.True(result);
        }
        [Fact]
        public async Task TestChangeCommentSpoilerBadPath()
        {
            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.ChangeCommentSpoiler("sgsfgv");
            }

            Assert.False(result);
        }

        [Fact]
        public async Task TestLikeComment()
        {
            var comment = new Repository.Models.Comment() {CommentText = "cvsdvcsda", CommentId = "234"};
            var sut1 = new Discussion() {DiscussionId = "34234", Subject = "Great"};
            var sut2 = new Repository.Models.Comment() {CommentId = "432", CommentText = "America",IsSpoiler = true,DiscussionId ="34234" };
            var sut = new UserLike() {UserId = "1341",Comment = comment, CommentId = "234"};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut1);
                context1.Add(sut2);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.LikeComment(sut);
            }
            Assert.False(result);
        }
        [Fact]
        public async Task TestGetTopicById()
        {
            var sut = new Repository.Models.Topic() {TopicId = "432", TopicName = "America"};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.SaveChanges();
            }

            Repository.Models.Topic result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetTopicById(sut.TopicId);
            }

            Assert.NotNull(result);
        }
        [Fact]
        public async Task TestGetDisscusionById()
        {
            var sut = new Repository.Models.Discussion() {DiscussionId = "432", Subject = "America"};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.SaveChanges();
            }

            Repository.Models.Discussion result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetDiscussionsById(sut.DiscussionId);
            }

            Assert.NotNull(result);
        }
        [Fact]
        public async Task TestGetCommentById()
        {
            var sut = new Repository.Models.Comment() {CommentId = "432", CommentText = "America"};

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.SaveChanges();
            }

            Repository.Models.Comment result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetCommentById(sut.CommentId);
            }

            Assert.NotNull(result);
        }
        [Fact]
        public async Task TestGetListOfComment()
        {
            List<string> ids = new List<string>();
         
            var sut = new Repository.Models.Comment() {CommentId = "432", CommentText = "America"};
            var sut2 = new Repository.Models.Comment() {CommentId = "432cd", CommentText = "Americaadc"};
            ids.Add(sut.CommentId);
            ids.Add(sut2.CommentId);

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(sut2);
                context1.SaveChanges();
            }

            List<Repository.Models.Comment> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetCommentReportList(ids);
            }

            Assert.Equal(2,result.Count);
        }
        [Fact]
        public async Task TestGetListOfDisccusion()
        {
            List<string> ids = new List<string>();
         
            var sut = new Repository.Models.Discussion() {DiscussionId = "432", Subject = "America"};
            var sut2 = new Repository.Models.Discussion() {DiscussionId = "432cd", Subject = "Americaadc"};
            ids.Add(sut.DiscussionId);
            ids.Add(sut2.DiscussionId);

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(sut2);
                context1.SaveChanges();
            }

            List<Repository.Models.Discussion> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetDiscussionReportList(ids);
            }

            Assert.Equal(2,result.Count);
        }

        [Fact]
        public async Task TestGetListOfdiscussioDESC()
        {

            var sut = new Discussion() {DiscussionId = "432", Subject = "America"};
            var sut2 = new Discussion() {DiscussionId = "432cd", Subject = "Americaadc"};


            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(sut2);
                context1.SaveChanges();
            }

            List<Discussion> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetSortedDiscussionsDescending();
            }

            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task TestGetListOfdiscussioASC()
        {

            var sut = new Discussion() {DiscussionId = "432", Subject = "America"};
            var sut2 = new Discussion() {DiscussionId = "432cd", Subject = "Americaadc"};


            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(sut2);
                context1.SaveChanges();
            }

            List<Discussion> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetSortedDiscussionsAscending();
            }

            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task TestGetListOfdiscussio()
        {

            var sut = new Discussion() {DiscussionId = "432", Subject = "America"};
            var sut2 = new Discussion() {DiscussionId = "432cd", Subject = "Americaadc"};


            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(sut2);
                context1.SaveChanges();
            }

            List<Discussion> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetSortedDiscussionsRecent();
            }

            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task TestGetDisccusionTopicByID()
        {
            var topic = new Repository.Models.Topic() {TopicId = "1212"};
            var dissc = new Discussion() {DiscussionId = "313"};
            var comm = new Repository.Models.Comment() {CommentId = "3421"};
            var sut = new Repository.Models.DiscussionTopic() {DiscussionId = "432",TopicId = "333", Topic = topic,Discussion = dissc};
            

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(topic);
                context1.Add(dissc);
                context1.Add(comm);
                context1.SaveChanges();
            }

            List<DiscussionTopic> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetDiscussionsByTopicId("1212");
            }

            Assert.Equal(1, result.Count);
        }
        [Fact]
        public async Task TestGetDisccusionTopicByUserID()
        {
            var topic = new Repository.Models.Topic() {TopicId = "1212"};
            var dissc = new Discussion() {DiscussionId = "313",UserId = "3421"};
            var comm = new Repository.Models.UserLike() {UserId = "3421",CommentId = "adfadc"};
            var sut = new Repository.Models.DiscussionTopic() {DiscussionId = "432",TopicId = "333", Topic = topic,Discussion = dissc};
            

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(sut);
                context1.Add(topic);
                context1.Add(dissc);
                context1.Add(comm);
                context1.SaveChanges();
            }

            List<Discussion> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetDiscussionsByUserId("3421");
            }

            Assert.Equal(1, result.Count);
        }
        [Fact]
        public async Task TestGetComments()
        {
           
            var comm = new Repository.Models.Comment() {CommentId = "3421",UserId = "1212"};
           
            

            using (var context1 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(comm);
                context1.SaveChanges();
            }

            List<Repository.Models.Comment> result;
            using (var context2 = new Cinephiliacs_ForumContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2, repoLogger);
                result = await msr.GetCommentByUserId("1212");
            }

            Assert.Equal(1, result.Count);
        }
    }
}
