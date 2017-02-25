using System;
using System.Threading.Tasks;
using BankAccountDomain;
using Microsoft.Its.Domain;
using Microsoft.Its.Domain.Sql;
using Xunit;

namespace BankAccountPersistence.Tests
{
    public class BankAccountPersistenceUnitTests : IDisposable
    {
        IEventSourcedRepository<BankAccount> _repository;
        BankAccount _bankAccount;

        public BankAccountPersistenceUnitTests()
        {
            Command<BankAccount>.AuthorizeDefault = (target, command) => true;
            var connectionString =
                @"Data Source=(localdb)\MSSQLLocalDB; Integrated Security=True; Initial Catalog=BankEventStore";
            var configuration = new Configuration();
            configuration.UseSqlEventStore(c => c.UseConnectionString(connectionString));
            _repository = configuration.Repository<BankAccount>();
            Guid bankAccountAggregateId = new Guid("0a59517e-f69d-46b4-aa24-45aea90f0012");
            _bankAccount = _repository.GetLatest(bankAccountAggregateId).Result;
            if (_bankAccount == null)
            {
                _bankAccount =
                    new BankAccount(new BankAccount.OpenAccount(bankAccountAggregateId) {CustomerId = new CustomerId("Xavier")});
            }
        }


        [Fact]
        public async Task CanDepositFunds()
        {
            var balance = _bankAccount.Balance;
            await _bankAccount.ApplyAsync(new BankAccount.DepositFunds
            {
                Amount = 10.0m
            });
            var expected = balance + 10.0m;
            Assert.Equal(expected, _bankAccount.Balance);
        }

        [Fact]
        public async Task CanWithdrawFunds()
        {
            var balance = _bankAccount.Balance;
            await _bankAccount.ApplyAsync(new BankAccount.WithdrawFunds()
            {
                Amount = 5.0m
            });
            var expected = balance - 5.0m;
            Assert.Equal(expected, _bankAccount.Balance);
        }

        public void Dispose()
        {
            var task = _repository.Save(_bankAccount);
            task.Wait();
        }
    }
}
