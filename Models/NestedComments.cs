using System;
using System.Collections.Generic;

namespace GlobalModels
{
    /// <summary>
    /// DTO Model for comments backend -> nested comment frontend
    /// </summary>
    public class NestedComment : IEquatable<NestedComment>
    {
        public Guid Commentid { get; set; } = Guid.NewGuid();
        public Guid Discussionid { get; set; }
        public string Userid { get; set; }
        public string Text { get; set; }
        public bool Isspoiler { get; set; }
        public string ParentCommentid { get; set; }
        public int Likes { get; set; }
        public List<NestedComment> Replies { get; set; }
        public DateTime CreationTime { get; set; }
        
        public NestedComment(Guid commentid, Guid discussionid, string uid, string text, bool isspoiler, string parentcommentid, int likes, DateTime creationTime)
        {
            Commentid = commentid;
            Discussionid = discussionid;
            Userid = uid;
            Text = text;
            Isspoiler = isspoiler;
            ParentCommentid = parentcommentid;
            Replies = new List<NestedComment>();
            Likes = likes;
            CreationTime = creationTime;
        }

        public NestedComment(Guid commentid, Guid discussionid, string uid, string text, bool isspoiler, string parentcommentid, int likes)
        {
            Commentid = commentid;
            Discussionid = discussionid;
            Userid = uid;
            Text = text;
            Isspoiler = isspoiler;
            ParentCommentid = parentcommentid;
            Replies = new List<NestedComment>();
            Likes = likes;
        }

        public bool Equals(NestedComment other)
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
            return this.Equals(obj as NestedComment);
        }

        public static bool operator ==(NestedComment lhs, NestedComment rhs)
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

        public static int CompareLikes(NestedComment c1, NestedComment c2)

        {

            return c1.Likes.CompareTo(c2.Likes);

        }

        public static bool operator !=(NestedComment lhs, NestedComment rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Commentid.GetHashCode();
        }
    }
}