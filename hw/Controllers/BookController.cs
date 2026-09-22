using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using hw.DTO;
using hw.Repositories;

namespace hw.Controllers;

[ApiController]
[Route("api/books")]
public class BookController(IBookRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ReturnResult<IEnumerable<BookDto>>>> GetAll([FromQuery] string? author = null)
    {
        var books = await repository.GetAllAsync(author);
        return Ok(new ReturnResult<IEnumerable<BookDto>>(true, mapper.Map<List<BookDto>>(books)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReturnResult<BookDto>>> GetById(int id)
    {
        var book = await repository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound(new ReturnResult<BookDto>(false, null, "Книга не найдена."));
        }
        return Ok(new ReturnResult<BookDto>(true, mapper.Map<BookDto>(book)));
    }

    [HttpPost]
    public async Task<ActionResult<ReturnResult<BookDto>>> Create(SaveBookDto dto)
    {
        var book = mapper.Map<Book>(dto);
        book.Author = await repository.GetOrCreateAuthorAsync(dto.AuthorName);
        await repository.AddAsync(book);
        return CreatedAtAction(nameof(GetById), new { id = book.Id },
            new ReturnResult<BookDto>(true, mapper.Map<BookDto>(book)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReturnResult<BookDto>>> Update(int id, SaveBookDto dto)
    {
        var book = await repository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound(new ReturnResult<BookDto>(false, null, "Книга не найдена."));
        }
        mapper.Map(dto, book);
        book.Author = await repository.GetOrCreateAuthorAsync(dto.AuthorName);
        await repository.UpdateAsync(book);
        return Ok(new ReturnResult<BookDto>(true, mapper.Map<BookDto>(book)));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ReturnResult<BookDto>>> Delete(int id)
    {
        var book = await repository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound(new ReturnResult<BookDto>(false, null, "Книга не найдена."));
        }
        await repository.DeleteAsync(book);
        return Ok(new ReturnResult<BookDto>(true, null, "Книга удалена."));
    }
}
