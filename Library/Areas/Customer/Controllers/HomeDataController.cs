using Library.DTOS.Books;
using Library.Repositoirs.IRepositoirs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class HomeDataController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeDataController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(BooksFilterDtos? booksFilterDtos , int page =1)
        {
            var books = await _unitOfWork.BooksRepository
                .GetAsync(includes: [e => e.category, e => e.author, e => e.publisher, e => e.imageBooks]);
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
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _unitOfWork.BooksRepository.GetOneAsync(e => e.Id == id, includes: [e => e.category]);
               
            if (book is not null)
            {
                var relatedBooks = (await _unitOfWork.BooksRepository
                    .GetAsync(e => e.CategoryId == book.CategoryId &&
                    e.Id != book.Id, includes: [e => e.category])).Skip(0).Take(4);
                var similarbook = (await _unitOfWork.BooksRepository
                    .GetAsync(e => e.Name.Contains(book.Name)&& e.Id != book.Id, includes: [e => e.category]))
                    .Skip(0).Take(4);
                var ratebook = (await _unitOfWork.BooksRepository
                    .GetAsync(e => e.Id != book.Id, includes: [e => e.category]))
                    .OrderByDescending(e => e.Rate)
                    .Skip(0).Take(4);
                var Returns = new
                {
                    Book = book,
                    RelatedBooks = relatedBooks.ToList(),
                    SimilarBooks = similarbook.ToList(),
                    RateBooks = ratebook.ToList()
                };
                return Ok(Returns);
               
               
            }

            return NotFound();
        }
    }
}
