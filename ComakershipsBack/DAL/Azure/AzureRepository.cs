using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Models.ViewModels;

namespace DAL
{
    public class AzureRepository : IAzureRepository
    {
        private static string _ConnectionString;
        private static string _ContainerReference;
        private static BlobContainerClient _Client;
        private static StorageSharedKeyCredential _Key;
        private static CloudStorageAccount _StorageAccount;

        public AzureRepository()
        {
            _ConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            // Parsing ConString into object so we can parse stuff
            _StorageAccount = CloudStorageAccount.Parse(_ConnectionString);

            // Setting default storage if no containerReference is supplied. Need to do this differently.
            _ContainerReference = "azure-default-storage";

            // Creating client with default containerReference
            _Client = getBlobContainerClient(_ConnectionString, _ContainerReference);

            // parsing accountKey from ConString
            string accountKey = _StorageAccount.Credentials.ExportBase64EncodedKey();

            // local development
            if (accountKey == null)
            {
                _Key = new StorageSharedKeyCredential(_Client.AccountName, "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==");
            }
            else
            {
                _Key = new StorageSharedKeyCredential(_Client.AccountName, accountKey);
            }
        }

        private BlobContainerClient getBlobContainerClient(string _connectionString, string _containerReference)
        {
            return new BlobContainerClient(_connectionString, _containerReference);
        }

        /// <summary>
        /// Function that's responsible for uploading images to Azure.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async Task<bool> uploadImageToAzureAsync(Stream image, Guid guid, int? companyId = null)
        {
            var fileName = guid.ToString() + "." + ImageFormat.Png.ToString().ToLower();

            var img = Bitmap.FromStream(image);

            // setting containerReference to companyContainerReference
            if (companyId != null)
            {
                _ContainerReference = await GetContainerReference((int)companyId);
            }

            return await SendAsBlob(img, fileName, _ContainerReference, _ConnectionString);
        }

        /// <summary>
        /// Private function that's called from the public UploadImageToAzureAsync.
        /// </summary>
        /// <param name="renderedImage"></param>
        /// <param name="fileName"></param>
        /// <param name="containerReference"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static async Task<bool> SendAsBlob(Image renderedImage, string fileName, string containerReference, string connectionString)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Fill stream with image in PNG format
                renderedImage.Save(memoryStream, ImageFormat.Png);

                // Reset read position to start of stream
                memoryStream.Position = 0;

                var storageAccount = CloudStorageAccount.Parse(connectionString);

                var blobClient = storageAccount.CreateCloudBlobClient();

                var cloudBlobContainer = blobClient.GetContainerReference(containerReference);

                // create the container if not exists
                await cloudBlobContainer.CreateIfNotExistsAsync();

                //URi = cloudBlobContainer.StorageUri.PrimaryUri;

                // Get Blob reference
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

