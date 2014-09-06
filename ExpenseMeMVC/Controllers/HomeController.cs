using ExpenseMeMVC.Models;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            var data = GetTransactionDetails();

            var tranDetails = data.First();
            var lineItems = new List<LineItemDetails>();

            var model = new ExpenseDetailsViewModel
            {
                TransactionDate = tranDetails.TransactionDate,
                Amount = tranDetails.Amount,
                MerchantName = tranDetails.MerchantName,
                LineItems = lineItems
            };

            return View(model);
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

        private List<TransactionDetails> GetTransactionDetails()
        {
            var sql = @"select d.effective_transaction_date as TransactionDate, d.amount, d.merchant_name as MerchantName, 
                               g.gl_code as GlCodes, g.tax_code as TaxCode, g.expense_type as ExpenseType
                        from statement_data d
                        inner join statement_gl_line g on g.card_type = d.card_type and g.reference_number = d.reference_number
                         where d.reference_number = @0";

            return database.Fetch<TransactionDetails>(sql, "Visa0000000000006109").ToList();
        }
    }
}