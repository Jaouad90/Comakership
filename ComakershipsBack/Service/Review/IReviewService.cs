using Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service
{
    public interface IReviewService
    {
        Task<int> SaveReview(ReviewPostVM review, ClaimsIdentity identity);
    }
}