using System;
using System.Threading.Tasks;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public class CalculateAndDepositInterestDaily : Command<BankAccount>
    {
    }

    public class CalculateAndDepositInterestDailyCommandHandler :
        ICommandHandler<BankAccount, CalculateAndDepositInterestDaily>
    {
        private readonly ICommandScheduler<BankAccount> _scheduler;

        public CalculateAndDepositInterestDailyCommandHandler(ICommandScheduler<BankAccount> scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
            _scheduler = scheduler;
        }

        public async Task EnactCommand(BankAccount target, CalculateAndDepositInterestDaily command)
        {
            if (target.Balance == 0) return;
            // 5% annual interest
            var interestRate = 0.05m / 365;
            var interest = interestRate * target.Balance;
            await target.ApplyAsync(new BankAccount.DepositInterest()
            {
                Amount = interest
            });
            await _scheduler.Schedule(target.Id,
                new CalculateAndDepositInterestDaily(), Clock.Now().AddDays(1));
        }

        public Task HandleScheduledCommandException(BankAccount target,
            CommandFailed<CalculateAndDepositInterestDaily> command)
        {
            return Task.CompletedTask;
        }
    }
}