using Microsoft.EntityFrameworkCore;
using TechChallenge.Data.Context;
using Testcontainers.MsSql;

namespace TechChallenge.Domain.IntegrationTest;

[CollectionDefinition(nameof(ContextCollection))]
public class ContextCollection : ICollectionFixture<ContextFixture>
{
}

public class ContextFixture : IAsyncLifetime
{
    public techchallengeDbContext _context { get; private set; }
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU13-windowsservercore-ltsc2022")
        .WithWaitStrategy(Wait.ForWindowsContainer().UntilPortIsAvailable(1433))
        .WithStartupTimeout(TimeSpan.FromMinutes(5)) // Aumenta o timeout
        .Build();

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        var options = new DbContextOptionsBuilder<techchallengeDbContext>()
           .UseSqlServer(_msSqlContainer.GetConnectionString())
           .Options;

        _context = new techchallengeDbContext(options);
        _context.Database.Migrate();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}
