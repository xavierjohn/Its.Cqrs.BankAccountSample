using System;
using System.Collections.Generic;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount : EventSourcedAggregate<BankAccount>
    {
        public BankAccount(OpenAccount openAccount) : base(openAccount)
        {
        }
        public BankAccount(Guid id, IEnumerable<IEvent> eventHistory) : base(id, eventHistory)
        {
        }

        public decimal Balance { get; private set; }
        public AccountStatuses AccountStatus { get; private set; }

        public enum AccountStatuses
        {
            Open,
            Closed
        }
    }
}