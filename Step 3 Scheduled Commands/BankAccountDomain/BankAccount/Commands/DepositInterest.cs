using System;
using System.Threading.Tasks;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public class DepositInterest : Command<BankAccount>
        {
            public decimal Amount { get; set; }
        }

        public class DepositInterestCommandHandler : ICommandHandler<BankAccount, DepositInterest>
        {
            private readonly ICommandScheduler<BankAccount> _scheduler;

            public DepositInterestCommandHandler(ICommandScheduler<BankAccount> scheduler)
            {
                if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
                _scheduler = scheduler;
            }

            public async Task EnactCommand(BankAccount target, DepositInterest command)
            {
                var created = new InterestDeposited {Amount = command.Amount};
                target.RecordEvent(created);
            }

            public Task HandleScheduledCommandException(BankAccount target, CommandFailed<DepositInterest> command)
            {
                return Task.CompletedTask;
            }
        }
    }
}