using Models;
using System.Threading.Tasks;

namespace DAL
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ComakershipsContext _context;

        public ReviewRepository()
        {
            _context = new ComakershipsContext();
        }

        public async Task<int> SaveReviewAsync(Review review)
        {
            await _context.Review.AddAsync(review);
            await _context.SaveChangesAsync();

            return review.Id;
        }
    }
}
