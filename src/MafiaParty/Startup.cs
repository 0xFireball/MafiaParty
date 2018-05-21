using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MafiaParty.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nancy;
using Nancy.Owin;
using Microsoft.Extensions.Logging;

namespace MafiaParty {
    public class Startup {
        private const string config_file_name = "mfp.json";
        private readonly IConfigurationRoot _fileConfig;

        public Startup(IHostingEnvironment env) {
            if (!File.Exists(config_file_name)) {
                try {
                    // Create config file
                    Console.WriteLine($"Configuration file {config_file_name} does not exist, creating default.");
                    var confFileContent = JsonConvert.SerializeObject(new SConfiguration(), Formatting.Indented);
                    File.WriteAllText(config_file_name, confFileContent);
                } catch (Exception ex) {
                    Console.WriteLine($"Could not write to {config_file_name}: {ex}");
                }
            }

            var builder = new ConfigurationBuilder()
                .AddJsonFile(config_file_name,
                    optional: true,
                    reloadOnChange: true)
                .SetBasePath(env.ContentRootPath);

            _fileConfig = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        // ReSharper disable once InconsistentNaming
        public void ConfigureServices(IServiceCollection services) {
            // Adds services required for using options.
            services.AddOptions();

            // Register IConfiguration
            services.Configure<SConfiguration>(_fileConfig);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once InconsistentNaming
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime,
            IHostingEnvironment env, ILoggerFactory loggerFactory) {
            // create default configuration
            var serverConfig = new SConfiguration();
            // bind configuration
            _fileConfig.Bind(serverConfig);

            // build context
            var context = new SContext(serverConfig);

            // load database
            context.connectDatabase();
            context.log.writeLine($"Database connected", SLogger.LogLevel.Information);

            // register application stop handler
            applicationLifetime.ApplicationStopping.Register(() => onUnload(context));
            context.log.writeLine($"Application interrupt handler registered", SLogger.LogLevel.Information);

            // add aspnet developer exception page
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // add aspnet logger
            if (env.IsDevelopment()) {
                loggerFactory.AddConsole(LogLevel.Information);
            } else {
                loggerFactory.AddConsole(LogLevel.Warning);
            }

            // add wwwroot/
            app.UseStaticFiles();

            // set up Nancy OWIN hosting
            app.UseOwin(x => x.UseNancy(options => {
                options.PassThroughWhenStatusCodesAre(
                    HttpStatusCode.NotFound,
                    HttpStatusCode.InternalServerError
                );
                options.Bootstrapper = new AppBootstrapper(context);
            }));

            context.log.writeLine($"Web services mapped successfully", SLogger.LogLevel.Information);
        }

        private void onUnload(SContext sctx) {
            sctx.log.writeLine("Server unloading.", SLogger.LogLevel.Information);
        }
    }
}
