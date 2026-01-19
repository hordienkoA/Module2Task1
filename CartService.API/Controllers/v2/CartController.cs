using CartService.BLL.Interfaces;
using CartService.Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartService.API.Controllers.v2
{
    [ApiController]
    [Route("v2/[controller]")]
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
        public async Task<IActionResult> CreateCart(CancellationToken ct = default)
        {
            var id = await _service.CreateCartAsync(ct);
            return Ok(new { cartId = id });
        }

        [HttpPost("{cartId}/items")]
        public async Task<IActionResult> AddItemToCart(
        [FromBody] CartItem cartItem,
        [FromRoute] string cartId,
        CancellationToken ct = default)
        {
            try
            {
                await _service.AddItemAsync(cartId, cartItem, ct);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for cart {CartId}", cartId);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartItems(
        [FromRoute] string cartId,
        CancellationToken ct = default)
        {
            try
            {
                var items = await _service.GetItemsAsync(cartId, ct);
                return Ok(items);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{cartId}/items/{itemId:int}")]
        public async Task<IActionResult> DeleteItem(
        [FromRoute] string cartId,
        [FromRoute] int itemId,
        CancellationToken ct = default)
        {
            try
            {
                await _service.RemoveItemAsync(cartId, itemId, ct);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}