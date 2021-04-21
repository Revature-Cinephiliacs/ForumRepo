using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalModels
{
    public sealed class NewDiscussion
    {
        [Required]
        [StringLength(20)]
        public string Movieid { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [StringLength(25)]
        public string Topic { get; set; }
        public NewDiscussion()
        {
            
        }

        public NewDiscussion(string movieid, string username, string subject, string topic)
        {
            Movieid = movieid;
            Username = username;
            Subject = subject;
            Topic = topic;
        }
        public NewDiscussion(Discussion discussion)
        {
            Movieid = discussion.Movieid;
            Username = discussion.Username;
            Subject = discussion.Subject;
            Topic = discussion.Topic;
        }
    }
}