using BuberDinner.Application.Authentication.Common;
using BuberDinner.Contracts.Authentication;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BuberDinner.Application.Authentication.Commands.Register;
using BuberDinner.Application.Authentication.Queries.Login;

namespace BuberDinner.Api.Controllers;

[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly ISender _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.FirstName, request.LastName, request.Email, request.Password);
        ErrorOr<AuthenticationResult> authResult = await _mediator.Send(command);

        return authResult.Match(
            authResult => Ok(MapAuthresult(authResult)),
            errors => Problem(errors)
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);
        ErrorOr<AuthenticationResult> authResult = await _mediator.Send(query);


        return authResult.Match(
            authResult => Ok(MapAuthresult(authResult)),
            errors => Problem(errors)
        );
    }

    private static AuthenticationResponse MapAuthresult(AuthenticationResult authResult)
    {
        return new AuthenticationResponse(
                    authResult.user.Id,
                    authResult.user.FirstName,
                    authResult.user.LastName,
                    authResult.user.Email,
                    authResult.Token
                );
    }
}