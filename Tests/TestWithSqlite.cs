using System;
using BLL;
using BLL.Data;
using BLL.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class TestWithSqlite : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        public DrinkRepository Repository { get; private set; }
        public DrinkAppContext Context { get; private set; }

        public TestWithSqlite()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<DrinkAppContext>()
                .UseSqlite(_connection)
                .Options;
            Context = new DrinkAppContext(options);
            Context.Database.EnsureCreated();
            var data = new DrinkLoader().InitializeDrinksFromFile();
            // Add drinks to the database
            Context.AddRange(data);
            Context.SaveChanges();

            Repository = new DrinkRepository(Context);
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}