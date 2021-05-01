using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class CommentNotification
    {
        /// <summary>
        /// Facillitates passing of notifications.
        /// </summary>
        /// <value></value>
        public string Imdbid { get; set; }
        public string Usernameid { get; set; }
        public string DiscussionId { get; set; }
        public string CommentId { get; set; }
        public List<string> Followers { get; set; }

        public CommentNotification(string movieid, string userid, string discussionid, string commentid, List<string> followers)
        {
            Imdbid = movieid;
            Usernameid = userid;
            DiscussionId = discussionid;
            CommentId = commentid;
            Followers = followers;
        }
    }
}
