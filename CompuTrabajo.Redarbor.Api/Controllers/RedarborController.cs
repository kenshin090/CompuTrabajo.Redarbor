using CompuTrabajo.Redarbor.Application.Command;
using CompuTrabajo.Redarbor.Application.Common;
using CompuTrabajo.Redarbor.Application.Common.Dto;
using CompuTrabajo.Redarbor.Application.Common.Interfaces;
using CompuTrabajo.Redarbor.Application.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RedarborController : ControllerBase
{
    private readonly ICommandHandler<CreateEmployeeCommand> _createHandler;
    private readonly ICommandHandler<UpdateEmployeeCommand> _updateHandler;
    private readonly ICommandHandler<DeleteEmployeeCommand> _deleteHandler;

    private readonly IQueryHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeReadDto>> _getAllHandler;
    private readonly IQueryHandler<GetEmployeeQuery, EmployeeReadDto?> _getByIdHandler;

    public RedarborController(
        ICommandHandler<CreateEmployeeCommand> createHandler,
        ICommandHandler<UpdateEmployeeCommand> updateHandler,
        ICommandHandler<DeleteEmployeeCommand> deleteHandler,
        IQueryHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeReadDto>> getAllHandler,
        IQueryHandler<GetEmployeeQuery, EmployeeReadDto?> getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
    }

    // -------------------------
    // GET ALL
    // -------------------------
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeReadDto>>> GetAll(CancellationToken ct)
    {
        var result = await _getAllHandler.Handle(new GetAllEmployeesQuery(), ct);
        return Ok(result);
    }

    // -------------------------
    // GET BY ID
    // -------------------------
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeReadDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _getByIdHandler.Handle(new GetEmployeeQuery(id), ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // -------------------------
    // CREATE
    // -------------------------
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command, CancellationToken ct)
    {
        await _createHandler.HandleAsync(command, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = command.EmployeeId },
            null);
    }

    // -------------------------
    // UPDATE
    // -------------------------
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeCommand command, CancellationToken ct)
    {
        if (id != command.EmployeeId)
            return BadRequest("Id mismatch");

        await _updateHandler.HandleAsync(command, ct);

        return NoContent();
    }

    // -------------------------
    // DELETE (Soft Delete)
    // -------------------------
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _deleteHandler.HandleAsync(new DeleteEmployeeCommand(id), ct);
        return NoContent();
    }
}