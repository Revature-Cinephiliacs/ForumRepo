using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Repo Model from database-first scaffolding
    /// </summary>
    public partial class Setting
    {
        public string Setting1 { get; set; }
        public int? IntValue { get; set; }
        public string StringValue { get; set; }
    }
}
