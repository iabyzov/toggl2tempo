using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
		.UseUrls("http://*:5000")
                .UseStartup<Startup>()
                .Build();
    }
}
