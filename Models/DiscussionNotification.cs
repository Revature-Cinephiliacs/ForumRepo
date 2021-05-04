using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GlobalModels
{
    /// <summary>
    /// Facillitates passing of notifications.
    /// </summary>
    /// <value></value>
    public class DiscussionNotification
    {
        public string Imdbid { get; set; }
        public string Usernameid { get; set; }
        public string Discussionid { get; set; }
    
        public DiscussionNotification(string movieid, string userid, string discussionid)
        {
            Imdbid = movieid;
            Usernameid = userid;
            Discussionid = discussionid;
        }

        public DiscussionNotification()
        {
        }
    }
}
