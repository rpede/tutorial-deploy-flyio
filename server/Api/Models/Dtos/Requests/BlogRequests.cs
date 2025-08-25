using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dtos.Requests;

public record PostsQuery(int Page = 0);

public record CommentFormData([Required] string Content, [Required] string AuthorId);
