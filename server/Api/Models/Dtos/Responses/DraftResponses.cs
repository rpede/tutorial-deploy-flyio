namespace Api.Models.Dtos.Responses;

public record Writer(string Id, string UserName);

public record Draft(long Id, string Title, Writer Author);

public record DraftDetail(long Id, string Title, string? Content, Writer Author);
