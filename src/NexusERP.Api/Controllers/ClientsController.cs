using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.Commands.CreateClient;
using NexusERP.Application.Modules.People.Commands.UpdateClient;
using NexusERP.Application.Modules.People.Commands.DeleteClient;
using NexusERP.Application.Modules.People.Queries.GetAllClients;
using NexusERP.Application.Modules.People.Queries.GetClientById;
using NexusERP.Application.Modules.People.DTOs;

namespace NexusERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClientDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllClientsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ClientDto>), 200)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetClientByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<int>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteClientCommand { Id = id });
        return Ok(result);
    }
}
