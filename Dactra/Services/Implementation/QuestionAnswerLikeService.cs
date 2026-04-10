using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Implementation
{
    public class QuestionAnswerLikeService : IQuestionAnswerLikeService
    {
        private readonly IQuestionAnswerLikeRepository _likeRepo;
        private readonly IQuestionAnswerRepository _answerRepo;
        private readonly IHubContext<QuestionHub> _hub;

        public QuestionAnswerLikeService(
            IQuestionAnswerLikeRepository likeRepo,
            IQuestionAnswerRepository answerRepo,
            IHubContext<QuestionHub> hub)
        {
            _likeRepo = likeRepo;
            _answerRepo = answerRepo;
            _hub = hub;
        }

        public async Task<AnswerLikeResponseDto> ToggleLikeAsync(int answerId, string userId)
        {
            var answer = await _answerRepo.GetByIdAsync(answerId)
                ?? throw new KeyNotFoundException($"Answer {answerId} not found.");

            var isLiked = await _likeRepo.IsLikedByUserAsync(answerId, userId);
            AnswerLikeResponseDto response;

            if (isLiked)
            {
                await _likeRepo.RemoveLikeAsync(answerId, userId);
                response = new AnswerLikeResponseDto
                {
                    AnswerId = answerId,
                    LikesCount = await _likeRepo.GetLikesCountAsync(answerId),
                    IsLikedByCurrentUser = false,
                    LikedAt = null
                };
            }
            else
            {
                var like = await _likeRepo.AddLikeAsync(answerId, userId);
                response = new AnswerLikeResponseDto
                {
                    AnswerId = answerId,
                    LikesCount = await _likeRepo.GetLikesCountAsync(answerId),
                    IsLikedByCurrentUser = true,
                    LikedAt = like.CreatedAt
                };
            }

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(answer.QuestionId))
                .SendAsync("AnswerLikeUpdated", new
                {
                    answerId,
                    response.LikesCount,
                    response.IsLikedByCurrentUser
                });

            return response;
        }
    }
}
