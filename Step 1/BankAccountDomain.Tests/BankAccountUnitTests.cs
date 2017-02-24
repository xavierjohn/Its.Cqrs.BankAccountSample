using Microsoft.Its.Domain;
using Xunit;

namespace BankAccountDomain.Tests
{
    public class BankAccountUnitTests
    {
        [Fact]
        public void CanCreateBankAccount()
        {
            var account = new BankAccount(new BankAccount.Create {CustomerId = new CustomerId("Xavier")});

            Assert.Equal(0.0m, account.Balance);
        }

        [Fact]
        public void CanDepositAndWithdrawMoneyIntoBankAccount()
        {
            var account = new BankAccount(new BankAccount.Create {CustomerId = new CustomerId("Xavier")});
            var depositAmount = 10.0m;
            account.Apply(new BankAccount.Deposit
            {
                Amount = depositAmount
            });
            Assert.Equal(10.0m, account.Balance);

            var withdrawAmount = 5.0m;
            account.Apply(new BankAccount.Withdraw
            {
                Amount = withdrawAmount
            });
            Assert.Equal(5.0m, account.Balance);
        }
    }
}