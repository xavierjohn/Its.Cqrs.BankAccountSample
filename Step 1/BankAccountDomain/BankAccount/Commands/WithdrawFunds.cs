using Its.Validation;
using Its.Validation.Configuration;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public void EnactCommand(WithdrawFunds command)
        {
            var created = new FundsWithdrawed {Amount = command.Amount};
            RecordEvent(created);
        }

        public class WithdrawFunds : Command<BankAccount>
        {
            public decimal Amount { get; set; }

            public override IValidationRule<BankAccount> Validator
            {
                get
                {
                    var accountIsNotClosed =
                        Validate.That<BankAccount>(account => account.AccountStatus == AccountStatuses.Open)
                            .WithErrorMessage("You cannot make a withdrawal from a closed account.");

                    var fundsAreAvailable = Validate.That<BankAccount>(account => account.Balance >= Amount)
                        .WithErrorMessage("Insufficient funds.");

                    return new ValidationPlan<BankAccount>
                    {
                        accountIsNotClosed,
                        fundsAreAvailable.When(accountIsNotClosed)
                    };
                }
            }

            public override IValidationRule CommandValidator
            {
                get
                {
                    return Validate.That<WithdrawFunds>(cmd => cmd.Amount > 0)
                        .WithErrorMessage("You cannot make a withdrawal for a negative amount.");
                }
            }
        }
    }
}