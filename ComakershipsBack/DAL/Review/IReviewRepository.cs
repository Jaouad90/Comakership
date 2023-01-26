using Models;
using System.Threading.Tasks;

namespace DAL
{
    public interface IReviewRepository
    {
        Task<int> SaveReviewAsync(Review review);
    }
}
