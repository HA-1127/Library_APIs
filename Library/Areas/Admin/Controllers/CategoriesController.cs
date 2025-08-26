using Library.DTOS.category;
using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]CategreyFilterDto? categreyFilterDto ,int page =1)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAsync();
            //filters
            if (categreyFilterDto.Name is not null)
            {
                categories = categories.Where(e => e.Name.Contains(categreyFilterDto.Name));
            }
            if (categreyFilterDto.Status)
            {
                categories = categories.Where(e => e.Status == categreyFilterDto.Status);
            }
            //pagination
            if (page < 0)
            {
                page = 1;
            }

            var Pageination = new
            {
                TotallNumberOfPage = Math.Ceiling(categories.Count() / 8.0),
                CurrantPage = page
            };

            var returns = new
            {
                FilterName = categreyFilterDto.Name,
                FilterStatus = categreyFilterDto.Status,
                Categreis  = categories.Skip((page - 1) * 8).Take(8).ToList()
             };
          
            return Ok(new
            {
                 returns,
                 Pageination
            });
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CatogeryCreatDtos catogeryCreatDtos)
        {
            var category = catogeryCreatDtos.Adapt<Category>();
           await _unitOfWork.CategoryRepository.CreateAsync(category);
           await _unitOfWork.CategoryRepository.CommitAsync();
            return Ok("add catogery succesfull");

        }
        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetOneAsync(e => e.Id == id, tracked:false);
            if (category is not null)
            {
                return Ok(category);
            }
            return NotFound();

        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id ,categoryUpdateDtos categoryUpdateDtos)
        {
           
            var catogrey = await _unitOfWork.CategoryRepository.GetOneAsync(e => e.Id == id ,tracked:false);
            var catogries = categoryUpdateDtos.Adapt<Category>();
            if (catogrey is not null)
            {
                _unitOfWork.CategoryRepository.Edit(catogries);
                await _unitOfWork.CategoryRepository.CommitAsync();
                return Ok("Update Categrey successfull");
            }
            return NotFound();
           
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categrie = await _unitOfWork.CategoryRepository.GetOneAsync(e => e.Id == id,tracked:false);
            if (categrie == null)
            {
                return NotFound(new { message = "No Category found matching this ID." });
            }
            try
            {
                _unitOfWork.CategoryRepository.Delete(categrie);
                await _unitOfWork.CategoryRepository.CommitAsync();
                return Ok("Delete Categrey successfull");
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });

            }

        }
    }
}
