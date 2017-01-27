using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskList.Models;
using Swashbuckle.AspNetCore.Swagger;
using Serilog;

namespace TaskList
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Populate configuration values based on appsettings.json and environment variables
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                // .AddInMemoryCollection(ConfigValues) // If we had in-memory data to load as configuration, it can be done with this API
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Serilog sinks based on configuration, as explained in
            // https://github.com/serilog/serilog-sinks-literate#json-appsettingsjson-configuration
            // https://github.com/serilog/serilog-settings-configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            // Decide which database to use based on configuration
            var databaseConfig = Configuration.GetSection("DatabaseConnection");
            switch (databaseConfig["Provider"]?.ToLowerInvariant())
            {
                case "inmemory":
                    services.AddDbContext<TasksContext>(options => options.UseInMemoryDatabase());
                    break;
                case "azuresql":
                    var connectionStringBase = databaseConfig["AzureSqlConnection:ConnectionString"];
                    var userId = databaseConfig["AzureSqlConnection:UserId"];
                    var password = databaseConfig["AzureSqlConnection:Password"];
                    var connectionString = $"{connectionStringBase};User ID={userId};Password={password}";
                    services.AddDbContext<TasksContext>(options => options.UseSqlServer(connectionString));
                    break;
                default:
                    throw new InvalidOperationException("No database provided for Entity Framework use. Make sure DatabaseConnection:Provider is set.");
            }
            // Add framework services.
            services.AddMvc();

            // Add swagger services
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new Info { Title = "Task APIs", Version = "v1", Description = "API for Task Tracker" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
            }

            // Start Serilog - a popular third-party structured logging framework
            // Many Serilog sinks are available. https://github.com/serilog
            loggerFactory.AddSerilog();
            // Make sure that any bufferred messages are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
            var log = loggerFactory.CreateLogger<Startup>();

            // Very simple custom middleware to log unhandled exceptions
            // More complex middleware would, of course, be enacpsulated in its own class
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware
            app.Use(async (context, next)=>
            {
                try
                {
                    await next();
                }
                catch(Exception exc)
                {
                    // Context.Response could be modifier here, if desired (set status code, for example)
                    // TODO - EventId's should be created unique to different event types the application may log
                    log.LogError(new EventId(1), exc, "Unhandled exception caught");
                    throw;
                }
            });

            // Seed database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var dbContext = serviceScope.ServiceProvider.GetService<TasksContext>())
            {
                var databaseConfig = Configuration.GetSection("DatabaseConnection");

                // Non-relational databases (like InMemory) cannot have migrations applied to them
                if (databaseConfig["Provider"]?.ToLowerInvariant() == "inmemory")
                {
                    log.LogDebug("Ensuring database created");
                    dbContext.Database.EnsureCreated();
                }
                else
                {
                    log.LogDebug("Ensuring database migrated");
                    dbContext.Database.Migrate();
                }
                dbContext.Seed();
            }

            // Developer exception page is useful for development but should not be used
            // in release mode. On the other hand, UseExceptionHandler is a good way
            // of capturing and handling any unhandled exceptions from controllers.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // This serves static content from /wwwroot (such as js, images, or even static html)
            app.UseStaticFiles();

            // Here, we enable MVC routing and provide default routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "Task APIs v1");
            });
        }
    }
}
