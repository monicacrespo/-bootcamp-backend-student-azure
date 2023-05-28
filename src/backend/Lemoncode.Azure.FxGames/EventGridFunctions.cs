// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Lemoncode.Azure.Models.Configuration;
using System.Linq;

namespace Lemoncode.Azure.FxGames
{
    public static class EventGridFunctions
    {
        [FunctionName("EventGridFunctions")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
        }

        [FunctionName("DeleteThumbnails")]     
        public static async Task DeleteThumbnails([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation("Thumbnails blob deletion started...");

            StorageBlobDeletedEventData deletedEvent = eventGridEvent.Data.ToObjectFromJson<StorageBlobDeletedEventData>();
            var storageConnection = Environment.GetEnvironmentVariable("AzureWebJobsGamesStorage");           

            try
            {
                var blobName = GetBlobNameFromUrl(deletedEvent.Url);

                BlobClient blobClient = new BlobClient(
                        storageConnection,
                        "thumbnails",
                        blobName);

                await blobClient.DeleteAsync();
            }
            catch (Exception ex)
            {
                log.LogError("Blob deletion error: " + ex.Message);
            }
            finally
            {
                log.LogInformation("Thumbnails blob deletion completed: {blobName}");
            }
        }

        private static string GetBlobNameFromUrl(string blobUrl)
        {
            // "url": "https://my-storage-account.blob.core.windows.net/testcontainer/file-to-delete.txt"
            // "url": "https://my-storage-account.blob.core.windows.net/screenshots/11/escape-from-monkey-island.jpg" 
            var lastTwoParts = blobUrl.Split('/').TakeLast(2);
            return string.Join("/", lastTwoParts);
        }
    }
}
