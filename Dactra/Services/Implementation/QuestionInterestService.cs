using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Implementation
{
    public class QuestionInterestService : IQuestionInterestService
    {
        private readonly IQuestionInterestRepository _interestRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IHubContext<QuestionHub> _hub;

        public QuestionInterestService(
            IQuestionInterestRepository interestRepo,
            IQuestionRepository questionRepo,
            IHubContext<QuestionHub> hub)
        {
            _interestRepo = interestRepo;
            _questionRepo = questionRepo;
            _hub = hub;
        }

        public async Task<QuestionInterestResponseDto> ToggleInterestAsync(int questionId, string userId)
        {
            if (!await _questionRepo.ExistsAsync(questionId))
                throw new KeyNotFoundException($"Question {questionId} not found.");

            bool isInterested;
            if (await _interestRepo.IsInterestedByUserAsync(questionId, userId))
            {
                await _interestRepo.RemoveAsync(questionId, userId);
                isInterested = false;
            }
            else
            {
                await _interestRepo.AddAsync(new QuestionInterest
                {
                    QuestionId = questionId,
                    UserId = userId,
                    CreatedAt = DateTimeOffset.UtcNow
                });
                isInterested = true;
            }

            var total = await _interestRepo.GetCountByQuestionIdAsync(questionId);
            var response = new QuestionInterestResponseDto
            {
                QuestionId = questionId,
                TotalInterests = total,
                IsInterested = isInterested
            };

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(questionId))
                .SendAsync("InterestUpdated", response);

            return response;
        }
    }
}
