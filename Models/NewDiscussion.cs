using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalModels
{
    /// <summary>
    /// DTO Model for frontend -> backend
    /// </summary>
    public sealed class NewDiscussion
    {
        [Required]
        [StringLength(20)]
        public string Movieid { get; set; }

        [Required]
        [StringLength(50)]
        public string Userid { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [StringLength(25)]
        public string Topic { get; set; }
        public NewDiscussion()
        {
            
        }

        public NewDiscussion(string movieid, string uid, string subject, string topic)
        {
            Movieid = movieid;
            Userid = uid;
            Subject = subject;
            Topic = topic;
        }
        public NewDiscussion(Discussion discussion)
        {
            Movieid = discussion.Movieid;
            Userid = discussion.Userid;
            Subject = discussion.Subject;
            Topic = discussion.Topic;
        }
    }
}