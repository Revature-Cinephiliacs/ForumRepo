using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Repo Model from database-first scaffolding
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
