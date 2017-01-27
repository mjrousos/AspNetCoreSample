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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Seed database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var dbContext = serviceScope.ServiceProvider.GetService<TasksContext>())
            {
                var databaseConfig = Configuration.GetSection("DatabaseConnection");

                // Non-relational databases (like InMemory) cannot have migrations applied to them
                if (databaseConfig["Provider"]?.ToLowerInvariant() == "inmemory")
                {
                    dbContext.Database.EnsureCreated();
                }
                else
                {
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
        }
    }
}
