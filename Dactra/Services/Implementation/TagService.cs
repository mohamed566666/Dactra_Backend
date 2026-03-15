using Dactra.DTOs.TagDTOs;

namespace Dactra.Services.Implementation
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepo;
        private readonly IPostTagRepository _postTagRepo;
        private readonly IAITaggingService _aiTagging;

        public TagService(
            ITagRepository tagRepo,
            IPostTagRepository postTagRepo,
            IAITaggingService aiTagging)
        {
            _tagRepo = tagRepo;
            _postTagRepo = postTagRepo;
            _aiTagging = aiTagging;
        }

        public async Task<List<TagDto>> GetAllTagsAsync()
        {
            var tags = await _tagRepo.GetAllAsync();
            return tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList();
        }

        public async Task<List<TagDto>> GetTagsByPostIdAsync(int postId)
        {
            var postTags = await _postTagRepo.GetByPostIdAsync(postId);
            return postTags.Select(pt => new TagDto { Id = pt.Tag.Id, Name = pt.Tag.Name }).ToList();
        }

        public async Task<List<TagDto>> AutoTagPostAsync(int postId, string content)
        {
            var allTags = await _tagRepo.GetAllAsync();
            var availableNames = allTags.Select(t => t.Name).ToList();

            var matchedNames = await _aiTagging.ExtractTagsFromContentAsync(content, availableNames);
            var matchedTags = await _tagRepo.GetByNamesAsync(matchedNames);

            await _postTagRepo.ReplaceTagsAsync(postId, matchedTags.Select(t => t.Id).ToList());

            return matchedTags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList();
        }

        public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
        {
            var existing = await _tagRepo.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new InvalidOperationException($"Tag '{dto.Name}' already exists.");

            var tag = new Tag { Name = dto.Name };
            var created = await _tagRepo.CreateAsync(tag);

            return new TagDto { Id = created.Id, Name = created.Name };
        }

        public async Task DeleteTagAsync(int id)
        {
            var tag = await _tagRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Tag {id} not found.");

            await _tagRepo.DeleteAsync(id);
        }
    }
}
