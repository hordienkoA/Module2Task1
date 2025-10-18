using CartService.BLL.Interfaces;
using CartService.Contracts.Interfaces;
using CartService.Contracts.Models;

namespace CartService.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;
        public CartService(ICartRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        public async Task AddItemAsync(string cartId, CartItem item, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cartId)) throw new ArgumentException("cartId required", nameof(cartId));
            ValidateItem(item);
            await _repo.AddOrUpdateItemAsync(cartId, item, ct);
        }

        public async Task<string> CreateCartAsync(CancellationToken ct = default)
        {
            return await _repo.CreateCartAsync(ct);
        }

        public async Task<IEnumerable<CartItem>> GetItemsAsync(string cartId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cartId)) throw new ArgumentNullException("cartId required", nameof(cartId));
            return await _repo.GetItemsAsync(cartId, ct);
        }

        public async Task RemoveItemAsync(string cartId, int itemId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cartId)) throw new ArgumentException("cartId required", nameof(cartId));
            await _repo.RemoveItemAsync(cartId, itemId, ct);
        }

        private void ValidateItem(CartItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Id <= 0) throw new ArgumentException("Item.Id must be > 0");
            if (string.IsNullOrWhiteSpace(item.Name)) throw new ArgumentException("Item.Name required");
            if (item.Price < 0) throw new ArgumentException("Item.Price must be >= 0");
            if (item.Quantity <= 0) throw new ArgumentException("Item.Quantity must be >= 1");
        }
    }
}
