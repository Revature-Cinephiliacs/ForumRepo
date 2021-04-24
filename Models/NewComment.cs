using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalModels
{
    /// <summary>
    /// DTO Model for frontend -> backend
    /// </summary>
    public sealed class NewComment
    {
        [Required]
        public Guid Discussionid { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; }

        [Required]
        public bool Isspoiler { get; set; }
        public NewComment()
        {
            
        }

        public NewComment(Guid discussionid, string username, string text, bool isspoiler)
        {
            Discussionid = discussionid;
            Username = username;
            Text = text;
            Isspoiler = isspoiler;
        }
        public NewComment(Comment comment)
        {
            Discussionid = comment.Discussionid;
            Username = comment.Username;
            Text = comment.Text;
            Isspoiler = comment.Isspoiler;
        }
    }
}