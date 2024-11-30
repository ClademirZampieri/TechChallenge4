using DotNet.Testcontainers.Builders;
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
        .WithImage("mcr.microsoft.com/windows/servercore:ltsc2022")
        .WithWaitStrategy(Wait.ForWindowsContainer().UntilPortIsAvailable(1433))
        .Build();

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        await Task.Delay(TimeSpan.FromSeconds(30));

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