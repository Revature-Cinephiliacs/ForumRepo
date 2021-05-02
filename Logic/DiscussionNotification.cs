using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        
        static HttpClient client = new HttpClient();
    
        public DiscussionNotification(string movieid, string userid, string discussionid)
        {
            Movieid = movieid;
            Userid = userid;
            Discussionid = discussionid;
        }

        public DiscussionNotification()
        {
        }
        /// <summary>
        /// posts notifications to Movie Api on given url.
        /// return the Uri of successful post action
        /// </summary>
        /// <value></value>
        public async Task<Uri> SendNotification()
        {
            DiscussionNotification dn = new();
            dn.Movieid = this.Movieid;
            dn.Userid = this.Userid;
            dn.Discussionid = this.Discussionid;

            HttpResponseMessage response = await client.PostAsJsonAsync(
        "api/Movie/notification/discussion", dn);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

    }
}
