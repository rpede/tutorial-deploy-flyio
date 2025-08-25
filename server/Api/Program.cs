using Api.Etc;
using Api.Etc.NSwag;
using Api.Services;
using DataAccess;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);

        var app = builder.Build();

        SetupDatabase(app);

        ConfigureApp(app);

        await app.RunAsync();
    }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Entity Framework
        var connectionString = builder.Configuration.GetConnectionString("AppDb");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );
        builder.Services.AddScoped<DbSeeder>();

        // Repositories
        builder.Services.AddScoped<IRepository<User>, UserRepository>();
        builder.Services.AddScoped<IRepository<Post>, PostRepository>();
        builder.Services.AddScoped<IRepository<Comment>, CommentRepository>();

        // Services
        builder.Services.AddScoped<IBlogService, BlogService>();
        builder.Services.AddScoped<IDraftService, DraftService>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddOpenApiDocument(conf =>
        {
            conf.DocumentProcessors.Add(new TypeMapDocumentProcessor<ProblemDetails>());
            conf.SchemaSettings.AlwaysAllowAdditionalObjectProperties = false;
            conf.SchemaSettings.GenerateAbstractProperties = true;
            conf.SchemaSettings.SchemaProcessors.Add(new RequiredSchemaProcessor());
        });

        // Exception handling
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }

    public static void SetupDatabase(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            seeder.SetupAsync().Wait();
        }
    }

    public static WebApplication ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        // app.UseHttpsRedirection();
        app.UseExceptionHandler();

        app.UseAuthorization();

        app.MapControllers();

        app.GenerateApiClientsFromOpenApi("/../../client/src/models/generated-client.ts").Wait();
        app.MapScalarApiReference();

        app.Run();

        return app;
    }
}
