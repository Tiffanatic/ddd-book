using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using static System.Environment;
using static System.Reflection.Assembly;

namespace Marketplace
{
    public static class Program
    {
        static Program()
        {
            CurrentDirectory = Path.GetDirectoryName(GetEntryAssembly()?.Location);
        }

        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            ConfigureWebHost(configuration, args).Build().Run();
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(CurrentDirectory)
                .Build();
        }

        private static IHostBuilder ConfigureWebHost(
            IConfiguration configuration, string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseConfiguration(configuration);
                    webBuilder.UseContentRoot(CurrentDirectory);
                    webBuilder.UseKestrel();
                });
        }
    }
}