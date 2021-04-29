using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Discussion
    {
        public Discussion()
        {
            Comments = new HashSet<Comment>();
            DiscussionFollows = new HashSet<DiscussionFollow>();
            DiscussionTopics = new HashSet<DiscussionTopic>();
        }

        public string DiscussionId { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string Subject { get; set; }
        public int? Totalikes { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<DiscussionFollow> DiscussionFollows { get; set; }
        public virtual ICollection<DiscussionTopic> DiscussionTopics { get; set; }
    }
}
