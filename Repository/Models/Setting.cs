using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Model from database-first (backend model)
    /// </summary>
    public partial class Setting
    {
        public string Setting1 { get; set; }
        public int? IntValue { get; set; }
        public string StringValue { get; set; }
    }
}
