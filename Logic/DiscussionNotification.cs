using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Facillitates passing of notifications.
    /// </summary>
    /// <value></value>
    public class DiscussionNotification
    {
        public string Movieid { get; set; }
        public string Userid { get; set; }
        public string Discussionid { get; set; }

        public DiscussionNotification(string movieid, string userid, string discussionid)
        {
            Movieid = movieid;
            Userid = userid;
            Discussionid = discussionid;
        }

    }
}
