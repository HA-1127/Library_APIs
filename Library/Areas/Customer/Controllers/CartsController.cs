using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Library.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart( int Bookid, int count)
        {
            var user = await _unitOfWork.UserManager.GetUserAsync(User);
            if (user is  null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId is null)
                    return NotFound();

                user = await _unitOfWork.UserManager.FindByIdAsync(userId);
            }
            var book = await _unitOfWork.BooksRepository.GetOneAsync(e => e.Id == Bookid);
            if (book is null && book.Quantity > count)
            {
                return BadRequest("Count is Valid");
            }
            var cart = await _unitOfWork.CartRepository.GetOneAsync(e => e.BookId == Bookid && e.ApplicationUserId == user.Id);
            if (cart is not null)
            {
                cart.Count += count;
            }
            else
            {
                await _unitOfWork.CartRepository.CreateAsync(new()
                {
                    BookId = Bookid,
                    ApplicationUserId = user.Id,
                    Count = count,

                });
            }
           
           await _unitOfWork.CartRepository.CommitAsync();
            return Ok("Successfull add book to cart");

        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var user = await _unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Userid is null)
                    return NotFound();
                user = await _unitOfWork.UserManager.FindByIdAsync(Userid);
            }
            if (user is null)
            {
                return NotFound();
            }
            var cart = await _unitOfWork.CartRepository.GetAsync(e => e.ApplicationUserId == user.Id , includes: [e=>e.book]);
            var TotallPrice = cart.Sum(e => e.book.Price * e.Count);
            return Ok(new
            {
                cart,
                TotallPrice

            });
        }
        [HttpPatch("Incremant/{Bookid}")]
        public async Task<IActionResult> Incremant(int Bookid )
        {
            var user = await _unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Userid is null)
                    return NotFound();
                user = await _unitOfWork.UserManager.FindByIdAsync(Userid);
            }
            if (user is null)
            {
                return NotFound();
            }
           
            var cartdb = await _unitOfWork.CartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id&& e.BookId ==Bookid);
            if (cartdb is null)
                return BadRequest("Not book in cart");
            else
            {
                cartdb.Count++;
            }
            await _unitOfWork.CartRepository.CommitAsync();
            return NoContent();
        }
        [HttpPatch("Decremant/{Bookid}")]
        public async Task<IActionResult> Decremant(int Bookid)
        {
            var user = await _unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Userid is null)
                    return NotFound();
                user = await _unitOfWork.UserManager.FindByIdAsync(Userid);
            }
            if (user is null)
            {
                return NotFound();
            }
           
            var cartdb = await _unitOfWork.CartRepository.GetOneAsync(e => e.BookId == Bookid && e.ApplicationUserId == user.Id);
            if (cartdb is null)
                return BadRequest("cart is vaild");
            if(cartdb.Count>1)
            cartdb.Count--;
            await _unitOfWork.CartRepository.CommitAsync();
            return NoContent();

        }
        [HttpDelete("Delete/{Bookid}")]
        public async Task<IActionResult> Delete(int Bookid)
        {
            var user = await _unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Userid is null)
                    return NotFound();
                user = await _unitOfWork.UserManager.FindByIdAsync(Userid);
            }
            if (user is null)
            {
                return NotFound();
            }
            var cartdb = await _unitOfWork.CartRepository.GetOneAsync(e => e.BookId == Bookid && e.ApplicationUserId == user.Id);
            if (cartdb is null)
            {
                return BadRequest("cart is vaild");
            }
            _unitOfWork.CartRepository.Delete(cartdb);
            await _unitOfWork.CartRepository.CommitAsync();
            return NoContent();
        }
        [HttpPost("Pay")]
        public async Task<IActionResult> Pay()
        {
            var user = await _unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Userid is null)
                    return NotFound();
                user = await _unitOfWork.UserManager.FindByIdAsync(Userid);
            }
            if (user is null)
            {
                return NotFound();
            }
            var carts = await _unitOfWork.CartRepository.GetAsync(e => e.ApplicationUserId == user.Id);
            await _unitOfWork.OrderRepository.CreateAsync(new()
            {
                ApplicationUserId = user.Id,
                Date = DateTime.UtcNow,
                orderStatus = OrderStatus.pending,
                paymentMethod = PaymentMethod.Visa,
                TotalPrice = carts.Sum(e=>e.book.Price * e.Count)
            });
           await _unitOfWork.CartRepository.CommitAsync();

            var order = (await _unitOfWork.OrderRepository.GetAsync(e => e.ApplicationUserId == user.Id))
                .OrderBy(e => e.Id).LastOrDefault();
            if (order is null)
            {
                return NotFound();
            }
           
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel?orderId={order.Id}",
            };

            foreach (var item in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.book.Name,
                            Description = item.book.Discription
                        },
                        UnitAmount = (long)item.book.Price * 100, // 400.00
                    },
                    Quantity = item.Count,
                });
            }


            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            await _unitOfWork.OrderRepository.CommitAsync();
            return Ok(new
            {
                url = session.Url
            });
        }



    }
}
