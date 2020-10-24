using FluentAssertions;
using System.Linq;
using Xunit;

namespace Tests
{
    public class RepositoryShould : IClassFixture<TestWithSqlite>
    {
        private readonly TestWithSqlite _fixture;

        public RepositoryShould(TestWithSqlite fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Get_all_drinks_from_the_database()
        {
            var drinks = _fixture.Repository.GetAllDrinks().ToList();
            drinks.Should().HaveCount(42);
        }

        [Fact]
        public void Not_get_empty_or_null_list_from_the_database()
        {
            var drinks = _fixture.Repository.GetAllDrinks().ToList();
            drinks.Should().NotBeNullOrEmpty();
        }
    }
}
