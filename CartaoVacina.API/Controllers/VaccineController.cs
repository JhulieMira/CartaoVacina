using CartaoVacina.Contracts.DTOS.Vaccines;
using CartaoVacina.Core.Exceptions;
using CartaoVacina.Core.Handlers.Commands;
using CartaoVacina.Core.Handlers.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartaoVacina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VaccineController(IMediator mediator): ControllerBase
{
    
    [HttpGet("{vaccineId:int}")]
    [ProducesResponseType(typeof(VaccineDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaccineDTO>> GetById(int vaccineId)
    {
        try
        {
            var query = new GetVaccineByIdQuery(vaccineId);
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
    [ProducesResponseType(typeof(List<VaccineDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaccineDTO>> List()
    {
        try
        {
            var query = new ListVaccinesQuery();
            var response = await mediator.Send(query);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(VaccineDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaccineDTO>> Create([FromBody]CreateVaccineDTO payload)
    {
        try
        {
            var query = new CreateVaccineCommand(payload);
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
    
    [HttpPatch("{vaccineId:int}")]
    [ProducesResponseType(typeof(VaccineDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaccineDTO>> Update(int vaccineId, [FromBody]UpdateVaccineDTO payload)
    {
        try
        {
            var query = new UpdateVaccineCommand(vaccineId, payload);
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
    
    [HttpDelete("{vaccineId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int vaccineId)
    {
        try
        {
            var query = new DeleteVaccineCommand(vaccineId);
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
}