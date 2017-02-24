using System;
using System.Linq;
using Microsoft.Its.Domain;
using Xunit;

namespace BankAccountDomain.Tests
{
    public class BankAccountUnitTests
    {
        public BankAccountUnitTests()
        {
            account = new BankAccount(new BankAccount.OpenAccount {CustomerId = new CustomerId("Xavier")});
        }

        private readonly BankAccount account;
        [Fact]
        public void Can_open_BankAccount()
        {
            Assert.Equal(0.0m, account.Balance);
            Assert.Equal(BankAccount.AccountStatuses.Open, account.AccountStatus);
        }

        [Fact]
        public void Can_close_BankAccount()
        {
            Assert.Equal(0.0m, account.Balance);
            Assert.Equal(BankAccount.AccountStatuses.Open, account.AccountStatus);
            account.Apply(new BankAccount.CloseAccount());
            Assert.Equal(BankAccount.AccountStatuses.Closed, account.AccountStatus);
        }

        [Fact]
        public void Can_deposit_and_withdraw_money_into_BankAccount()
        {
            var depositAmount = 10.0m;
            account.Apply(new BankAccount.DepositFunds
            {
                Amount = depositAmount
            });
            Assert.Equal(10.0m, account.Balance);

            var withdrawAmount = 5.0m;
            account.Apply(new BankAccount.WithdrawFunds
            {
                Amount = withdrawAmount
            });
            Assert.Equal(5.0m, account.Balance);
        }

        [Fact]
        public void Cannot_close_or_withdraw_from_closed_account()
        {
            account.Apply(new BankAccount.CloseAccount());
            Assert.Equal(BankAccount.AccountStatuses.Closed, account.AccountStatus);

            Action testCode = () => account.Apply(new BankAccount.CloseAccount());
            var exception = Assert.Throws<CommandValidationException>(testCode);
            Assert.Equal("Account is already closed.",
                exception.ValidationReport.Failures.First().Message);

            testCode = () => account.Apply(new BankAccount.WithdrawFunds
            {
                Amount = 10.0m
            });
            exception = Assert.Throws<CommandValidationException>(testCode);
            Assert.Equal("You cannot make a withdrawal from a closed account.",
                exception.ValidationReport.Failures.First().Message);
        }

        [Fact]
        public void Cannot_deposit_negative_amount()
        {
            var depositAmount = -10.0m;
            Action testCode = () => account.Apply(new BankAccount.DepositFunds
            {
                Amount = depositAmount
            });
            var exception = Assert.Throws<CommandValidationException>(testCode);
            Assert.Equal("You cannot make a deposit for a negative amount.",
                exception.ValidationReport.Failures.First().Message);
        }
    }
}