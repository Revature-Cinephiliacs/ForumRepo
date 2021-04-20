using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class DiscussionTopic
    {
        public int DiscussionId { get; set; }
        public string TopicName { get; set; }

        public virtual Discussion Discussion { get; set; }
        public virtual Topic TopicNameNavigation { get; set; }
    }
}
