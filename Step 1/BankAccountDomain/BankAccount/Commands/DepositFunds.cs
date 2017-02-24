using Its.Validation;
using Its.Validation.Configuration;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public void EnactCommand(DepositFunds command)
        {
            var created = new FundsDeposited {Amount = command.Amount};
            RecordEvent(created);
        }

        public class DepositFunds : Command<BankAccount>
        {
            public decimal Amount { get; set; }

            public override bool Authorize(BankAccount target) => true;

            /// <summary>
            /// Gets a validator to check the state of the command in and of itself, as distinct from an aggregate.
            /// </summary>
            /// <remarks>
            /// By default, this returns a Microsoft.Its.Validation.ValidationPlan`1 where TCommand is the command's actual type, with rules built up from any System.ComponentModel.DataAnnotations attributes applied to its properties.
            /// </remarks>
            public override IValidationRule CommandValidator
            {
                get
                {
                    return Validate.That<DepositFunds>(cmd => cmd.Amount > 0)
                        .WithErrorMessage("You cannot make a deposit for a negative amount.");
                }
            }
        }
    }
}