using CartService.BLL.Interfaces;
using CartService.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
namespace CartService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _service;
        public CartController(ILogger<CartController> logger, ICartService cartService)
        {
            _logger = logger;
            _service = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart(CancellationToken ct)
        {
            var id = await _service.CreateCartAsync(ct);
            return Ok(new { cartId = id });
        }

        [HttpPost("{cartId}/items")]
        public async Task<IActionResult> AddItemToCart(CartItem cartItem, [FromRoute] string cartId, CancellationToken ct)
        {
            await _service.AddItemAsync(cartId, cartItem, ct);
            return Ok();
        }

        [HttpGet("{cartId}/items")]
        public async Task<IActionResult> GetItems([FromRoute]string cartId, CancellationToken ct)
        {
            var items = await _service.GetItemsAsync(cartId, ct);
            return Ok(items);
        }

        [HttpDelete("{cartId}/items/{itemId:int}")]
        public async Task<IActionResult> DeleteItem([FromRoute]string cartId, [FromRoute]int itemId, CancellationToken ct)
        {
            await _service.RemoveItemAsync(cartId, itemId, ct);
            return Ok();
        }
    }
}
