using Api.Models.Dtos.Responses;
using DataAccess.Entities;

namespace Api.Mappers;

public static class EntitiesToDtoExtensions
{
    public static AuthUserInfo ToDto(this User user)
    {
        return new AuthUserInfo(Id: user.Id, UserName: user.UserName, Role: user.Role);
    }
}
