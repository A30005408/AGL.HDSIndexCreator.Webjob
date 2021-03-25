using AGL.HDSIndexCreator.Webjob.Models;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Rest.Azure;
using System;
using System.Configuration;
using System.Net;

namespace AGL.HDSIndexCreator.Webjob.Services
{
    public class ServiceOrderService
    {
        string azureSQLConnectionStr = ConfigurationManager.AppSettings["AzureSqlConnectionString"].ToString();
        Uri Uri = new Uri(ConfigurationManager.AppSettings["SearchServiceEndPoint"]);
        static string searchServiceKey = ConfigurationManager.AppSettings["SearchServiceAdminApiKey"];
        AzureKeyCredential keyCredential = new AzureKeyCredential(searchServiceKey);

        
        public void CreateCustomerIndex()
        {
            //creating index
            SearchIndexClient indexClient = new SearchIndexClient(Uri, keyCredential);
            SearchIndexerClient indexerClient = new SearchIndexerClient(Uri, keyCredential);

            Console.WriteLine("Creating index...");
            FieldBuilder fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(ServiceOrderDetails));
            //var searchIndex = new SearchIndex("serviceorder-sql-idx", searchFields);
            var searchIndex = new SearchIndex("serviceorder-sql-idx", searchFields);

            // If we have run the sample before, this index will be populated
            // We can clear the index by deleting it if it exists and creating
            // it again
            CleanupSearchIndexClientResources(indexClient, searchIndex);

            indexClient.CreateOrUpdateIndex(searchIndex);
            //Creating data source

            Console.WriteLine("Creating data source...");

            var dataSource =
                  new SearchIndexerDataSourceConnection(
                     "serviceorder-sql-ds",
                     SearchIndexerDataSourceType.AzureSql,
                     azureSQLConnectionStr,
                     new SearchIndexerDataContainer("[ServiceOrder]"));

            indexerClient.CreateOrUpdateDataSourceConnection(dataSource);

            //Creating indexer
            Console.WriteLine("Creating Azure SQL indexer...");

            //var schedule = new IndexingSchedule(TimeSpan.FromDays(1))
            //{
            //    StartTime = DateTimeOffset.Now
            //};

            var parameters = new IndexingParameters()
            {
                BatchSize = 100,
                MaxFailedItems = 0,
                MaxFailedItemsPerBatch = 0
            };



            // Indexer declarations require a data source and search index.
            // Common optional properties include a schedule, parameters, and field mappings
            // The field mappings below are redundant due to how the Hotel class is defined, but 
            // we included them anyway to show the syntax 
            var indexer = new SearchIndexer("serviceorder-sql-idxr", dataSource.Name, searchIndex.Name)
            {
                Description = "Service Order indexer",
                Schedule = new IndexingSchedule(TimeSpan.FromMinutes(5)),
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
            }

            // Wait 5 seconds for indexing to complete before checking status
            Console.WriteLine("Waiting for indexing...\n");
            System.Threading.Thread.Sleep(5000);
        }

        //private static void CheckIndexerStatus(SearchIndexerClient indexerClient, SearchIndexer indexer)
        //{
        //    try
        //    {
        //        string indexerName = "customer-sql-idxr";
        //        SearchIndexerStatus execInfo = indexerClient.GetIndexerStatus(indexerName);

        //        Console.WriteLine("Indexer has run {0} times.", execInfo.ExecutionHistory.Count);
        //        Console.WriteLine("Indexer Status: " + execInfo.Status.ToString());

        //        IndexerExecutionResult result = execInfo.LastResult;

        //        Console.WriteLine("Latest run");
        //        Console.WriteLine("Run Status: {0}", result.Status.ToString());
        //        Console.WriteLine("Total Documents: {0}, Failed: {1}", result.ItemCount, result.FailedItemCount);

        //        TimeSpan elapsed = result.EndTime.Value - result.StartTime.Value;
        //        Console.WriteLine("StartTime: {0:T}, EndTime: {1:T}, Elapsed: {2:t}", result.StartTime.Value, result.EndTime.Value, elapsed);

        //        string errorMsg = (result.ErrorMessage == null) ? "none" : result.ErrorMessage;
        //        Console.WriteLine("ErrorMessage: {0}", errorMsg);
        //        Console.WriteLine(" Document Errors: {0}, Warnings: {1}\n", result.Errors.Count, result.Warnings.Count);
        //    }
        //    catch (Exception e)
        //    {
        //        // Handle exception
        //    }
        //}

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

        //private static void CleanupSearchIndexerClientResources(SearchIndexerClient indexerClient, SearchIndexer indexer)
        //{
        //    try
        //    {
        //        if (indexerClient.GetIndexer(indexer.Name) != null)
        //        {
        //            indexerClient.ResetIndexer(indexer.Name);
        //        }
        //    }
        //    catch (RequestFailedException e) when (e.Status == 404)
        //    {
        //        //if exception occurred and status is "Not Found", this is working as expected
        //        Console.WriteLine("Failed to find indexer and this is because it doesn't exist.");
        //    }
        //}
    }
}
