namespace Dactra.Services.Implementation
{
    public class PostService:IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _postRepository.GetAllAsync();
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _postRepository.GetByIdAsync(id);
        }

        public async Task<Post> CreateAsync(Post post)
        {
            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();
            return post;
        }

        public async Task<bool> UpdateAsync(int postId, int doctorId, UpdatePostDto dto)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return false;

            // 🔒 صاحب البوست بس
            if (post.DoctorId != doctorId)
                throw new UnauthorizedAccessException();

            post.title = dto.Title;
            post.Content = dto.Content;
            post.MajorsId = dto.MajorsId;

            _postRepository.Update(post);
            await _postRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int postId, int doctorId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return false;

            // 🔒 صاحب البوست بس
            if (post.DoctorId != doctorId)
                throw new UnauthorizedAccessException();

            _postRepository.SoftDelete(post);
            await _postRepository.SaveChangesAsync();
            return true;
        }
    }
}
