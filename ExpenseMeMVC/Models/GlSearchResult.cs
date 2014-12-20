using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseMeMVC.Models
{
    public class GlSearchQueryModel
    {
        public int SegmentId { get; set; }
        public string Code { get; set; }
        public string Parameters { get; set; }
    }

    public class GlSearchResult
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}