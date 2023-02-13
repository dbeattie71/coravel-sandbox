using Microsoft.EntityFrameworkCore;

namespace CoravelSandbox;

public static class Extensions
{
    public static IServiceCollection AddDbContext<T>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? optionsAction = null
    ) where T : DbContext
    {
        optionsAction ??= builder => builder.UseSqlite(connectionString);
        services.AddDbContext<T>((provider, builder) =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            builder.UseLoggerFactory(loggerFactory);
            //builder.EnableDetailedErrors();
            optionsAction.Invoke(builder);
        });

        return services;
    }
}