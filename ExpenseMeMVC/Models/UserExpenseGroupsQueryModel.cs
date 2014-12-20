using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseMeMVC.Models
{
    public class UserExpenseGroupsQueryModel
    {
    }

    public class UserExpenseGroupsResult
    {
        public string ExpenseGroup { get; set; }
        public string Description { get; set; }
    }
}