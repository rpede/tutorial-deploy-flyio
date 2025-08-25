using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dtos.Requests;

public record DraftFormData([Required] string Title, [Required] string Content, bool? Publish);
