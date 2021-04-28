using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class UserLike
    {
        public string CommentId { get; set; }
        public string UserId { get; set; }

        public virtual Discussion Comment { get; set; }
    }
}
