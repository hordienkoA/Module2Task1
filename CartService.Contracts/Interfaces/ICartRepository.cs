using CartService.Contracts.Models;

namespace CartService.Contracts.Interfaces
{
    public interface ICartRepository
    {
        Task<string> CreateCartAsync(CancellationToken ct);
        Task<Cart?> GetCartAsync(string cartId, CancellationToken ct = default);
        Task<IEnumerable<CartItem>> GetItemsAsync(string cartId, CancellationToken ct = default);
        Task AddOrUpdateItemAsync(string cartId, CartItem item, CancellationToken ct = default);
        Task RemoveItemAsync(string cartId, int itemId, CancellationToken ct = default);
        Task DeleteCartAsync(string cartId, CancellationToken ct = default);
    }
}
