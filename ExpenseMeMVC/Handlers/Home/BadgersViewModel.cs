using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpenseMeMVC.Handlers.Home
{
    public class BadgersViewModel
    {
        public int ApprovalCount { get; set; }
        public int TransactionCount { get; set; }
        public bool IsApprover { get; set; }
        public bool HasClaimAccount { get; set; }
    }
}
