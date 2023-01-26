using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using Models;
using Models.ViewModels;

namespace ServiceLayer
{
    public class DeliverableService : BaseService<Deliverable>, IDeliverableService
    {
        private readonly IDeliverableRepository _deliverableRepository;
        private readonly IComakershipService _comakershipService;
        private readonly IAzureRepository _azureRepository;
        private readonly IMapper _mapper;

        public DeliverableService(IDeliverableRepository deliverableRepository, IMapper mapper, IAzureRepository azureRepository, IComakershipService comakershipService) : base(deliverableRepository)
        {
            _deliverableRepository = deliverableRepository;
            _comakershipService = comakershipService;
            _azureRepository = azureRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateDeliverable(int comakershipId, DeliverablePost newDeliverable)
        {
            var deliverable = _mapper.Map<Deliverable>(newDeliverable);
            var comakership = await _comakershipService.GetSingle(c => c.Id == comakershipId, c => c.Deliverables);
            if (comakership != null)
            {
                return await _deliverableRepository.CreateDeliverable(comakership, deliverable);
            }
            else throw new Exception("This comakership doesn't exists");
        }

        public async Task<bool> DeleteDeliverable(int id)
        {
            return await _deliverableRepository.DeleteWhere(d => d.Id == id);
        }

        public async Task<DeliverableGet> GetDeliverable(int id)
        {
            var deliverables = await _deliverableRepository.GetSingle(id);
            return _mapper.Map<DeliverableGet>(deliverables);
        }

        public async Task<IList<DeliverableGet>> GetDeliverables(int comakershipId)
        {
            var deliverables = await _deliverableRepository.GetDeliverables(comakershipId);            
            return _mapper.Map<List<DeliverableGet>>(deliverables);
        }

        public async Task<bool> UpdateDeliverable(DeliverablePut updatedDeliverable)
        {
            var dbDeliverable = await _deliverableRepository.GetSingle(updatedDeliverable.Id);
            if (dbDeliverable == null)
            {
                return false;
            }
            _mapper.Map(updatedDeliverable, dbDeliverable);            
            return await _deliverableRepository.Update(dbDeliverable);
        }

        public async Task<bool> UploadDeliverableToAzureStorageBlob(int ComakershipId, string blobName, MemoryStream file, HttpContent ctnt)
        {
            // retrieving filename and content from content disposition. Risky
            var fileName = ctnt.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            var fileContentType = ctnt.Headers.ContentType.ToString();

            return await _azureRepository.UploadFileToAzureStorageBlobForComakershipDeliverable(ComakershipId, blobName, file, fileName, fileContentType);
        }

        public async Task<List<FileVM>> GetListComakershipFilesFromAzureStorageBlob(int ComakershipId)
        {
            return await _azureRepository.RetrieveFilesOfComakershipFromAzureStorage(ComakershipId);
        }

        public async Task<bool> checkIfComakershipExistAsync(int comakershipId)
        {
            var Comakership =  await _comakershipService.GetComakership(comakershipId);

            if(Comakership != null)
            {
                return true;
            }
            return false;
        }
    }
}
