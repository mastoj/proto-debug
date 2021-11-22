using System;
using System.Data.SqlClient;
using System.IO;
using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Configuration;
using Akka.DI.Core;
using Akka.DI.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Splunk;


namespace ProtoClusterDemo.Web
{
    public static class ActorSystemRefs
    {
        public static ActorSystem ActorSystem;
    }

    public class Program
    {
        public static int Main(string[] args)
        {
            var actorServiceCollection = new ServiceCollection();

            try
            {
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
//                        .UseSerilog()
                        .UseStartup<Startup>();
                });
    }
}
