using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public void EnactCommand(Withdraw command)
        {
            var created = new Withdrawed() {Amount = command.Amount};
            RecordEvent(created);
        }

        public class Withdraw : Command<BankAccount>
        {
            public decimal Amount { get; set; }

            public override bool Authorize(BankAccount target) => true;
        }
    }
}