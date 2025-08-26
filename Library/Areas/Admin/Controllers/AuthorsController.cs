using Library.DTOS.author;
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
    public class AuthorsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] AuthorFilterDtos? authorFilterDtos, int page = 1)
        {
            var authors = await _unitOfWork.AuthorRepository.GetAsync();
            //filters
            if (authorFilterDtos.FirstName is not null)
            {
                authors = authors.Where(e => e.FirstName.Contains(authorFilterDtos.FirstName));
            }
            if (authorFilterDtos.LastName is not null)
            {
                authors = authors.Where(e => e.LastName.Contains(authorFilterDtos.LastName));
            }
            if (authorFilterDtos.Email is not null)
            {
                authors = authors.Where(e => e.Email == authorFilterDtos.Email);
            }
            if (authorFilterDtos.Address is not null)
            {
                authors = authors.Where(e => e.Address == authorFilterDtos.Address);
            }
            if (authorFilterDtos.FromAge is not null)
            {
                authors = authors.Where(e => e.Age > authorFilterDtos.FromAge);
            }
            if (authorFilterDtos.ToAge is not null)
            {
                authors = authors.Where(e => e.Age < authorFilterDtos.ToAge);
            }
            var Returns = new
            {
                FilterFristName = authorFilterDtos.FirstName,
                FilterLastName = authorFilterDtos.LastName,
                FilterAddress = authorFilterDtos.Address,
                FiltersEmail = authorFilterDtos.Email,
                FilterFromAge = authorFilterDtos.FromAge,
                FiltersToAge = authorFilterDtos.ToAge,
                Authors = authors.Skip((page - 1) * 8).Take(8).ToList()
            };
            //pagination
            if (page < 0)
            {
                page = 1;
            }
            var Pagination = new
            {
                TotalNUmberOfPage = Math.Ceiling(authors.Count() / 8.0),
                CurantPage = page
            };
            return Ok(new
            {
                Returns,
                Pagination
            });
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] AutherCreateDto autherCreateDto)
        {
            var authors = autherCreateDto.Adapt<Author>();
            if (autherCreateDto.Image is not null && autherCreateDto.Image.Length > 0)
            {
                var Filename = Guid.NewGuid().ToString() + Path.GetExtension(autherCreateDto.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\authors", Filename);

                // Save img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    await autherCreateDto.Image.CopyToAsync(stream);
                }
                // save in db
                authors.Image = filePath;
               await _unitOfWork.AuthorRepository.CreateAsync(authors);
               await _unitOfWork.AuthorRepository.CommitAsync();
                return Ok("Successfull Add Authors");

            }
            return NotFound();
        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var author = await _unitOfWork.AuthorRepository.GetOneAsync(e => e.Id == id);
            if (author is not null)
            {
                return Ok(author);
            }
            return NotFound();
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id,[FromForm] AuthorUpdateDtos authorUpdateDtos)
        {
            var authorsdb = await _unitOfWork.AuthorRepository.GetOneAsync(e => e.Id == id, tracked: false);
            var author = authorUpdateDtos.Adapt<Author>();
            if (authorsdb is not null)
            {
                if (authorUpdateDtos.Image is not null && authorUpdateDtos.Image.Length > 0)
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(authorUpdateDtos.Image.FileName);
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\authors", FileName);
                    //save image in wwwroot
                    using (var streem = System.IO.File.Create(FilePath))
                    {
                        await authorUpdateDtos.Image.CopyToAsync(streem);
                    }
                    //delet image olde
                    var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\authors", authorsdb.Image);
                    if (System.IO.File.Exists(OldFilePath))
                    {
                        System.IO.File.Delete(OldFilePath);
                    }
                    //save image in db 
                    author.Image = FilePath;
                }
                else
                {
                    author.Image = authorsdb.Image;
                }
                _unitOfWork.AuthorRepository.Edit(author);
              await  _unitOfWork.AuthorRepository.CommitAsync();
                return Ok("Successfull Update Authors");
            }

            return NotFound();
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _unitOfWork.AuthorRepository.GetOneAsync(e => e.Id == id ,tracked:false);
            if (author is null)
            {
                return NotFound(new { message = "Not Found Authors Matching this is id" });
            }
            try
            {
                // delete image old
                var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\authors", author.Image);

                if (System.IO.File.Exists(OldFilePath))
                {
                    System.IO.File.Delete(OldFilePath);
                }
                _unitOfWork.AuthorRepository.Delete(author);
                await _unitOfWork.AuthorRepository.CommitAsync();
                return Ok("Successfull Delete Auther");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });

            }
        }
    }

}
