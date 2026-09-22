using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductLab.DTO;
using ProductLab.Models;
using ProductLab.Repositories;
using ProductLab.Results;

namespace ProductLab.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductsController(
        IProductRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ReturnResult<IEnumerable<ProductDto>>>> GetAll()
    {
        var products = await _repository.GetAllAsync();
        var dto = _mapper.Map<IEnumerable<ProductDto>>(products);

        return Ok(ReturnResult<IEnumerable<ProductDto>>.Success(dto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReturnResult<ProductDto>>> GetById(int id)
    {
        var product = await _repository.GetAsync(id);

        if (product is null)
        {
            return NotFound(
                ReturnResult<ProductDto>.Failure("Product not found")
            );
        }

        var dto = _mapper.Map<ProductDto>(product);

        return Ok(ReturnResult<ProductDto>.Success(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ReturnResult<ProductDto>>> Create(
        [FromBody] ProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        product.Id = 0;

        await _repository.CreateAsync(product);

        var resultDto = _mapper.Map<ProductDto>(product);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ReturnResult<ProductDto>.Success(resultDto)
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReturnResult<ProductDto>>> Update(
        int id,
        [FromBody] ProductDto dto)
    {
        var product = await _repository.GetAsync(id);

        if (product is null)
        {
            return NotFound(
                ReturnResult<ProductDto>.Failure("Product not found")
            );
        }

        dto.Id = id;
        _mapper.Map(dto, product);

        await _repository.UpdateAsync(product);

        var resultDto = _mapper.Map<ProductDto>(product);

        return Ok(ReturnResult<ProductDto>.Success(resultDto));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ReturnResult<bool>>> Delete(int id)
    {
        var product = await _repository.GetAsync(id);

        if (product is null)
        {
            return NotFound(
                ReturnResult<bool>.Failure("Product not found")
            );
        }

        await _repository.DeleteAsync(id);

        return Ok(ReturnResult<bool>.Success(true));
    }
}
