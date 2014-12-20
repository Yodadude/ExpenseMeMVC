using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExpenseMeMVC.Handlers.Home;
using ExpenseMeMVC.Handlers.Expense;

namespace ExpenseMeMVC.Controllers
{
    public class ExpenseController : BaseController
    {
        // GET: Expense
        public ActionResult Index(BadgersQueryModel query)
        {
            var viewModel = Command.Invoke<BadgersQueryModel, BadgersViewModel>(query);

            return View(viewModel);
        }

        public ActionResult Transactions(TransactionsQueryModel query)
        {
            var viewModel = Command.Invoke<TransactionsQueryModel, TransactionsViewModel>(query);

            return View(viewModel);
        }

        public ActionResult Transaction(string transactionId)
        {
            return View();
        }

        public ActionResult Verify(TransactionVerifyQueryModel query)
        {
            var viewModel = Command.Invoke<TransactionVerifyQueryModel, TransactionVerifyViewModel>(query);

            return View(viewModel);
        }
    }
}