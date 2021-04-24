using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Database-first generated backend model
    /// </summary>
    public partial class DiscussionTopic
    {
        public string DiscussionId { get; set; }
        public string TopicName { get; set; }

        public virtual Discussion Discussion { get; set; }
        public virtual Topic TopicNameNavigation { get; set; }
    }
}
