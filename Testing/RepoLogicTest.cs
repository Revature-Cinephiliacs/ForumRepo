using System;
using Xunit;
using Microsoft.EntityFrameworkCore;

using Repository;

namespace Testing
{
    public class RepoLocigTesting
    {
        readonly DbContextOptions<Repository.Models.Cinephiliacs_ForumContext> dbOptions =
            new DbContextOptionsBuilder<Repository.Models.Cinephiliacs_ForumContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        [Fact]
        public void Test1()
        {

        }

    }
}
