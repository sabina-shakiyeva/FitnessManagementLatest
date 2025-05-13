using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
	public interface IProductService
	{
		Task<List<ProductGetDto>> GetAllAsync();
		Task<ProductGetDto> GetByIdAsync(int id);
		Task AddAsync(ProductCreateDto product);
		Task UpdateAsync(int productId, ProductUpdateDto productDto);
		Task DeleteAsync(int productId);
	}
}
