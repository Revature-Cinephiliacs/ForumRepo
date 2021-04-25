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
        [StringLength(50)]
        public string Userid { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; }

        [Required]
        public bool Isspoiler { get; set; }
        public NewComment()
        {
            
        }
        public NewComment(Guid discussionid, string uid, string text, bool isspoiler)
        {
            Discussionid = discussionid;
            Userid = uid;
            Text = text;
            Isspoiler = isspoiler;
        }
        public NewComment(Comment comment)
        {
            Discussionid = comment.Discussionid;
            Userid = comment.Userid;
            Text = comment.Text;
            Isspoiler = comment.Isspoiler;
        }
    }
}