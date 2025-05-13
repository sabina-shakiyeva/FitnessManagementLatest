using Fitness.Business.Abstract;
using Fitness.Entities.Models.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//Admuin ucun
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}
		//bunu hem adminde hem userde istifade ede biler
		[HttpGet("products")]
		public async Task<IActionResult> GetAll()
		{
			var products = await _productService.GetAllAsync();
			return Ok(products);
		}
		[HttpPost("add-product")]
		public async Task<IActionResult> Add([FromForm] ProductCreateDto productDto)
		{
			await _productService.AddAsync(productDto);
			return Ok(new { message = "Product successfully added." });
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var product = await _productService.GetByIdAsync(id);
			return Ok(product);
		}
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDto productDto)
		{
			await _productService.UpdateAsync(id, productDto);
			return Ok(new { message = "Product successfully updated." });
		}
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _productService.DeleteAsync(id);
			return Ok(new { message = "Product successfully deleted." });
		}
	}
}
