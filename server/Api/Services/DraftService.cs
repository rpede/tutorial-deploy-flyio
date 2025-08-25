using DataAccess.Repositories;
using Entities = DataAccess.Entities;
using Requests = Api.Models.Dtos.Requests;
using Responses = Api.Models.Dtos.Responses;

public interface IDraftService
{
    Responses.DraftDetail GetById(long id);
    IEnumerable<Responses.Draft> List();
    Task<long> Create(Requests.DraftFormData data);
    Task Update(long id, Requests.DraftFormData data);
    Task Delete(long id);
}

public class DraftService(
    IRepository<Entities.Post> _postRepository,
    IRepository<Entities.User> _userRepository
) : IDraftService
{
    public static string[] AllowedRoles => [Role.Admin, Role.Editor];

    public Responses.DraftDetail GetById(long id)
    {
        var post = _postRepository.Query().Single(x => x.Id == id);
        var user = _userRepository.Query().Single(x => x.Id == post.AuthorId);
        return new Responses.DraftDetail(
            Id: post.Id,
            Title: post.Title,
            Content: post.Content,
            Author: new Responses.Writer(user.Id, user.UserName!)
        );
    }

    public IEnumerable<Responses.Draft> List()
    {
        return _postRepository
            .Query()
            .Where(post => post.PublishedAt == null)
            .Join(
                _userRepository.Query(),
                post => post.AuthorId,
                user => user.Id,
                (post, user) => new { post, user }
            )
            .Select(x => new Responses.Draft(
                x.post.Id,
                x.post.Title,
                new Responses.Writer(x.user.Id, x.user!.UserName!)
            ))
            .ToArray();
    }

    public async Task<long> Create(Requests.DraftFormData data)
    {
        var post = new Entities.Post
        {
            Title = data.Title,
            Content = data.Content,
            AuthorId = null, // TODO fix
            PublishedAt = data.Publish ?? false ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await _postRepository.Add(post);
        return post.Id;
    }

    public async Task Update(long id, Requests.DraftFormData data)
    {
        var post = _postRepository.Query().Single(x => x.Id == id);
        post.Title = data.Title;
        post.Content = data.Content;
        post.UpdatedAt = DateTime.UtcNow;
        if (data.Publish ?? false)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        await _postRepository.Update(post);
    }

    public async Task Delete(long id)
    {
        var post = _postRepository.Query().Single(x => x.Id == id);
        await _postRepository.Delete(post);
    }
}
