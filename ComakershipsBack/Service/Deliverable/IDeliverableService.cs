using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Models;
using Models.ViewModels;

namespace ServiceLayer
{
    public interface IDeliverableService : IBaseService<Deliverable>
    {
        Task<IList<DeliverableGet>> GetDeliverables(int comakershipId);

        Task<DeliverableGet> GetDeliverable(int id);

        Task<int> CreateDeliverable(int comakershipId, DeliverablePost newDeliverable);

        Task<bool> UpdateDeliverable(DeliverablePut updatedDeliverable);

        Task<bool> DeleteDeliverable(int id);

        Task<bool> UploadDeliverableToAzureStorageBlob(int ComakershipId, string blobName, MemoryStream file, HttpContent ctnt);

        Task<List<FileVM>> GetListComakershipFilesFromAzureStorageBlob(int ComakershipId);

        Task<bool> checkIfComakershipExistAsync(int comakershipId);
    }
}
