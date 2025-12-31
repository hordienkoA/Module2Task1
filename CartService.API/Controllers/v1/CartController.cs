using CartService.BLL.Interfaces;
using CartService.Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartService.API.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _service;

        public CartController(ILogger<CartController> logger, ICartService cartService)
        {
            _logger = logger;
            _service = cartService;
        }

        /// <summary>
        /// Creates a new cart and returns the cart id.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        [HttpPost]
        public async Task<IActionResult> CreateCart(CancellationToken ct = default)
        {
            var id = await _service.CreateCartAsync(ct);
            return Ok(new { cartId = id });
        }

        /// <summary>
        /// Adds an item to the specified cart. If the cart does not exist it will be created.
        /// Returns 200 on success.
        /// </summary>
        [HttpPost("{cartId}/items")]
        public async Task<IActionResult> AddItemToCart(
        [FromBody] CartItem cartItem,
        [FromRoute] string cartId,
        CancellationToken ct = default)
        {
            try
            {
                await _service.AddItemAsync(cartId, cartItem, ct);
                return Ok(); // 200 - item added or cart created
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for cart {CartId}", cartId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get full cart model (cart id + list of items) for v1.
        /// Returns 200 with Cart or 404 if not found.
        /// </summary>
        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCart(
        [FromRoute] string cartId,
        CancellationToken ct = default)
        {
            try
            {
                var cart = await _service.GetCartAsync(cartId, ct);
                if (cart == null) return NotFound();
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid cart id: {CartId}", cartId);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an item from the specified cart. Returns 200 on success or 404 if not found.
        /// </summary>
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
                return BadRequest("Invalid cart id.");
            }
        }
    }
}
