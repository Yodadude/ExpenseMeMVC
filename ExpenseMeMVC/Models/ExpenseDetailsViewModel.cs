using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseMeMVC.Models
{
    public class ExpenseDetailsViewModel
    {
        public TransactionDetails Transaction { get; set; }
        public List<LineItemDetails> LineItems { get; set; }
        public List<ExpenseTypeDetails> ExpenseTypeDetails { get; set; }
        public List<TaxCodeDetails> TaxCodeDetails { get; set; }
        public List<GlCodeDetails> GlCodeDetails { get; set; }
    }

    public class TransactionDetails
    {
        public int CardType { get; set; }
        public string ReferenceNumber { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string MerchantName { get; set; }
        public string Purpose { get; set; }
        public string ExpenseGroup { get; set; }
        public bool TaxReceipt { get; set; }
        public decimal ForeignAmount { get; set; }
        public string TransactionCurrencyType { get; set; }
    }

    public class LineItemDetails
    {
        public int LineId { get; set; }
        public string Description { get; set; }
        public string ExpenseType { get; set; }
        public string TaxCode { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string CurrencyType { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public List<GlCodeDetails> GlCodes { get; set; }
    }

    public class GlCodeDetails
    {
        public int SegmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MaxLength { get; set; }
        public int Width { get; set; }
        public string ValidationType { get; set; }
    }

    public class ExpenseTypeDetails
    {
        public string ExpenseType { get; set; }
        public string Description { get; set; }
        public string GlCode { get; set; }
    }

    public class TaxCodeDetails
    {
        public string TaxCode { get; set; }
        public decimal TaxPercentage { get; set; }
        public string TaxInclusive { get; set; }
    }
}