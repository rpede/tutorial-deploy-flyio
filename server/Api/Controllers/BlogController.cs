using Api.Models.Dtos.Requests;
using Api.Models.Dtos.Responses;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/blog")]
public class BlogController(IBlogService blogService) : ControllerBase
{
    private readonly IBlogService blogService = blogService;

    [HttpGet]
    [Route("")]
    public IEnumerable<Post> List([FromQuery] int? page) =>
        blogService.Newest(new PostsQuery(page ?? 0));

    [HttpGet]
    [Route("{id}")]
    public PostDetail Get(long id) => blogService.GetById(id);

    [HttpPost]
    [Route("{id}/comment")]
    public async Task<long> Comment(CommentFormData data, long id)
    {
        return await blogService.CreateComment(id, data);
    }
}
