using Its.Validation;
using Its.Validation.Configuration;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public void EnactCommand(CloseAccount command)
        {
            var created = new AccountClosed();
            RecordEvent(created);
        }

        public class CloseAccount : Command<BankAccount>
        {
            public override IValidationRule<BankAccount> Validator
            {
                get
                {
                    var accountIsNotClosed =
                        Validate.That<BankAccount>(account => account.AccountStatus != AccountStatuses.Closed)
                            .WithErrorMessage("Account is already closed.");

                    return new ValidationPlan<BankAccount>
                    {
                        accountIsNotClosed
                    };
                }
            }
        }
    }
}