using ExpenseMeMVC.Models;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft;
using ExpenseMeMVC.Handlers.Home;
using ExpenseMeMVC.Infrastructure;

namespace ExpenseMeMVC.Controllers
{
    public class HomeController : BaseController
    {
        private ISessionState _sessionState;

        public HomeController(ISessionState sessionState)
        {
            _sessionState = sessionState;
        }

        public ActionResult Index(BadgersQueryModel query)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("logon", "account");
            }

            var viewModel = Command.Invoke<BadgersQueryModel, BadgersViewModel>(query);

            return View(viewModel);
        }

        public ActionResult Badges(BadgersQueryModel query)
        {
            var viewModel = Command.Invoke<BadgersQueryModel, BadgersViewModel>(query);
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult GetExpenseDetailsViewModel()
        {
            //var info = GetTransactionAndLineItemDetails("Visa0000000000006109");
            var info = GetTransactionAndLineItemDetails("MasterCard0000027318");

            var vm = new ExpenseDetailsViewModel();

            vm.ExpenseTypeDetails = GetExpenseTypeDetals(info.First().CardType);
            vm.TaxCodeDetails = GetTaxCodeDetals();
            vm.GlCodeDetails = GetGlCodeDetals();

            vm.Transaction = info.Select(x => new TransactionDetails
            {
                CardType = x.CardType,
                ReferenceNumber = x.ReferenceNumber,
                TransactionDate = x.TransactionDate,
                MerchantName = x.MerchantName,
                Amount = x.Amount,
                Purpose = x.Purpose,
                ExpenseGroup = x.ExpenseGroup,
                TaxReceipt = x.TaxReceipt.Equals("Y")
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


            vm.LineItems.Add(new LineItemDetails
            {
                Description = "Line 2",
                ExpenseType = "",
                TaxCode = "GST",
                NetAmount = 10.00m,
                TaxAmount = 0,
                GrossAmount = 10.00m,
                Price = 10.00m,
                Quantity = 1,
                CurrencyType = "AUD",
                ExchangeRate = 1.0000m,
                GlCodes = vm.LineItems.First().GlCodes.ToList()
            });

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GlSearch(GlSearchQueryModel query)
        {
            var sql = @"select code as Code, description as Descrption from gl_segment_codes where segment_id = @0 order by description";

            var list = DataContext.Fetch<GlSearchResult>(sql, query.SegmentId).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UserExpenseGroups(UserExpenseGroupsQueryModel query)
        {
            var sql = @"select expense_group_name as ExpenseGroup, description as Descrption from user_expense_groups where user_name = @0 and active_flag = 'Y' order by 1";

            var list = DataContext.Fetch<UserExpenseGroupsResult>(sql, _sessionState.UserName).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
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

            return DataContext.Fetch<ExpenseTypeDetails>(sql, cardType).ToList();
        }

        private List<TaxCodeDetails> GetTaxCodeDetals()
        {
            var sql = @"select tax_code as TaxCode, description, tax_percentage as TaxPercentage, tax_inclusive as TaxInclusive from tax_codes where card_type = 0";

            return DataContext.Fetch<TaxCodeDetails>(sql).ToList();
        }

        private List<GlCodeDetails> GetGlCodeDetals()
        {
            var sql = @"select segment_id as SegmentId, name as Name, char_limit as MaxLength, width as Width, validation_type as ValidationType 
                          from gl_segment_defn where gl_type = (select gl_type from gl_type where status = 'active')";

            return DataContext.Fetch<GlCodeDetails>(sql).ToList();
        }

        private List<TransactionLineItemsQueryResult> GetTransactionAndLineItemDetails(string referenceNumber)
        {
            var sql = @"select d.card_type as CardType, d.reference_number as ReferenceNumber, 
                                d.effective_transaction_date as TransactionDate, d.amount, d.merchant_name as MerchantName, 
                                cl.commit_description as purpose, cl.expense_group_name as expenseGroup, cl.tax_receipt as taxReceipt,
                                g.gl_code as GlCodes, g.tax_code as TaxCode, g.expense_type as ExpenseType,
                                g.price, g.quantity, g.currency_type as CurrencyType, g.exchange_rate as ExchangeRate, 
                                g.net_amount as NetAmount , g.tax_amount as TaxAmount, g.amount as GrossAmount, g.fbt_id as LineId
                        from statement_data d
                        inner join statement_gl_line g on g.card_type = d.card_type and g.reference_number = d.reference_number
                        left outer join commitment_log cl on cl.commitment_id = d.commitment_id
                         where d.reference_number = @0";

            return DataContext.Fetch<TransactionLineItemsQueryResult>(sql, referenceNumber).ToList();
        }

        private class TransactionLineItemsQueryResult
        {
            public int CardType { get; set; }
            public string ReferenceNumber { get; set; }
            public DateTime TransactionDate { get; set; }
            public decimal Amount { get; set; }
            public string MerchantName { get; set; }
            public string Purpose { get; set; }
            public string ExpenseGroup { get; set; }
            public string TaxReceipt { get; set; }
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
    }
}