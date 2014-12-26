using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpenseMeMVC.Infrastructure;
using NPoco;

namespace ExpenseMeMVC.Handlers.Home
{
    public class BadgersQueryHandler : ICommandHandler<BadgersQueryModel, BadgersViewModel>
    {
        private IDatabase _database;
        private ISessionState _sessionState;

        public BadgersQueryHandler(IDatabase database, ISessionState sessionState)
        {
            _database = database;
            _sessionState = sessionState;
        }

        public BadgersViewModel Handle(BadgersQueryModel input)
        {
            var sql = @"
            select count(*) from statement_data sd 
            inner join wf_instance_status wi on wi.instance_id = sd.reference_number
            where sd.owner_user_name = @0
              and wi.activity_id = 3";

            var transactionCount = _database.Single<int>(sql, _sessionState.UserName);

            sql = @"
            select count(*) from wf_instance_status wi
            inner join security_access sa on sa.name = wi.security_name and sa.group_name = wi.security_group_name
            where activity_id in (select activity_id from wf_activities where activity_type=104)
            and sa.user_name = @0
            and wi.object_type in ('T')";

            var approvalCount = _database.Single<int>(sql, _sessionState.UserName);

            return new BadgersViewModel
            { 
                ApprovalCount = approvalCount,
                TransactionCount = transactionCount,
                IsApprover = true,
                HasClaimAccount = true
            };
        }
    }
}