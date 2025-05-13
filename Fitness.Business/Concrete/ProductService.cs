using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Product;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using FitnessManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
	public class ProductService : IProductService
	{
		private readonly IProductDal _productDal;
		private readonly IFileService _fileService;
		private readonly IMapper _mapper;

		public ProductService(IProductDal productDal, IFileService fileService, IMapper mapper)
		{
			_productDal = productDal;
			_fileService = fileService;
			_mapper = mapper;

		}

		public async Task AddAsync(ProductCreateDto productDto)
		{
			var product=_mapper.Map<Product>(productDto);
			if (productDto.ImageUrl != null)
			{
				string imageUrl = _fileService.UploadFileAsync(productDto.ImageUrl).Result;
				product.ImageUrl = imageUrl;
			}
			await _productDal.Add(product);
		}
		
		public async Task DeleteAsync(int productId)
		{
			var product = await _productDal.Get(p => p.Id == productId);
			if (product == null)
			{
				throw new Exception("Product not found!");
			}

			await _productDal.Delete(product);
		}
		
		public async Task<List<ProductGetDto>> GetAllAsync()
		{
            //var products = await _productDal.GetList();
            var products = await _productDal.GetList(p => p.Stock > 0);


            var productDtos = products.Select(product => new ProductGetDto
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				PointCost=product.PointCost,
				Stock = product.Stock,
				ImageUrl = product.ImageUrl != null ? _fileService.GetFileUrl(product.ImageUrl) : null
			}).ToList();

			return productDtos;

		}
		
		public async Task<ProductGetDto> GetByIdAsync(int id)
		{
			var product = await _productDal.Get(p => p.Id == id);
			var productDto=new ProductGetDto
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				PointCost = product.PointCost,
				Stock = product.Stock,
				ImageUrl = product.ImageUrl != null ? _fileService.GetFileUrl(product.ImageUrl) : null
			};
			return productDto;
		}

		public async Task UpdateAsync(int productId, ProductUpdateDto productDto)
		{
			var product = await _productDal.Get(p => p.Id == productId);
			if (product == null)
			{
				throw new Exception("Product not found!");
			}

			
			product.Name = productDto.Name ?? product.Name;
			product.Description = productDto.Description ?? product.Description;
			product.PointCost = productDto.PointCost ?? product.PointCost;
			product.Stock = productDto.Stock ?? product.Stock;

			if (productDto.ImageUrl != null)
			{
				string imageUrl = await _fileService.UploadFileAsync(productDto.ImageUrl);
				product.ImageUrl = imageUrl;
			}

			await _productDal.Update(product);
		}

	}
}
