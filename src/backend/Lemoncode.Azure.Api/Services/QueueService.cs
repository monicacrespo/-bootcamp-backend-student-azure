using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Lemoncode.Azure.Models;
using Lemoncode.Azure.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Extensions.Msal;
using static System.Net.WebRequestMethods;
using System.ComponentModel;
using System.Drawing;
using System;

namespace Lemoncode.Azure.Api.Services
{
    public class QueueService
    {
        private readonly StorageOptions storageOptions;

        public QueueService(
            IOptions<StorageOptions> storageOptionsSettings
        )
        {
            this.storageOptions = storageOptionsSettings.Value;
        }

        // Azure Queue storage is a service for storing large numbers of messages that
        // can be accessed from anywhere in the world via authenticated calls using
        // HTTP or HTTPS, e.g. "https://MYSTORAGEACCOUNT.queue.core.windows.net/QUEUENAME".
        // A single queue message can be up to 64 KB in size, and a
        // queue can contain millions of messages, up to the total capacity limit of
        // a storage account.
        public async Task CreateQueueAndSendMessage(string message)
        {
            // Connection string to your Azure Storage account
            string connectionString = storageOptions.ConnectionString;

            // Name of the queue we'll send messages to
            string queueName = storageOptions.ScreenshotsQueue;

            // Get a reference to a queue and then create it
            QueueClient queue = new QueueClient(connectionString, queueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64 // or QueueMessageEncoding.None
            });
            if (null != await queue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("The queue was created.");
            }

            // Send a message to our queue
            await queue.SendMessageAsync(message);
        }
    }
}