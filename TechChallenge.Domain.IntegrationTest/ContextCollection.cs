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
    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433)) // Verifica a porta 1433
    .WithStartupTimeout(TimeSpan.FromMinutes(2)) // Aumenta o tempo limite de inicialização
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
