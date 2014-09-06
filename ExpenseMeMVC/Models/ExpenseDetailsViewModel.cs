using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseMeMVC.Models
{
    public class ExpenseDetailsViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Merchant Name")]
        public string MerchantName { get; set; }
        public List<LineItemDetails> LineItems { get; set; }
    }

    public class TransactionDetails
    {
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string MerchantName { get; set; }
        public string GlCodes { get; set; }
        public string TaxCode { get; set; }
        public string ExpenseType { get; set; }
    }

    public class LineItemDetails
    {
        public string TaxCode { get; set; }
        public string ExpenseType { get; set; }
        public string GlCodes { get; set; }
    }
}