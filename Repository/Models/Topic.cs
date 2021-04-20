using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Topic
    {
        public Topic()
        {
            DiscussionTopics = new HashSet<DiscussionTopic>();
        }

        public string TopicName { get; set; }

        public virtual ICollection<DiscussionTopic> DiscussionTopics { get; set; }
    }
}
