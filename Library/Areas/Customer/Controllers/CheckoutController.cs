using Library.Migrations;
using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;



namespace Library.Areas.Customer.Controllers
{
    [Area("Checkout")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
      

        public CheckoutController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("Success/{orderId}")]
        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(e => e.Id == orderId);
            //update order
            if (order is null)
            {
                return NotFound();
            }
            order.orderStatus = OrderStatus.completed;
            var service = new SessionService();
            var session = service.Get(order.SessionId);
            order.TransactionId = session.PaymentIntentId;
            await _unitOfWork.OrderRepository.CommitAsync();

            // cart => orderitem
            var user = await _unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Userid is null)
                {
                    return NotFound();
                }
                user = await _unitOfWork.UserManager.FindByIdAsync(Userid);
            }
            if (user is null)
            {
                return NotFound();
            }
            var carts = await _unitOfWork.CartRepository.GetAsync(e => e.ApplicationUserId == user.Id);

            var orderItems = carts.Select(e => new OrderItem()
            {
                OrderId = orderId,
                BookId = e.BookId,
                Price = e.book.Price,
                Quantity = e.Count
            }).ToList();
            await _unitOfWork.OrderItemRepository.CreateRangeAsync(orderItems);
            await _unitOfWork.OrderItemRepository.CommitAsync();
            foreach (var item in carts)
            { // decrease quantity
                item.book.Quantity -= item.Count;
                //delet old cart
                _unitOfWork.CartRepository.Delete(item);


            }
            await _unitOfWork.CartRepository.CommitAsync();
            await _unitOfWork.BooksRepository.CommitAsync();

            return Ok();
        }

        [HttpPost("Cancel/{orderId}")]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(e => e.Id == orderId);
            if (order is null)
            {
                return NotFound();
            }
            order.orderStatus = OrderStatus.canceled;
            await _unitOfWork.OrderRepository.CommitAsync();
            return Ok(); 
        }
    }

}
