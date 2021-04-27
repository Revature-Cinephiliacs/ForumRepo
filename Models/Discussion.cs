using System;
using System.Collections.Generic;

namespace GlobalModels
{
    /// <summary>
    /// DTO Model for frontend -> backend
    /// </summary>
    public sealed class Discussion : IEquatable<Discussion>
    {
        public Guid Discussionid { get; set; } = Guid.NewGuid();
        public string Movieid { get; set; }
        public string Userid { get; set; }
        public string Subject { get; set; }
        public string Topic { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public Discussion(Guid discussionid, string movieid, string uid, string subject, string topic, ICollection<Comment> comments)
        {
            Discussionid = discussionid;
            Movieid = movieid;
            Userid = uid;
            Subject = subject;
            Topic = topic;
            Comments = comments;

        }

        public bool Equals(Discussion other)
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

            return Discussionid == other.Discussionid;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Discussion);
        }

        public static bool operator ==(Discussion lhs, Discussion rhs)
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

        public static bool operator !=(Discussion lhs, Discussion rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Discussionid.GetHashCode();
        }
    }
}