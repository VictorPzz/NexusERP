using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Reports.DTOs;
using NexusERP.Application.Modules.Reports.Queries.SalesReport;
using NexusERP.Application.Modules.Reports.Queries.PurchasesReport;
using NexusERP.Application.Modules.Reports.Queries.InventoryReport;

namespace NexusERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("sales")]
    [ProducesResponseType(typeof(ApiResponse<SalesReportDto>), 200)]
    public async Task<IActionResult> GetSalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string? groupBy)
    {
        var query = new SalesReportQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            GroupBy = groupBy
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("purchases")]
    [ProducesResponseType(typeof(ApiResponse<PurchasesReportDto>), 200)]
    public async Task<IActionResult> GetPurchasesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string? groupBy)
    {
        var query = new PurchasesReportQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            GroupBy = groupBy
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("inventory")]
    [ProducesResponseType(typeof(ApiResponse<InventoryReportDto>), 200)]
    public async Task<IActionResult> GetInventoryReport(
        [FromQuery] int? categoryId,
        [FromQuery] bool? lowStock)
    {
        var query = new InventoryReportQuery
        {
            CategoryId = categoryId,
            LowStock = lowStock
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
