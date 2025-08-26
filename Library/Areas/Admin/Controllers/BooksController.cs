using Library.DTOS.Books;
using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] BooksFilterDtos? booksFilterDtos, int page = 1)
        {
            var books = await _unitOfWork.BooksRepository.GetAsync(includes: [e => e.publisher, e => e.author, e => e.category, e => e.imageBooks]);

            //filters 
            if (booksFilterDtos.Name is not null)
            {
                books = books.Where(e => e.Name.Contains(booksFilterDtos.Name));
            }
            if (booksFilterDtos.AuthorNme is not null)
            {
                books = books.Where(e => (e.author.FirstName + " " + e.author.LastName).Contains(booksFilterDtos.AuthorNme));
            }
            if (booksFilterDtos.PublisherName is not null)
            {
                books = books.Where(e => (e.author.FirstName + " " + e.author.LastName).Contains(booksFilterDtos.PublisherName));
            }
            if (booksFilterDtos.MinPrice is not null)
            {
                books = books.Where(e => e.Price - e.Price * (e.Discount / 100) > booksFilterDtos.MinPrice);
            }
            if (booksFilterDtos.MaxPrice is not null)
            {
                books = books.Where(e => e.Price - e.Price * (e.Discount / 100) < booksFilterDtos.MaxPrice);
            }
            if (booksFilterDtos.PublicationYare is not null)
            {
                books = books.Where(e => e.PublicationYare == booksFilterDtos.PublicationYare);
            }
            if (booksFilterDtos.isHot)
            {
                books = books.Where(e => e.Discount > 50);
            }
            var categrey = await _unitOfWork.CategoryRepository.GetAsync();
            if (booksFilterDtos.CategoryId is not null)
            {
                if (booksFilterDtos.CategoryId > 0 && categrey.Count() > booksFilterDtos.CategoryId)
                {
                    books = books.Where(e => e.CategoryId == booksFilterDtos.CategoryId);
                }
            }
            var Returns = new
            {
                bookname = booksFilterDtos.Name,
                authorName = booksFilterDtos.AuthorNme,
                publisherName = booksFilterDtos.PublisherName,
                publisherYare = booksFilterDtos.PublicationYare,
                minprice = booksFilterDtos.MinPrice,
                maxprice = booksFilterDtos.MaxPrice,
                categreyId = booksFilterDtos.CategoryId,
                IsHot = booksFilterDtos.isHot,
                book = books.Skip((page - 1) * 8).Take(8).ToList()
            };
            //pagination
            
            if (page < 0)
            {
                page = 1;
            }
            var Pagination = new
            {
                TotalNumberOfPage = Math.Ceiling(books.Count() / 8.0),
                CurantPage = page
            };
            return Ok(new
            {
                Returns,
                Pagination
            });
            
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] BooksCreateDtos booksCreateDtos)
        {
            var books = booksCreateDtos.Adapt<Book>();
            var newimges =new List<string>();
            if (booksCreateDtos.Images is not null && booksCreateDtos.Images.Count() > 0)
            {
                foreach (var item in booksCreateDtos.Images)
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Books", FileName);
                    //save image in wwwroot
                    using (var streem = System.IO.File.Create(FilePath))
                    {
                      await  item.CopyToAsync(streem);
                    }
                    newimges.Add(FilePath);
                }
               await _unitOfWork.BooksRepository.CreateAsync(books);
               await _unitOfWork.BooksRepository.CommitAsync();
                if (newimges is not null)
                {
                    foreach (var item in newimges)
                    {
                      await  _unitOfWork.ImageBooksRepository.CreateAsync(new ImageBook
                      {
                            BookId = books.Id,
                            UrlImage = item
                           
                        });
                    }
                }
              await  _unitOfWork.ImageBooksRepository.CommitAsync();
               
                return Ok("successfull add Book");
            }
            return NotFound();
        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _unitOfWork.BooksRepository.GetOneAsync(e => e.Id == id);
            if (book is not null)
            {
                return Ok(book);
            }
            return NotFound();
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] BooksUpdateDtos booksUpdateDtos)
        {
            var newimage = new List<string>();
            var books = booksUpdateDtos.Adapt<Book>();
            var booksold = await _unitOfWork.ImageBooksRepository.GetAsync(e => e.BookId == books.Id, tracked:false);
            if (booksold is not null)
            {
                if (booksUpdateDtos.Images is not null && booksUpdateDtos.Images.Count() > 0)
                {
                    foreach (var item in booksUpdateDtos.Images)
                    {
                        var FileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Books", FileName);
                        //save image in wwwroot
                        using (var streem = System.IO.File.Create(FilePath))
                        {
                            await item.CopyToAsync(streem);
                        }
                        newimage.Add(FilePath);
                    }
                    _unitOfWork.BooksRepository.Edit(books);
                    await _unitOfWork.BooksRepository.CommitAsync();


                    //delet image in db and wwwroot
                    foreach (var item in booksold)
                    {
                        var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Books", item.UrlImage);

                        // delet old image wwwroot
                        if (System.IO.File.Exists(OldFilePath))
                        {
                            System.IO.File.Delete(OldFilePath);
                        }
                        //delet old image in db
                        _unitOfWork.ImageBooksRepository.Delete(item);
                        await _unitOfWork.ImageBooksRepository.CommitAsync();
                    }


                    //save image in db
                    if (newimage is not null)
                    {
                        foreach (var item in newimage)
                        {
                         await _unitOfWork.ImageBooksRepository.CreateAsync(new ImageBook
                            {
                                BookId = books.Id,
                                UrlImage = item
                            });
                        }
                    }
                   await _unitOfWork.ImageBooksRepository.CommitAsync();

                }
                _unitOfWork.BooksRepository.Edit(books);
                await _unitOfWork.BooksRepository.CommitAsync();
               
                return Ok("Successfull Updage Book");
            }
            return BadRequest();
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _unitOfWork.BooksRepository.GetOneAsync(e => e.Id == id);
            var booksold = await _unitOfWork.ImageBooksRepository.GetAsync(e => e.BookId == book.Id, tracked: false);

            if (book is not null)
            {
                //delet image in db and wwwroot
                foreach (var item in booksold)
                {
                    var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Books", item.UrlImage);

                    // delet old image wwwroot
                    if (System.IO.File.Exists(OldFilePath))
                    {
                        System.IO.File.Delete(OldFilePath);
                    }
                    //delet old image in db
                    _unitOfWork.ImageBooksRepository.Delete(item);
                    await _unitOfWork.ImageBooksRepository.CommitAsync();
                }
                _unitOfWork.BooksRepository.Delete(book);
               await _unitOfWork.BooksRepository.CommitAsync();
                return Ok("successfull delete book");
            }
            return BadRequest();
        }

    }
}
