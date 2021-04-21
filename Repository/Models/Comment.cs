using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Repo Model -> Comment for database 
    /// </summary>
    public partial class Comment
    {
        public string CommentId { get; set; }
        public string DiscussionId { get; set; }
        public string Username { get; set; }
        public DateTime CreationTime { get; set; }
        public string CommentText { get; set; }
        public bool IsSpoiler { get; set; }

        public virtual Discussion Discussion { get; set; }
    }
}
