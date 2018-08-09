using System;
using AspNet.Security.OAuth.Jira;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BLL.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StructureMap;

namespace WebHost
{
    public class Startup
    {
        public const string TogglToTempoAuthScheme = "TogglToTempoAuthScheme";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add configuration
            services.AddSingleton(Configuration);

            // Add framework services.
            services.AddDbContext<SyncDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // Add framework services.
            services.AddMvc().AddRazorPagesOptions(opts =>
            {
                opts.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                opts.Conventions.AuthorizePage("/Index");
                opts.Conventions.AddPageRoute("/Index", "{*url}");
            });

            services.AddAuthentication(opts =>
                {
                    opts.DefaultScheme = TogglToTempoAuthScheme;
                })
                .AddJira(opts => {
                    opts.SaveTokens = true;
                    opts.AccessTokenEndpoint = Configuration["Authentication:Jira:AccessTokenEndpoint"];
                    opts.RequestTokenEndpoint = Configuration["Authentication:Jira:RequestTokenEndpoint"];
                    opts.ConsumerKey = Configuration["Authentication:Jira:ConsumerKey"];
                    opts.ConsumerSecret = Configuration["Authentication:Jira:ConsumerSecret"];
                    opts.AuthenticationEndpoint = Configuration["Authentication:Jira:AuthenticationEndpoint"];
                    opts.UserInfoEndpoint = Configuration["Authentication:Jira:UserInfoEndpoint"];
                })
                .AddCookie(TogglToTempoAuthScheme);

            return ConfigureIoC(services);
        }

        public IServiceProvider ConfigureIoC(IServiceCollection services)
        {
            var container = new Container();

            container.Configure(config =>
            {
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssembliesAndExecutablesFromApplicationBaseDirectory();
                    _.WithDefaultConventions();
                });

                config.For<IHttpContextAccessor>().Use<HttpContextAccessor>().Singleton();

                //Populate the container using the service collection
                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
