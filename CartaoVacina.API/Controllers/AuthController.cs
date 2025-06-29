using CartaoVacina.Contracts.Data.DTOS.Accounts;
using CartaoVacina.Core.Handlers.Commands.Accounts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartaoVacina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO payload)
    {
        try
        {
            var command = new LoginCommand(payload);
            var response = await mediator.Send(command);

            return Ok(response);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO payload)
    {
        try
        {
            var command = new RegisterCommand(payload);
            var response = await mediator.Send(command);

            return Ok(response);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already registered"))
                return Conflict(ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
} 