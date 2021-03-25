using Microsoft.Azure.WebJobs;
using AGL.HDSIndexCreator.Webjob.Services;
using Autofac;
using Microsoft.Extensions.Logging;
using AGL.HDSIndexCreator.Webjob.Shared.Constant;
using AGL.HDSIndexCreator.Webjob.Models;

namespace AGL.HDSIndexCreator.Webjob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    public static class Program
    {
        public static IContainer IocContainer;
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);

            IocContainer = RegisterDependencies();
            var orionCustomerService = IocContainer.Resolve<IOrionCustomerService>();
            var orionIndexCreator = new OrionIndexCreator(orionCustomerService);
            orionIndexCreator.ExecuteJob();
            host.RunAndBlock();
        }

        public static IContainer RegisterDependencies()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(typeof(IOrionCustomerService).Assembly);
            containerBuilder.RegisterType<OrionCustomerService>().As<IOrionCustomerService>().SingleInstance();           
            containerBuilder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
            containerBuilder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();          
            return containerBuilder.Build();
        }
    }
}
