using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,User")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    
    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    // GET: api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        return Ok(_repository.GetAll());
    }

    // GET api/products/5
    [HttpGet("{id}")]
    public ActionResult<Product> Get(int id)
    {
        var product = _repository.GetById(id);
        return product is null ? NotFound() : Ok(product);
    }

    // POST api/products
    [HttpPost]
    public ActionResult<Product> Post([FromBody] Product product)
    {
        var createdProduct = _repository.Create(product);
        return CreatedAtAction(nameof(Get), new { id = createdProduct.Id }, createdProduct);
    }

    // PUT api/products/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Product product)
    {
        try
        {
            _repository.Update(id, product);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE api/products/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _repository.Delete(id);
        return NoContent();
    }
}
