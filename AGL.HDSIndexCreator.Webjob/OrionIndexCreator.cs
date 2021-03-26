using System.IO;
using AGL.HDSIndexCreator.Webjob.Models;
using AGL.HDSIndexCreator.Webjob.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace AGL.HDSIndexCreator.Webjob
{
    public class OrionIndexCreator
    {
        private readonly IOrionIndexService _orionCustomerService;

        public OrionIndexCreator(IOrionIndexService orionCustomerService)
        {
            _orionCustomerService = orionCustomerService;
        }

        public void ExecuteJob()
        {
            var loggerFactory = new LoggerFactory();
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.ApplicationInsights(TelemetryConverter.Traces, LogEventLevel.Information)
                .CreateLogger();
            loggerFactory.AddSerilog(loggerConfig);

            var logProvider = loggerFactory.CreateLogger<OrionIndexCreator>();
            logProvider.LogInformation("Orion Customer Index job is calling");
            _orionCustomerService.CreateIndex<Customer>();
            logProvider.LogInformation("ServiceOrder Index job is calling");
            _orionCustomerService.CreateIndex<ServiceOrder>();
        }
    }
}
