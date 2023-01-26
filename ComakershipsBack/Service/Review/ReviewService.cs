using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using Models;
using Models.ViewModels;

namespace Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewrepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewrepository, IMapper mapper)
        {
            _reviewrepository = reviewrepository;
            _mapper = mapper;
        }

        public async Task<int> SaveReview(ReviewPostVM review, ClaimsIdentity identity)
        {
            //TODO: check here if StudentUser has completed a comakership for the company 
            try
            {
                Review newReview = _mapper.Map<Review>(review);
                newReview.ReviewersName = identity.Name;

                if(newReview.Rating > 10 || newReview.Rating < 1) {
                    throw new Exception("Review rating out of bounds");
                }

                return await _reviewrepository.SaveReviewAsync(newReview);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}