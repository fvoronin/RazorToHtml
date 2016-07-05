using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;
using Mvc.RenderViewToString;
using System.IO;
using Microsoft.Extensions.ObjectPool;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main()
        {
            // Initialize the necessary services
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, customApplicationBasePath: null);

            var provider = services.BuildServiceProvider();
            var helper = provider.GetRequiredService<RazorViewToStringRenderer>();

            var model = new Person {
                FirstName = "John",
                LastName = "Doe"
            };
            var emailContent = helper.RenderViewToString("Template", model);
            Console.WriteLine(emailContent);
            Console.ReadLine();
        }

        private static void ConfigureDefaultServices(IServiceCollection services, string customApplicationBasePath)
        {
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            var basePath = customApplicationBasePath ?? applicationEnvironment.ApplicationBasePath;
            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                WebRootFileProvider = new PhysicalFileProvider(customApplicationBasePath ?? applicationEnvironment.ApplicationBasePath),
                ApplicationName =  "RazorToHtml"
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(customApplicationBasePath ?? applicationEnvironment.ApplicationBasePath));
            });
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddSingleton<RazorViewToStringRenderer>();
        }
    }
}
