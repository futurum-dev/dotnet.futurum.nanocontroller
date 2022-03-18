using Futurum.Core.Result;

namespace Futurum.NanoController.Sample.Blog;

public static class BlogMapper
{
    public static BlogDto MapToDto(Blog domain) => 
        new(domain.Id.GetValueOrDefault(x => x.Value, 0), domain.Url);

    public static Result<Blog> MapToDomain(BlogDto dto) => 
        new Blog(dto.Id.ToId(), dto.Url).ToResultOk();
}