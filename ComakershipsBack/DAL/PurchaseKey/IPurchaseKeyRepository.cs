using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface IPurchaseKeyRepository : IBaseRepository<PurchaseKey>
    {
        Task<IEnumerable<PurchaseKey>> GetKeys();

        Task<PurchaseKey> GetKey(string key);

        Task<int> CreateKey(PurchaseKey newKey);

        Task<bool> UpdateKey(PurchaseKey updatedKey);

        Task<bool> DeleteKey(PurchaseKey deleteKey);
    }
}
