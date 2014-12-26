using ExpenseMeMVC.Infrastructure;
using ExpenseMeMVC.Models;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseMeMVC.Handlers.Expense
{
    public class TransactionsQueryHandler : ICommandHandler<TransactionsQueryModel, TransactionsViewModel>
    {
        private IDatabase _database;
        private ISessionState _sessionState;

        public TransactionsQueryHandler(IDatabase database, ISessionState sessionState)
        {
            _database = database;
            _sessionState = sessionState;
        }

        public TransactionsViewModel Handle(TransactionsQueryModel input)
        {
            var sql = @"
            select reference_number as TransactionId, effective_transaction_date as TransactionDate, merchant_name as MerchantName, amount as Amount, original_currency_amount as ForeignAmount, original_currency_code as CurrencyType
            from statement_data sd 
            inner join wf_instance_status wi on wi.instance_id = sd.reference_number
            where sd.owner_user_name = @0
              and wi.activity_id = 3";

            var list = _database.Fetch<TransactionDetails>(sql, _sessionState.UserName);

            return new TransactionsViewModel { List = list };
        }
    }

    public class TransactionsQueryModel
    {

    }

    public class TransactionsViewModel
    {
        public List<TransactionDetails> List { get; set; }
    }

}