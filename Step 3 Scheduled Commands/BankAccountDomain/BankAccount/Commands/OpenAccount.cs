using System;
using System.Threading.Tasks;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public class OpenAccount : ConstructorCommand<BankAccount>
        {
            public OpenAccount(Guid aggregateId) : base(aggregateId)
            {
            }

            public CustomerId CustomerId { get; set; }
        }

        public class OpenAccountCommandHandler : ICommandHandler<BankAccount, OpenAccount>
        {
            private readonly ICommandScheduler<BankAccount> _scheduler;

            public OpenAccountCommandHandler(ICommandScheduler<BankAccount> scheduler)
            {
                if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
                _scheduler = scheduler;
            }

            public async Task EnactCommand(BankAccount target, OpenAccount command)
            {
                var created = new AccountOpened {CustomerId = command.CustomerId};
                target.RecordEvent(created);
                await _scheduler.Schedule(target.Id,
                    new CalculateAndDepositInterestDaily(), Clock.Now().AddDays(1));
            }

            public Task HandleScheduledCommandException(BankAccount target, CommandFailed<OpenAccount> command)
            {
                return Task.CompletedTask;
            }
        }
    }
}