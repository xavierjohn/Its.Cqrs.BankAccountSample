using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public void EnactCommand(Create command)
        {
            var created = new Created {CustomerId = command.CustomerId};
            RecordEvent(created);
        }

        public class Create : ConstructorCommand<BankAccount>
        {
            public CustomerId CustomerId { get; set; }

            public override bool Authorize(BankAccount target) => true;
        }
    }
}