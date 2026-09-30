using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql;

using Platform.Common.Auth;

namespace Test.Platform.Integration;

/// <summary>
/// The HashRefreshTokens migration must turn existing plain tokens into hashes that the new code accepts,
/// so users who are signed in when the migration runs stay signed in.
/// Uses its own database, because it has to stop half-way through the migrations.
/// </summary>
public class HashRefreshTokensMigrationTests
{
  private const string MigrationBeforeHashing = "20260928193319_SyncPendingModelChanges";

  [Test]
  public async Task ExistingPlainTokens_AreConvertedToTheSameHashAsTheCode()
  {
    var connectionString = await CreateEmptyDatabase("migration_test");
    await using var dbContext = PostgresDatabase.CreateDbContext(connectionString);
    var migrator = dbContext.GetService<IMigrator>();

    // 1. The old schema, with a plain refresh token in it.
    await migrator.MigrateAsync(MigrationBeforeHashing);
    const string plainToken = "an-old-plain-refresh-token";
    await dbContext.Database.ExecuteSqlRawAsync("""
      INSERT INTO "AspNetUsers" ("Id", "FirstName", "LastName", "DateOfBirth", "EmailConfirmed",
        "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount")
      VALUES ('user-1', 'Jane', 'Doe', '2000-01-01', true, false, false, false, 0);
      INSERT INTO "RefreshTokens" ("Id", "Token", "UserId", "ExpiresAt")
      VALUES (gen_random_uuid(), 'an-old-plain-refresh-token', 'user-1', now() + interval '1 day');
      """);

    // 2. Run the hashing migration (and everything after it).
    await migrator.MigrateAsync();

    // 3. The stored value is now exactly what the C# code computes for the same token.
    var stored = await dbContext.RefreshTokens.AsNoTracking().SingleAsync();
    Assert.That(stored.TokenHash, Is.EqualTo(RefreshTokenHasher.Hash(plainToken)));
  }

  private static async Task<string> CreateEmptyDatabase(string name)
  {
    await using var connection = new NpgsqlConnection(PostgresDatabase.ConnectionString);
    await connection.OpenAsync();
    await using (var drop = new NpgsqlCommand($"DROP DATABASE IF EXISTS {name}", connection))
    {
      await drop.ExecuteNonQueryAsync();
    }

    await using (var create = new NpgsqlCommand($"CREATE DATABASE {name}", connection))
    {
      await create.ExecuteNonQueryAsync();
    }

    return new NpgsqlConnectionStringBuilder(PostgresDatabase.ConnectionString) { Database = name }.ConnectionString;
  }
}
