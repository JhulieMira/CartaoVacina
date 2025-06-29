using CartaoVacina.Contracts.Data.DTOS.Users;
using CartaoVacina.Contracts.Data.DTOS.Vaccinations;
using CartaoVacina.Core.Exceptions;
using CartaoVacina.Core.Handlers.Commands;
using CartaoVacina.Core.Handlers.Commands.Users;
using CartaoVacina.Core.Handlers.Commands.Vaccinations;
using CartaoVacina.Core.Handlers.Queries;
using CartaoVacina.Core.Handlers.Queries.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartaoVacina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IMediator mediator) : ControllerBase
{

    [HttpGet("{userId:int}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    
    [HttpGet]
    [ProducesResponseType(typeof(List<UserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<UserDTO>>> List()
    {
        try
        {
            var query = new ListUsersQuery();
            var response = await mediator.Send(query);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDTO>> Create([FromBody]CreateUserDTO payload)
    {
        try
        {
            var query = new CreateUserCommand(payload);
            var response = await mediator.Send(query);

            return Ok(response);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPatch("{userId:int}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDTO>> Update(int userId, [FromBody]UpdateUserDTO payload)
    {
        try
        {
            var query = new UpdateUserCommand(userId, payload);
            var response = await mediator.Send(query);

            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpDelete("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDTO>> Delete(int userId)
    {
        try
        {
            var query = new DeleteUserCommand(userId);
            await mediator.Send(query);

            return NoContent();
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
    
    [HttpPost("{userId:int}/vaccinations")]
    [ProducesResponseType(typeof(VaccinationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaccinationDTO>> CreateVaccination(int userId, [FromBody]CreateVaccinationDTO payload)
    {
        try
        {
            var command = new CreateVaccinationCommand(userId, payload);
            var response = await mediator.Send(command);

            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPatch("{userId:int}/vaccinations/{vaccinationId:int}")]
    [ProducesResponseType(typeof(VaccinationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaccinationDTO>> UpdateVaccination(int userId, int vaccinationId, [FromBody] UpdateVaccinationDTO payload)
    {
        try
        {
            var command = new UpdateVaccinationCommand(userId, vaccinationId, payload);
            var response = await mediator.Send(command);

            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpDelete("{userId:int}/vaccinations/{vaccinationId:int}")]
    [ProducesResponseType(typeof(VaccinationDTO), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteVaccination(int userId, int vaccinationId)
    {
        try
        {
            var command = new DeleteVaccinationCommand(userId, vaccinationId);
            await mediator.Send(command);

            return NoContent();
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