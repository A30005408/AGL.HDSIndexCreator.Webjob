using AGL.HDSIndexCreator.Webjob.Models;
using AGL.HDSIndexCreator.Webjob.Shared.Constant;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Rest.Azure;
using System;
using System.Configuration;
using System.Net;

namespace AGL.HDSIndexCreator.Webjob.Services
{
    public class OrionIndexService : IOrionIndexService
    {
        
        private readonly ILogger<OrionIndexService> _logger;

        public OrionIndexService(ILogger<OrionIndexService> logger)
        {
            _logger = logger;
        }
        public void CreateIndex<T>()
        {
            string azureSQLConnectionStr = ConfigurationManager.AppSettings["AzureSqlConnectionString"].ToString();
            Uri Uri = new Uri(ConfigurationManager.AppSettings["SearchServiceEndPoint"]);
            string searchServiceKey = ConfigurationManager.AppSettings["SearchServiceAdminApiKey"];
            AzureKeyCredential keyCredential = new AzureKeyCredential(searchServiceKey);


            SearchIndexClient indexClient = new SearchIndexClient(Uri, keyCredential);
            SearchIndexerClient indexerClient = new SearchIndexerClient(Uri, keyCredential);          
            Console.WriteLine("Creating index...");
            _logger.LogInformation("Creating index for Orion Customer");
            FieldBuilder fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(T));
            var searchIndex = new SearchIndex(GetIndexName(HdsConstants.Orion, typeof(T).ToString()), searchFields);
            CleanupSearchIndexClientResources(indexClient, searchIndex);

            indexClient.CreateOrUpdateIndex(searchIndex);
           

             Console.WriteLine("Creating data source...");
            _logger.LogInformation("Creating data source for Orion Customer");

            var dataSource =
                  new SearchIndexerDataSourceConnection(
                     GetDataSourceName(HdsConstants.Orion, typeof(T).ToString()),
                     SearchIndexerDataSourceType.AzureSql,
                     azureSQLConnectionStr,
                     new SearchIndexerDataContainer($"[{GetTableName(typeof(T).ToString())}]"));

            indexerClient.CreateOrUpdateDataSourceConnection(dataSource);

            //Creating indexer
            Console.WriteLine("Creating Azure SQL indexer...");
            _logger.LogInformation("Creating Azure SQL indexer for Orion Customer");

            var schedule = new IndexingSchedule(TimeSpan.FromDays(1))
            {
                StartTime = DateTimeOffset.Now
            };

            var parameters = new IndexingParameters()
            {
                BatchSize = 100,
                MaxFailedItems = 0,
                MaxFailedItemsPerBatch = 0
            };
           
            var indexer = new SearchIndexer(GetIndexerName(HdsConstants.Orion, typeof(T).ToString()), dataSource.Name, searchIndex.Name)
            {
                Description = "Data indexer",
                Schedule = schedule,
                Parameters = parameters,
            };

            indexerClient.CreateOrUpdateIndexerAsync(indexer);


            Console.WriteLine("Running Azure SQL indexer...");

            try
            {
                indexerClient.RunIndexerAsync(indexer.Name);
            }
            catch (CloudException e) when (e.Response.StatusCode == (HttpStatusCode)429)
            {
                 Console.WriteLine("Failed to run indexer: {0}", e.Response.Content);
                _logger.LogError("Failed to run indexer: {0}", e.Response.Content);
            }
        }

        private static void CleanupSearchIndexClientResources(SearchIndexClient indexClient, SearchIndex index)
        {
            try
            {
                if (indexClient.GetIndex(index.Name) != null)
                {
                    indexClient.DeleteIndex(index.Name);
                }
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                //if exception occurred and status is "Not Found", this is working as expected
                Console.WriteLine("Failed to find index and this is because it doesn't exist.");
            }
        }

        private string GetIndexName(string applicationName, string tableName)
        {
            return $"{applicationName.ToLower()}-{tableName.Substring(tableName.LastIndexOf('.')+1).ToLower()}-index";
        }

        private string GetDataSourceName(string applicationName, string tableName)
        {
            return $"{applicationName.ToLower()}-{tableName.Substring(tableName.LastIndexOf('.') + 1).ToLower()}-ds";
        }

        private string GetIndexerName(string applicationName, string tableName)
        {
            return $"{applicationName.ToLower()}-{tableName.Substring(tableName.LastIndexOf('.') + 1).ToLower()}-indexer";
        } 
        
        private string GetTableName(string fullstr)
        {
            return fullstr.Substring(fullstr.LastIndexOf('.') + 1);
        }
        
    }
}
