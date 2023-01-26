using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using AutoMapper;

namespace ServiceLayer
{
    public class KeyService : BaseService<PurchaseKey>, IKeyService
    {
        private readonly IPurchaseKeyRepository _keyRepository;
        private readonly IMapper _mapper;

        public KeyService(IPurchaseKeyRepository keyRepository, IMapper mapper) : base(keyRepository)
        {
            _keyRepository = keyRepository;
            _mapper = mapper;
        }
        
        public async Task<int> CreateKey(PurchaseKeyPost postedKey)
        {
            var key = _mapper.Map<PurchaseKey>(postedKey);
            key.Claimed = false;
            return await _keyRepository.CreateKey(key);
        }

        public async Task<IList<PurchaseKey>> GetKeys()
        {
            var keys = await _keyRepository.GetKeys();
            return _mapper.Map<List<PurchaseKey>>(keys);
        }

        public async Task<PurchaseKey> GetKey(string keykeyString)
        {
            var key = await _keyRepository.GetKey(keykeyString);
            return _mapper.Map<PurchaseKey>(key);
        }

        public async Task<bool> UpdateKey(PurchaseKey updatedKey)
        {
            var dbKey = await _keyRepository.GetKey(updatedKey.Key);
            if (dbKey == null)
            {
                return false;
            }
            _mapper.Map(updatedKey, dbKey);

            return await _keyRepository.UpdateKey(dbKey);
        }

        public async Task<bool> DeleteKey(string keyString)
        {
            var dbKey = await _keyRepository.GetKey(keyString);
            if (dbKey == null)
            {
                return false;
            }
            return await _keyRepository.DeleteKey(dbKey);
        }

        public async Task<bool> ValidateKey(string keyString)
        {
            var key = await _keyRepository.GetKey(keyString);
            if (key == null || key.Claimed == true)
            {
                return false;
            }
            key.Claimed = true;
            return await _keyRepository.UpdateKey(key);
        }
    }
}
