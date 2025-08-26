using Library.DTOS.order;
using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(OrdersFiltersDtos? ordersFiltersDtos , int page =1)
        {
            var orders = await _unitOfWork.OrderRepository.GetAsync(includes: [e => e.applicationUser]);
            // filters 
             if (ordersFiltersDtos.UserName is not null)
            {
                orders = orders.Where(e => e.applicationUser.UserName.Contains(ordersFiltersDtos.UserName));

            }
             if (ordersFiltersDtos.MinPrice is not null)
            {
                orders = orders.Where(e => e.TotalPrice > ordersFiltersDtos.MinPrice);
            }
            if (ordersFiltersDtos.MaxPrice is not null)
            {
                orders = orders.Where(e => e.TotalPrice < ordersFiltersDtos.MaxPrice);
            }
            if (ordersFiltersDtos.FromDate is not null)
            {
                orders = orders.Where(e => e.Date.Date > ordersFiltersDtos.FromDate.Value.Date);
            }
            if (ordersFiltersDtos.Todate is not null)
            {
                orders = orders.Where(e => e.Date.Date < ordersFiltersDtos.Todate);
            }
            if (ordersFiltersDtos.paymentMethod is not null)
            {
                orders = orders.Where(e => e.paymentMethod == ordersFiltersDtos.paymentMethod);
            }
            if (ordersFiltersDtos.orderStatus is not null)
            {
                orders = orders.Where(e => e.orderStatus == ordersFiltersDtos.orderStatus);
            }
            var Returns = new
            {
                FilterUserName = ordersFiltersDtos.UserName,
                FiltersMaxprice = ordersFiltersDtos.MaxPrice,
                FiltersMinPrice = ordersFiltersDtos.MinPrice,
                FiltersFromDate = ordersFiltersDtos.FromDate,
                FiltersToDate = ordersFiltersDtos.Todate,
                FilterOrsedsStutas = ordersFiltersDtos.orderStatus,
                FiltersPaymant = ordersFiltersDtos.paymentMethod,
                order = orders.Skip((page - 1) * 8).Take(8).ToList()
            };
            //pagination
            if (page < 0)
            {
                page = 1;
            }
            var pagination = new
            {
                TotalNUmberOfPage = Math.Ceiling(orders.Count() / 8.0),
                CurrentPage = page
            };
            return Ok(new
            {
                Returns,
                pagination
            });

        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Detalis(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.applicationUser]);
            if (order is not null)
            {
                return Ok(order);
            }
            return NotFound();

        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.applicationUser]);
            if (order is null)
            {
                return NotFound(new { massge = "Not Found Order matching this is Id" });
            }
            try
            {
                _unitOfWork.OrderRepository.Delete(order);
                await _unitOfWork.OrderRepository.CommitAsync();
                return Ok("Successfull Delete Orders");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });

            }
        }
        [HttpPatch("Completed/{id}")]
        public async Task<IActionResult> Completed(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.applicationUser]);

            if (order is null)
            {
                return NotFound();
            }
            order.orderStatus = OrderStatus.completed;
           
          await  _unitOfWork.OrderRepository.CommitAsync();
            return Ok("Update Successfull");

        }
        [HttpPatch("Shipped/{id}")]
        public async Task<IActionResult> Shipped(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.applicationUser]);

            if (order is null)
            {
                return NotFound();
            }
            order.orderStatus = OrderStatus.shipped;

            await _unitOfWork.OrderRepository.CommitAsync();
            return Ok("Update Successfull");

        }
        [HttpPatch("Canceled/{id}")]
        public async Task<IActionResult> Canceled(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.applicationUser]);

            if (order is null)
            {
                return NotFound();
            }
            order.orderStatus = OrderStatus.canceled;

            await _unitOfWork.OrderRepository.CommitAsync();
            return Ok("Update Successfull");

        }

    }
}
