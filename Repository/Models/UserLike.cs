using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Repo Model from database-first scaffolding
    /// </summary>
    public partial class UserLike
    {
        public string CommentId { get; set; }
        public string UserId { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
