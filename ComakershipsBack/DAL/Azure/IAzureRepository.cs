using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Models.ViewModels;

namespace DAL
{
    public interface IAzureRepository
    {
        Task<bool> uploadImageToAzureAsync(Stream image, Guid guid, int? companyId);
        Task<string> retrieveAzureBlobStorageImageUri(string fileName, int companyId);
        Task<bool> MoveCompanyLogo(int companyid, Guid filename);
        Task<bool> UploadFileToAzureStorageBlobForComakershipDeliverable(int comakershipId, string blobName, MemoryStream file, string fileName, string fileContentType);
        Task<List<FileVM>> RetrieveFilesOfComakershipFromAzureStorage(int comakershipId);
    }
}
