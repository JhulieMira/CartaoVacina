using CartaoVacina.Contracts.DTOS.Users;
using CartaoVacina.Core.Exceptions;
using CartaoVacina.Core.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartaoVacina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IMediator mediator) : ControllerBase
{

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserDTO>> GetById(int userId)
    {
        try
        {
            var query = new GetUserByIdQuery(userId);
            var response = await mediator.Send(query);
            
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}