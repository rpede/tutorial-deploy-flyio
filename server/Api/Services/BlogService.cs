using DataAccess.Repositories;
using Entities = DataAccess.Entities;
using Requests = Api.Models.Dtos.Requests;
using Responses = Api.Models.Dtos.Responses;

namespace Api.Services;

public interface IBlogService
{
    Responses.PostDetail GetById(long id);
    IEnumerable<Responses.Post> Newest(Requests.PostsQuery query);
    Task<long> CreateComment(long postId, Requests.CommentFormData data);
}

public class BlogService(
    IRepository<Entities.Post> _postRepository,
    IRepository<Entities.User> _userRepository,
    IRepository<Entities.Comment> _commentRepository
) : IBlogService
{
    public Responses.PostDetail GetById(long id)
    {
        var post = _postRepository
            .Query()
            // Only post that has been published
            .Single(post => post.Id == id && post.PublishedAt != null);
        var author = _userRepository
            .Query()
            .Where(user => user.Id == post!.AuthorId)
            .Select(user => new Responses.Author(user.Id, user.UserName))
            .Single();
        var comments = _commentRepository
            .Query()
            .Where(comment => comment.PostId == post.Id)
            .Join(
                _userRepository.Query(),
                comment => comment.AuthorId,
                user => user.Id,
                (comment, author) =>
                    new Responses.CommentForPost(
                        comment.Id,
                        comment.Content,
                        comment.CreatedAt,
                        new Responses.Author(author!.Id, author!.UserName!)
                    )
            )
            .ToArray();
        return new Responses.PostDetail(
            post.Id,
            post.Title,
            post.Content,
            author!,
            comments,
            (DateTime)post.PublishedAt!,
            post.UpdatedAt > post.PublishedAt ? post.UpdatedAt : null
        );
    }

    public IEnumerable<Responses.Post> Newest(Requests.PostsQuery query)
    {
        const int pageSize = 10;
        return _postRepository
            .Query()
            .Where(post => post.PublishedAt != null)
            .OrderByDescending(post => post.PublishedAt)
            .Skip(query.Page * pageSize)
            .Take(pageSize)
            .Join(
                _userRepository.Query(),
                post => post.AuthorId,
                user => user.Id,
                (post, user) => new { post, user }
            )
            .Select(x => new Responses.Post(
                x.post.Id,
                x.post.Title,
                x.post.Content,
                new Responses.Author(x.user.Id, x.user!.UserName!),
                (DateTime)x.post.PublishedAt!,
                x.post.UpdatedAt > x.post.PublishedAt ? x.post.UpdatedAt : null
            ))
            .ToArray();
    }

    public async Task<long> CreateComment(long postId, Requests.CommentFormData data)
    {
        var comment = new Entities.Comment
        {
            Content = data.Content,
            AuthorId = data.AuthorId,
            PostId = postId,
            CreatedAt = DateTime.UtcNow,
        };
        await _commentRepository.Add(comment);
        return comment.Id;
    }
}
