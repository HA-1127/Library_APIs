using Library.DTOS.publisher;
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
    public class PublishersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PublishersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] PublisherFilterDto? publisherFilterDto, int page = 1)
        {
            var publishers = await _unitOfWork.PublisharRepository.GetAsync();
            //filters 
            if (publisherFilterDto.FristName is not null)
            {
                publishers = publishers.Where(e => e.FristName.Contains(publisherFilterDto.FristName));
            }
            if (publisherFilterDto.LastName is not null)
            {
                publishers = publishers.Where(e => e.LastName.Contains(publisherFilterDto.LastName));
            }
            if (publisherFilterDto.Email is not null)
            {
                publishers = publishers.Where(e => e.Email == publisherFilterDto.Email);

            }
            if (publisherFilterDto.Address is not null)
            {
                publishers = publishers.Where(e => e.Address == publisherFilterDto.Address);
            }
            var Returns = new
            {
                FilterFristName = publisherFilterDto.FristName,
                FilterLastName = publisherFilterDto.LastName,
                FilterEmail = publisherFilterDto.Email,
                FilterAddress = publisherFilterDto.Address,
                Publishers = publishers.Skip((page - 1) * 8).Take(8).ToList()
            };
            //pagination 
            if (page < 0)
            {
                page = 1;
            }
            var Pagination = new
            {
                TotalNumberOfPage = Math.Ceiling(publishers.Count() / 8.0),
                CurantPage = page
            };
            return Ok(new
            {
                Returns,
                Pagination
            });
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(PublisherCreateDtos publisherCreateDtos)
        {
            var publisher = publisherCreateDtos.Adapt<Publisher>();
            await _unitOfWork.PublisharRepository.CreateAsync(publisher);
           await _unitOfWork.PublisharRepository.CommitAsync();
            return Ok("Successfull add Publishers");
        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var publisher = await _unitOfWork.PublisharRepository.GetOneAsync(e => e.Id == id);
            if (publisher is not null)
            {
                return Ok(publisher);
            }
            return NotFound();
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, PublisherUpdateDtos publisherUpdateDtos)
        {
            var publisher = await _unitOfWork.PublisharRepository.GetOneAsync(e => e.Id == id, tracked: false);
            var publishers = publisherUpdateDtos.Adapt<Publisher>();
            if (publisher is not null)
            {
                _unitOfWork.PublisharRepository.Edit(publishers);
               await _unitOfWork.PublisharRepository.CommitAsync();
                return Ok("successfull Update Publishers");
            }
            return NotFound();
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var publisher = await _unitOfWork.PublisharRepository.GetOneAsync(e => e.Id == id, tracked: false);
            if (publisher is null)
            {
                return NotFound(new { message = "No found Publisher Mathing this Id. " });
            }
            try
            {
                _unitOfWork.PublisharRepository.Delete(publisher);
                await _unitOfWork.PublisharRepository.CommitAsync();
                return Ok("Successfull Delete Publisher");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });

            }
        }
    }
}
