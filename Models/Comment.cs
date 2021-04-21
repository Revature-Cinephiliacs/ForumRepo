using System;

namespace GlobalModels
{
    public sealed class Comment : IEquatable<Comment>
    {
        public int Commentid { get; set; }
        public int Discussionid { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public bool Isspoiler { get; set; }

        public Comment(int commentid, int discussionid, string username, string text, bool isspoiler)
        {
            Commentid = commentid;
            Discussionid = discussionid;
            Username = username;
            Text = text;
            Isspoiler = isspoiler;
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
            return Commentid;
        }
    }
}