                try
                {
                    // Write to blob
                    await cloudBlockBlob.UploadFromStreamAsync(memoryStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Function that checks if the blob actually exists or not
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<bool> blobExists(string fileName, int companyId)
        {
            // initializing blobClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(_ConnectionString);

            // retrieving container reference of company
            _ContainerReference = await GetContainerReference(companyId);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_ContainerReference);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // checks if the file exists on blobstorage
            return blobClient.Exists();
        }

        /// <summary>
        /// Function that retrieves the Uri based on the filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> retrieveAzureBlobStorageImageUri(string fileName, int companyId)
        {
            await Task.Yield();
            fileName += ".png";
            if (await blobExists(fileName, companyId))
            {
                BlobContainerClient client = getBlobContainerClient(_ConnectionString, await GetContainerReference(companyId));

                string Uri = GetBlobSasUri(client, fileName, _Key, null);
                return Uri;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Function that retrieves the BlobSasUri from the storage account
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <param name="key"></param>
        /// <param name="storedPolicyName"></param>
        /// <returns></returns>
        private static string GetBlobSasUri(BlobContainerClient container,
        string blobName, StorageSharedKeyCredential key, string storedPolicyName = null)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
            };

            if (storedPolicyName == null)
            {
                sasBuilder.StartsOn = DateTimeOffset.UtcNow;
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(4);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                if (_StorageAccount.Credentials.AccountName == "devstoreaccount1")
                {
                    sasBuilder.Protocol = SasProtocol.HttpsAndHttp;
                }
                else
                {
                    sasBuilder.Protocol = SasProtocol.Https;
                }
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            // Use the key to get the SAS token.
            string sasToken = "?" + sasBuilder.ToSasQueryParameters(key).ToString();                // todo i needed to add "?" for the SASToken to work??

            Console.WriteLine("SAS for blob is: {0}", sasToken);
            Console.WriteLine();

            return container.GetBlockBlobClient(blobName).Uri + sasToken;
        }

        private async Task<string> GetContainerReference(int companyId)
        {
            await Task.Yield();
            string containerReference = $"azure-storage-container-{companyId}";

            return containerReference;
        }

        public async Task<bool> MoveCompanyLogo(int companyid, Guid filename)
        {
            string TargetContainer = await GetContainerReference(companyid);
            string sourceContainer = _ContainerReference;
            string fileName = $"{filename}.png";

            return await MoveBlobInSameStorageAcountAsync(sourceContainer, TargetContainer, fileName);
        }
        private async Task<bool> MoveBlobInSameStorageAcountAsync(string SourceContainer, string TargetContainer, string fileToMove)
        {
            CloudBlobClient blobClient = _StorageAccount.CreateCloudBlobClient();

            // create references to blobcontainers
            CloudBlobContainer sourceContainer = blobClient.GetContainerReference(SourceContainer);
            CloudBlobContainer targetContainer = blobClient.GetContainerReference(TargetContainer);
            
            // create the container if not exists
            await targetContainer.CreateIfNotExistsAsync();

            // create reference to blobs
            CloudBlockBlob sourceBlob = sourceContainer.GetBlockBlobReference(fileToMove);
            CloudBlockBlob targetBlob = targetContainer.GetBlockBlobReference(fileToMove);

            // copying over and delete from source
            await targetBlob.StartCopyAsync(sourceBlob);
            await sourceBlob.DeleteAsync();

            return true;
        }

        public async Task<bool> UploadFileToAzureStorageBlobForComakershipDeliverable(int comakershipId, string blobName, MemoryStream file, string fileName, string fileContentType)
        {
            // creating containerReference
            string containerPrefix = $"comakership-{comakershipId}";

            // instance of CloudblobClient
            CloudBlobClient blobClient = _StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerPrefix);

            try
            {
                // setting requestpolicy to retry every 20 seconds with a maximum of 4 retries. 
                BlobRequestOptions optionsWithRetryPolicy = new BlobRequestOptions() { RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.LinearRetry(TimeSpan.FromSeconds(20), 4) };
                await container.CreateIfNotExistsAsync(optionsWithRetryPolicy, null);
            }
            catch (StorageException e)
            {
                // TODO rethrow exception for catching in servicelayer/controller
                throw;
            }

            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                blockBlob.Properties.ContentType = fileContentType;

                // setting metadata before uploading to Azure
                blockBlob.Metadata["originalfilename"] = $"{fileName}";
                await blockBlob.UploadFromStreamAsync(file);

                return true;
            }
            catch (Exception)
            {
                // TODO rethrow exception for catching in servicelayer/controller
                throw;
            }
        }
        
        public async Task<List<FileVM>> RetrieveFilesOfComakershipFromAzureStorage(int comakershipId)
        {
            // creating containerReference
            string containerPrefix = $"comakership-{comakershipId}";
            List<FileVM> files = new List<FileVM>();

            // instance of CloudblobClient
            CloudBlobClient blobClient = _StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerPrefix);
            BlobContainerClient client = getBlobContainerClient(_ConnectionString, container.Name);
            BlobContinuationToken token = null;

            try
            {
                do
                {
                    var results = await container.ListBlobsSegmentedAsync(null, token);
                    token = results.ContinuationToken;

                    foreach (IListBlobItem item in results.Results)
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        await blob.FetchAttributesAsync();
                        files.Add(new FileVM
                        {
                            OriginalName = blob.Metadata["originalfilename"],
                            SasUri = GetBlobSasUri(client, item.Uri.Segments.Last(), _Key, null)
                        });

                    }
                } while (token != null);

                return files;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
