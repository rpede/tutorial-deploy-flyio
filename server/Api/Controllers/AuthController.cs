using Api.Models.Dtos.Requests;
using Api.Models.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("register")]
    public async Task<RegisterResponse> Register([FromBody] RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IResult> Logout()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("userinfo")]
    public async Task<AuthUserInfo> UserInfo()
    {
        throw new NotImplementedException();
    }
}
