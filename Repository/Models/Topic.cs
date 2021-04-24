using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Model from database-first (backend model)
    /// </summary>
    public partial class Topic
    {
        public Topic()
        {
            DiscussionTopics = new HashSet<DiscussionTopic>();
        }

        public string TopicId { get; set; }
        public string TopicName { get; set; }

        public virtual ICollection<DiscussionTopic> DiscussionTopics { get; set; }
    }
}
