using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Model from database-first (backend model)
    /// </summary>
    public partial class Comment
    {
        public string CommentId { get; set; }
        public string DiscussionId { get; set; }
        public string UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string CommentText { get; set; }
        public bool IsSpoiler { get; set; }

        public virtual Discussion Discussion { get; set; }
    }
}
