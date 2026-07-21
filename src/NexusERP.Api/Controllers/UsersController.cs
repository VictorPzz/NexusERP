using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Identity.Commands.CreateUser;
using NexusERP.Application.Modules.Identity.Commands.UpdateUser;
using NexusERP.Application.Modules.Identity.Commands.DeleteUser;
using NexusERP.Application.Modules.Identity.Commands.AssignRole;
using NexusERP.Application.Modules.Identity.Commands.RemoveRole;
using NexusERP.Application.Modules.Identity.Queries.GetAllUsers;
using NexusERP.Application.Modules.Identity.Queries.GetUserById;
using NexusERP.Application.Modules.Identity.Queries.GetUserRoles;

namespace NexusERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), 200)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<int>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteUserCommand { Id = id });
        return Ok(result);
    }

    [HttpGet("{userId:int}/roles")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleInfoDto>>), 200)]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var result = await _mediator.Send(new GetUserRolesQuery { UserId = userId });
        return Ok(result);
    }

    [HttpPost("{userId:int}/roles/{roleId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(int userId, int roleId)
    {
        var result = await _mediator.Send(new AssignRoleCommand { UserId = userId, RoleId = roleId });
        return Ok(result);
    }

    [HttpDelete("{userId:int}/roles/{roleId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var result = await _mediator.Send(new RemoveRoleCommand { UserId = userId, RoleId = roleId });
        return Ok(result);
    }
}
