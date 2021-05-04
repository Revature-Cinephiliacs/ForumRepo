using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GlobalModels
{
    public class CommentNotification
    {
        /// <summary>
        /// Facillitates passing of notifications.
        /// </summary>
        /// <value></value>
        public string Usernameid { get; set; }
        public string CommentId { get; set; }
        public List<string> Followers { get; set; }

        public CommentNotification()
        {

        }
        public CommentNotification(string userid, string commentid, List<string> followers)
        {
           
            Usernameid = userid;
            CommentId = commentid;
            Followers = followers;
        }
    }
}
