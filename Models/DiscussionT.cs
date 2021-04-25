using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalModels
{
    public class DiscussionT
    {
        public DiscussionT()
        {
            Comments = new List<Comment>();
            DiscussionTopics = new List<string>();
        }

        public string DiscussionId { get; set; }
        public string MovieId { get; set; }
        public string Userid { get; set; }
        public DateTime CreationTime { get; set; }
        public string Subject { get; set; }

        public List<Comment> Comments { get; set; }
        public List<string> DiscussionTopics { get; set; }
    }
}
