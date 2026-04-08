using Dactra.DTOs.QuestionDTOs;
using Dactra.Models;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionRepository(ApplicationDbContext context) => _context = context;

        public async Task<Question?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Questions
                .Include(q => q.Patient)
                .Include(q => q.Answers.Where(a => !a.isDeleted && a.ParentAnswerId == null))
                .Include(q => q.Answers.Where(a => !a.isDeleted && a.ParentAnswerId == null))
                    .ThenInclude(a => a.Replies.Where(r => !r.isDeleted))
                .Include(q => q.Interests)
                .Include(q => q.SavedBy)
                .Include(q => q.QuestionTags)
                    .ThenInclude(qt => qt.Tag)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(q => !q.isDeleted);

            return await query.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Question?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Questions
                .Where(q => q.Id == id && !q.isDeleted)
                .Include(q => q.Patient)
                .Include(q => q.Answers.Where(a => !a.isDeleted && a.ParentAnswerId == null))
                .Include(q => q.Answers.Where(a => !a.isDeleted && a.ParentAnswerId == null))
                    .ThenInclude(a => a.Replies.Where(r => !r.isDeleted))
                .Include(q => q.Interests)
                .Include(q => q.SavedBy)
                .Include(q => q.QuestionTags)
                    .ThenInclude(qt => qt.Tag)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<Question> Questions, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var query = _context.Questions
                .Where(q => !q.isDeleted)
                .Include(q => q.Patient)
                .Include(q => q.Answers.Where(a => !a.isDeleted))
                .Include(q => q.Interests)
                .Include(q => q.SavedBy)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .OrderByDescending(q => q.CreatedAt);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<(List<Question> Questions, int TotalCount)> GetByPatientIdAsync(int patientId, int page, int pageSize)
        {
            var query = _context.Questions
                .Where(q => !q.isDeleted && q.PatientId == patientId)
                .Include(q => q.Patient)
                .Include(q => q.Answers.Where(a => !a.isDeleted))
                .Include(q => q.Interests)
                .Include(q => q.SavedBy)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .OrderByDescending(q => q.CreatedAt);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<(List<Question> Questions, int TotalCount)> GetByTagAsync(int tagId, int page, int pageSize)
        {
            var query = _context.Questions
                .Where(q => !q.isDeleted && q.QuestionTags.Any(qt => qt.TagId == tagId))
                .Include(q => q.Patient)
                .Include(q => q.Answers.Where(a => !a.isDeleted))
                .Include(q => q.Interests)
                .Include(q => q.SavedBy)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .OrderByDescending(q => q.CreatedAt);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<(List<Question> Questions, int TotalCount)> GetFilteredAsync(
            QuestionFilterDto filter, string userId, int page, int pageSize)
        {
            var query = _context.Questions
                .Where(q => !q.isDeleted)
                .Include(q => q.Patient)
                .Include(q => q.Answers.Where(a => !a.isDeleted))
                .Include(q => q.Interests)
                .Include(q => q.SavedBy)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .AsQueryable();

            query = filter switch
            {
                QuestionFilterDto.Interested => query.Where(q => q.Interests.Any(i => i.UserId == userId)),
                QuestionFilterDto.Saved => query.Where(q => q.SavedBy.Any(s => s.UserId == userId)),
                QuestionFilterDto.Answered => query.Where(q => q.Answers.Any(a => !a.isDeleted && a.AnswererUserId == userId)),
                _ => query
            };

            query = query.OrderByDescending(q => q.CreatedAt);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Question> CreateAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                question.isDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Questions.AnyAsync(q => q.Id == id && !q.isDeleted);

        public async Task<bool> BelongsToPatientAsync(int questionId, int patientId)
            => await _context.Questions.AnyAsync(q => q.Id == questionId && q.PatientId == patientId && !q.isDeleted);

        public async Task AssignTagsAsync(int questionId, List<int> tagIds)
        {
            var existing = await _context.QuestionTags
                .Where(qt => qt.QuestionId == questionId)
                .ToListAsync();

            _context.QuestionTags.RemoveRange(existing);

            var newTags = tagIds.Select(tagId => new QuestionTag
            {
                QuestionId = questionId,
                TagId = tagId
            }).ToList();

            _context.QuestionTags.AddRange(newTags);
            await _context.SaveChangesAsync();
        }

        public async Task<UserQuestionStatsDto> GetUserQuestionStatsAsync(string userId)
        {
            var interested = await _context.QuestionInterests
                .CountAsync(i => i.UserId == userId && !i.Question.isDeleted);

            var saved = await _context.QuestionSaves
                .CountAsync(s => s.UserId == userId && !s.Question.isDeleted);

            var answered = await _context.QuestionAnswers
                .Where(a => !a.isDeleted && !a.Question.isDeleted && a.AnswererUserId == userId)
                .Select(a => a.QuestionId)
                .Distinct()
                .CountAsync();
            var shared = await _context.Questions.Include(s => s.Patient)
                .Where(s => s.Patient.UserId == userId && !s.isDeleted)
                .CountAsync();

            return new UserQuestionStatsDto
            {
                TotalInterested = interested,
                TotalSaved = saved,
                TotalAnswered = answered,
                TotalShared = shared
            };
        }
        public async Task<QuestionStatsDto> GetQuestionStatsAsync(int questionId)
        {
            var interested = await _context.QuestionInterests
                .CountAsync(i => i.QuestionId == questionId && !i.Question.isDeleted);
            var saved = await _context.QuestionSaves
                .CountAsync(s => s.QuestionId == questionId && !s.Question.isDeleted);
            var answered = await _context.QuestionAnswers
                .Where(a => !a.isDeleted && !a.Question.isDeleted && a.QuestionId == questionId)
                .Select(a => a.Id)
                .Distinct()
                .CountAsync();
            return new QuestionStatsDto
            {
                TotalInterested = interested,
                TotalSaved = saved,
                TotalAnswered = answered
            };
        }
    }
}