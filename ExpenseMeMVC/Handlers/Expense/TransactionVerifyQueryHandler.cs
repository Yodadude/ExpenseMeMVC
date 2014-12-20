using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpenseMeMVC.Infrastructure;
using Newtonsoft.Json;
using ExpenseMeMVC.Models;

namespace ExpenseMeMVC.Handlers.Expense
{
    public class TransactionVerifyQueryHandler : ICommandHandler<TransactionVerifyQueryModel, TransactionVerifyViewModel>
    {
        private IDatabase _database;

        public TransactionVerifyQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public TransactionVerifyViewModel Handle(TransactionVerifyQueryModel input)
        {

            var model = new TransactionVerifyViewModel();

            var list = GetTransactionInfo(input.TransactionId);

            model.Transaction = list.Select(x => new TransactionDetails
            {
                CardType = x.CardType,
                ReferenceNumber = x.ReferenceNumber,
                TransactionDate = x.TransactionDate,
                MerchantName = x.MerchantName,
                Amount = x.Amount,
                Purpose = x.Purpose,
                ExpenseGroup = x.ExpenseGroup,
                TaxReceipt = x.TaxReceipt.Equals("Y"),
                ForeignAmount = x.ForeignAmount,
                TransactionCurrencyType = x.TransactionCurrencyType
            }).First();

            model.LineItems = list.Select(x => new LineItemDetails
            {
                LineId = x.LineId,
                Description = x.Description,
                ExpenseType = x.ExpenseType,
                TaxCode = x.TaxCode,
                NetAmount = x.NetAmount,
                TaxAmount = x.TaxAmount,
                GrossAmount = x.GrossAmount,
                Price = x.Price,
                Quantity = x.Quantity,
                CurrencyType = x.CurrencyType,
                ExchangeRate = x.ExchangeRate,
                GlCodes = ConvertGlCodesToList(x.GlCodes)
            }).ToList();

            //model.ModelAsJson = JsonConvert.SerializeObject(model);

            return model;
        }

        private List<GlSegmentDefinition> GetGlSegmentDefinitions()
        {
            var sql = @"select segment_id, name from gl_segment_defn where gl_type = (select gl_type from gl_type where status='active')";

            return _database.Fetch<GlSegmentDefinition>(sql).ToList();
        }

        private List<TransactionQueryResult> GetTransactionInfo(string transactionId)
        {
            var sql = @"
            select d.card_type as CardType, d.reference_number as ReferenceNumber, 
                d.effective_transaction_date as TransactionDate, d.amount, d.merchant_name as MerchantName, 
                d.original_currency_code as TransactionCurrencyType, d.original_currency_amount as ForeignAmount,
                cl.commitment_id as CommitmentId, cl.commit_description as purpose, cl.expense_group_name as expenseGroup, cl.tax_receipt as TaxReceipt,
                g.gl_code as GlCodes, g.tax_code as TaxCode, g.expense_type as ExpenseType,
                g.price, g.quantity, g.currency_type as CurrencyType, g.exchange_rate as ExchangeRate, 
                g.net_amount as NetAmount , g.tax_amount as TaxAmount, g.amount as GrossAmount, g.fbt_id as LineId
            from statement_data d
            inner join statement_gl_line g on g.card_type = d.card_type and g.reference_number = d.reference_number
            left outer join commitment_log cl on cl.commitment_id = d.commitment_id
            where d.reference_number = @0";

            return _database.Fetch<TransactionQueryResult>(sql, transactionId).ToList();
        }

        private List<GlCodeDetails> ConvertGlCodesToList(string glCodes)
        {
            var glCodesArray = glCodes.Split('\t');

            return GetGlSegmentDefinitions().Select(x => new GlCodeDetails
                {
                    SegmentId = x.SegmentId,
                    Code = glCodesArray[x.SegmentId - 1],
                    Name = x.Name
                }).ToList();

        }
    }

    public class TransactionVerifyQueryModel
    {
        public string TransactionId { get; set; }
    }

    public class TransactionQueryResult
    {
        public int CardType { get; set; }
        public string ReferenceNumber { get; set; }
        public string CommitmentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string MerchantName { get; set; }
        public string Purpose { get; set; }
        public string ExpenseGroup { get; set; }
        public string TaxReceipt { get; set; }
        public string TransactionCurrencyType { get; set; }
        public decimal ForeignAmount { get; set; }
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
        public string GlCodes { get; set; }
    }

    public class TransactionVerifyViewModel
    {
        public TransactionDetails Transaction { get; set; }
        public List<LineItemDetails> LineItems { get; set; }
    }

    public class GlSegmentDefinition
    {
        public int SegmentId { get; set; }
        public string Name { get; set; }
    }
}