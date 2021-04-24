using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Model from database-first (backend model)
    /// </summary>
    public partial class DiscussionTopic
    {
        public string DiscussionId { get; set; }
        public string TopicId { get; set; }

        public virtual Discussion Discussion { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
