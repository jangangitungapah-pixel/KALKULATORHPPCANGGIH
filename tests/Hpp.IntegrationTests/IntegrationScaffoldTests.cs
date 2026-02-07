using Xunit;

namespace Hpp.IntegrationTests;

public class IntegrationScaffoldTests
{
    [Fact(Skip = "TODO: configure SQL Server container or LocalDB")]
    public void Migrations_Can_Run_Against_Real_Database()
    {
        // TODO: Use docker image mcr.microsoft.com/mssql/server:2022-latest
        // and configure connection string in test settings.
        Assert.True(true);
    }
}
