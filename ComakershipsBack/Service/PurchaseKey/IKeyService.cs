using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ServiceLayer
{
    public interface IKeyService : IBaseService<PurchaseKey>
    {
        Task<IList<PurchaseKey>> GetKeys();

        Task<PurchaseKey> GetKey(string key);        

        Task<int> CreateKey(PurchaseKeyPost postedKey);

        Task<bool> UpdateKey(PurchaseKey updatedKey);

        Task<bool> DeleteKey(string key);
        Task<bool> ValidateKey(string key);
    }
}
