using ExpenseMeMVC.Models;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft;

namespace ExpenseMeMVC.Controllers
{
    public class HomeController : Controller
    {
        private Database database;

        public HomeController()
        {
            database = new Database(@"Server=(local)\sqlexpress;Database=pm99_mivision;User Id=promasterweb; Password=changeoninstall;", DatabaseType.SqlServer2012);
        }

        public ActionResult Index()
        {
            return View(new ExpenseDetailsViewModel());
        }

        [HttpGet]
        public ActionResult GetExpenseDetailsViewModel()
        {
            var info = GetTransactionAndLineItemDetails("Visa0000000000006109");

            var vm = new ExpenseDetailsViewModel();

            vm.ExpenseTypeDetails = GetExpenseTypeDetals(info.First().CardType);
            vm.TaxCodeDetails = GetTaxCodeDetals();
            vm.GlCodeDetails = GetGlCodeDetals();

            vm.Transaction = info.Select(x => new TransactionDetails
            {
                CardType = x.CardType,
                ReferenceNumber = x.ReferenceNumber,
                TransactionDate = x.TransactionDate.ToShortDateString(),
                MerchantName = x.MerchantName,
                Amount = x.Amount
            }).First();

            vm.LineItems = info.Select(x => new LineItemDetails
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
                GlCodes = ConvertGlCodesToList(x.GlCodes, vm.GlCodeDetails)
            }).ToList();


            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private List<GlCodeDetails> ConvertGlCodesToList(string glCodes, List<GlCodeDetails> codeDetails)
        {
            var codes = new List<GlCodeDetails>();
            var glCodes2 = glCodes.Split('\t');

            foreach (var item in codeDetails)
            {
                codes.Add(new GlCodeDetails
                {
                    SegmentId = item.SegmentId,
                    Code = glCodes2[item.SegmentId - 1],
                    Name = item.Name
                });
            };

            return codes;
        }

        private List<ExpenseTypeDetails> GetExpenseTypeDetals(int cardType)
        {
            var sql = @"select expense_type as ExpenseType, description, gl_code as GlCodes from expense_types 
                        where exists (select 1 from account_expense_types a where a.card_type = @0 and a.expense_type = expense_types. expense_type)";

            return database.Fetch<ExpenseTypeDetails>(sql, cardType).ToList();
        }

        private List<TaxCodeDetails> GetTaxCodeDetals()
        {
            var sql = @"select tax_code as TaxCode, description, tax_percentage as TaxPercentage, tax_inclusive as TaxInclusive from tax_codes where card_type = 0";

            return database.Fetch<TaxCodeDetails>(sql).ToList();
        }

        private List<GlCodeDetails> GetGlCodeDetals()
        {
            var sql = @"select segment_id as SegmentId, name as Name, char_limit as MaxLength, width as Width, validation_type as ValidationType 
                          from gl_segment_defn where gl_type = (select gl_type from gl_type where status = 'active')";

            return database.Fetch<GlCodeDetails>(sql).ToList();
        }

        private List<TransactionLineItemsQueryResult> GetTransactionAndLineItemDetails(string referenceNumber)
        {
            var sql = @"select d.card_type as CardType, d.reference_number as ReferenceNumber, 
                                d.effective_transaction_date as TransactionDate, d.amount, d.merchant_name as MerchantName, 
                                g.gl_code as GlCodes, g.tax_code as TaxCode, g.expense_type as ExpenseType,
                                g.price, g.quantity, g.currency_type as CurrencyType, g.exchange_rate as ExchangeRate, 
                                g.net_amount as NetAmount , g.tax_amount as TaxAmount, g.amount as GrossAmount, g.fbt_id as LineId
                        from statement_data d
                        inner join statement_gl_line g on g.card_type = d.card_type and g.reference_number = d.reference_number
                         where d.reference_number = @0";

            return database.Fetch<TransactionLineItemsQueryResult>(sql, referenceNumber).ToList();
        }

        private class TransactionLineItemsQueryResult
        {
            public int CardType { get; set; }
            public string ReferenceNumber { get; set; }
            public DateTime TransactionDate { get; set; }
            public decimal Amount { get; set; }
            public string MerchantName { get; set; }
            public int LineId { get; set; }
            public string Description { get; set; }
            public string ExpenseType { get; set; }
            public string TaxCode { get; set; }
            public decimal Price { get; set; }
            public decimal Quantity { get; set; }
            public string CurrencyType { get; set; }
            public decimal ExchangeRate { get; set; }
            public bool TaxReceipt { get; set; }
            public decimal NetAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal GrossAmount { get; set; }
            public string GlCodes { get; set; }
        }
    }
}