using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TeamPractice.DTO;
using TeamPractice.Models;
using TeamPractice.Repositories;
using TeamPractice.Results;

namespace TeamPractice.Controllers;

[ApiController]
[Route("api/team")]
public class TeamController : ControllerBase
{
    private readonly ITeamRepository _repository;
    private readonly IMapper _mapper;

    public TeamController(ITeamRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ReturnResult<IEnumerable<TeamDto>>>> GetAll()
    {
        var teams = await _repository.GetAllAsync();
        var dto = _mapper.Map<IEnumerable<TeamDto>>(teams);

        return Ok(ReturnResult<IEnumerable<TeamDto>>.Ok(dto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReturnResult<TeamDto>>> GetById(int id)
    {
        var team = await _repository.GetAsync(id);

        if (team is null)
            return NotFound(ReturnResult<TeamDto>.Error("Team not found"));

        return Ok(ReturnResult<TeamDto>.Ok(_mapper.Map<TeamDto>(team)));
    }

    [HttpPost]
    public async Task<ActionResult<ReturnResult<TeamDto>>> Create([FromBody] TeamDto dto)
    {
        var team = _mapper.Map<Team>(dto);
        team.Id = 0;

        await _repository.CreateAsync(team);

        var resultDto = _mapper.Map<TeamDto>(team);

        return CreatedAtAction(
            nameof(GetById),
            new { id = team.Id },
            ReturnResult<TeamDto>.Ok(resultDto, "Team created")
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReturnResult<TeamDto>>> Update(
        int id,
        [FromBody] TeamDto dto)
    {
        var team = await _repository.GetAsync(id);

        if (team is null)
            return NotFound(ReturnResult<TeamDto>.Error("Team not found"));

        dto.Id = id;
        _mapper.Map(dto, team);

        await _repository.UpdateAsync(team);

        return Ok(
            ReturnResult<TeamDto>.Ok(
                _mapper.Map<TeamDto>(team),
                "Team updated"
            )
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ReturnResult<bool>>> Delete(int id)
    {
        var team = await _repository.GetAsync(id);

        if (team is null)
            return NotFound(ReturnResult<bool>.Error("Team not found"));

        await _repository.DeleteAsync(id);

        return Ok(ReturnResult<bool>.Ok(true, "Team deleted"));
    }

    [HttpGet("city/{city}")]
    public async Task<ActionResult<ReturnResult<IEnumerable<TeamDto>>>> GetByCity(
        string city)
    {
        var teams = await _repository.GetByCityAsync(city);
        var dto = _mapper.Map<IEnumerable<TeamDto>>(teams);

        return Ok(ReturnResult<IEnumerable<TeamDto>>.Ok(dto));
    }
}
