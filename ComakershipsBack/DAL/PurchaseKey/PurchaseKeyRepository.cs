using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class PurchaseKeyRepository : BaseRepository<PurchaseKey>, IPurchaseKeyRepository
    {
        public PurchaseKeyRepository(ComakershipsContext _context) :base(_context)
        {           
        }

        // Create
        public async Task<int> CreateKey(PurchaseKey newKey)
        {
            _context.PurchaseKeys.Add(newKey);
            await _context.SaveChangesAsync();

            return newKey.Id;
        }

        // Read one
        public async Task<PurchaseKey> GetKey(string key)
        {
            return await _context.PurchaseKeys
                .FirstOrDefaultAsync(k => k.Key == key);
        }

        // Read all
        public async Task<IEnumerable<PurchaseKey>> GetKeys()
        {
            return await _context.PurchaseKeys
                .ToListAsync();
        }

        // Update
        public async Task<bool> UpdateKey(PurchaseKey updatedKey)
        {
            var currentKey = await _context.PurchaseKeys.FirstOrDefaultAsync(c => c.Id == updatedKey.Id);

            if (currentKey != null)
            {
                _context.PurchaseKeys.Update(updatedKey);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        // Delete
        public async Task<bool> DeleteKey(PurchaseKey deleteKey)
        {
            var purchaseKey = _context.PurchaseKeys.FirstOrDefault(k => k.Id == deleteKey.Id);
            if (purchaseKey != null)
            {
                _context.PurchaseKeys.Remove(purchaseKey);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
