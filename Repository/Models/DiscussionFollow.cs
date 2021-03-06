using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Repo Model from database-first scaffolding
    /// </summary>
    public partial class DiscussionFollow
    {
        public string DiscussionId { get; set; }
        public string UserId { get; set; }

        public virtual Discussion Discussion { get; set; }
    }
}
