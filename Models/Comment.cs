using System;

namespace GlobalModels
{
    /// <summary>
    /// DTO Model for frontend -> backend
    /// </summary>
    public sealed class Comment : IEquatable<Comment>
    {
        public Guid Commentid { get; set; } = Guid.NewGuid();
        public Guid Discussionid { get; set; }
        public string Userid { get; set; }
        public string Text { get; set; }
        public bool Isspoiler { get; set; }
        public string ParentCommentid { get; set; }
        public int Likes { get; set; }

        public Comment(Guid commentid, Guid discussionid, string uid, string text, bool isspoiler, string parentcommentid, int likes)
        {
            Commentid = commentid;
            Discussionid = discussionid;
            Userid = uid;
            Text = text;
            Isspoiler = isspoiler;
            ParentCommentid = parentcommentid;
            Likes = likes;
        }

        public bool Equals(Comment other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return Commentid == other.Commentid;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Comment);
        }

        public static bool operator ==(Comment lhs, Comment rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Comment lhs, Comment rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Commentid.GetHashCode();
        }
    }
}