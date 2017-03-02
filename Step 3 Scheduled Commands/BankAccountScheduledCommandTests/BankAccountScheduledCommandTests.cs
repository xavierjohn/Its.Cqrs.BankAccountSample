using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BankAccountDomain;
using Microsoft.Its.Domain;
using Microsoft.Its.Domain.Sql;
using Microsoft.Its.Domain.Sql.CommandScheduler;
using Xunit;
using Clock = Microsoft.Its.Domain.Clock;

namespace BankAccountScheduledCommandTests
{
    public static class TestSettings
    {
        public static string CustomClockName = "TestClock";

        public static string ConnectionStringEventStore =
            @"Data Source=(localdb)\MSSQLLocalDB; Integrated Security=True; Initial Catalog=BankEventStore";

        public static string ConnectionStringCommandScheduler =
            @"Data Source=(localdb)\MSSQLLocalDB; Integrated Security=True; Initial Catalog=BankCommandScheduler";

        public static Guid BankAccountAggregateId = new Guid("0a59517e-f69d-46b4-aa24-45aea90f0012");
    }

    public sealed class CleanUpDatabaseFixture
    {
        public CleanUpDatabaseFixture()
        {
            try
            {
                using (var con = new SqlConnection(TestSettings.ConnectionStringCommandScheduler))
                {
                    con.Open();
                    using (
                        var command =
                            new SqlCommand(
                                $"delete from [Scheduler].[Clock] where [Name]='{TestSettings.CustomClockName}'",
                                con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                using (var con = new SqlConnection(TestSettings.ConnectionStringEventStore))
                {
                    con.Open();
                    using (
                        var command =
                            new SqlCommand(
                                $"delete from [EventStore].[Events] where [AggregateId]='{TestSettings.BankAccountAggregateId}'",
                                con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }


    public class BankAccountScheduledCommandTests : IDisposable
    {
        public BankAccountScheduledCommandTests()
        {
            Command<BankAccount>.AuthorizeDefault = (target, command) => true;

            _configuration = Configuration.Current;
            _configuration.UseSqlEventStore(c => c.UseConnectionString(TestSettings.ConnectionStringEventStore))
                .UseSqlStorageForScheduledCommands(
                    c => c.UseConnectionString(TestSettings.ConnectionStringCommandScheduler))
                .UseDependency<GetClockName>(c => _ => TestSettings.CustomClockName);

            CreateClockIfNotExists();

            _repository = _configuration.Repository<BankAccount>();

            _bankAccount = _repository.GetLatest(TestSettings.BankAccountAggregateId).Result ??
                           new BankAccount(new BankAccount.OpenAccount(TestSettings.BankAccountAggregateId)
                           {
                               CustomerId = new CustomerId("XavierScheduledCommand")
                           });
            _bankAccount.Apply(new BankAccount.DepositFunds
            {
                Amount = 100.0m
            });
            var task = _repository.Save(_bankAccount);
            task.Wait();
        }

        public void Dispose()
        {
            var task = _repository.Save(_bankAccount);
            task.Wait();
            _configuration.Dispose();
        }

        private void CreateClockIfNotExists()
        {
            try
            {
                using (var con = new SqlConnection(TestSettings.ConnectionStringCommandScheduler))
                {
                    con.Open();
                    using (
                        var command =
                            new SqlCommand(
                                $"select 1 from [Scheduler].[Clock] where [Name]='{TestSettings.CustomClockName}'",
                                con))
                    {
                        var exists = command.ExecuteScalar();
                        if (exists == null)
                            _configuration
                                .SchedulerClockRepository()
                                .CreateClock(TestSettings.CustomClockName, Clock.Now());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static CleanUpDatabaseFixture Current = new CleanUpDatabaseFixture();

        private readonly Configuration _configuration;

        private readonly IEventSourcedRepository<BankAccount> _repository;
        private readonly BankAccount _bankAccount;

        private async Task<DateTimeOffset> ForwardClock(DateTimeOffset clockTime)
        {
            clockTime = clockTime.AddDays(1);
            await _configuration.SchedulerClockTrigger()
                .AdvanceClock(TestSettings.CustomClockName, clockTime);
            return clockTime;
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
            await _bankAccount.ApplyAsync(new BankAccount.WithdrawFunds
            {
                Amount = 5.0m
            });
            var expected = balance - 5.0m;
            Assert.Equal(expected, _bankAccount.Balance);
        }

        [Fact]
        public async Task TestScheduledCommand()
        {
            var clockTime = DateTimeOffset.Now.AddMilliseconds(10);
            // Small offset so that add day will not fall on the boundary of the scheduled command.
            clockTime = await ForwardClock(clockTime);
            await ForwardClock(clockTime);
        }
    }
}