using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
       
        public string Usernameid { get; set; }
        public string DiscussionId { get; set; }
        public string CommentId { get; set; }
        public List<string> Followers { get; set; }
        static HttpClient client = new HttpClient();
        public CommentNotification()
        {

        }
        public CommentNotification(string userid, string discussionid, string commentid, List<string> followers)
        {
           
            Usernameid = userid;
            DiscussionId = discussionid;
            CommentId = commentid;
            Followers = followers;
        }

        /// <summary>
        /// posts notifications to User Api on given url.
        /// return the Uri of successful post action
        /// </summary>
        /// <value></value>

        public async Task<Uri> SendNotification()
        {
            CommentNotification cn = new();
            
            cn.Usernameid = this.Usernameid;
            cn.DiscussionId = this.DiscussionId;
            cn.CommentId = this.CommentId;
            cn.Followers = this.Followers;
           

            HttpResponseMessage response = await client.PostAsJsonAsync(
        "api/User/notification/comment", cn);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
    }
}
