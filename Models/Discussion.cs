using System;

namespace GlobalModels
{
    public sealed class Discussion : IEquatable<Discussion>
    {
        public int Discussionid { get; set; }
        public string Movieid { get; set; }
        public string Username { get; set; }
        public string Subject { get; set; }
        public string Topic { get; set; }

        public Discussion(int discussionid, string movieid, string username, string subject, string topic)
        {
            Discussionid = discussionid;
            Movieid = movieid;
            Username = username;
            Subject = subject;
            Topic = topic;
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
            return Discussionid;
        }
    }
}