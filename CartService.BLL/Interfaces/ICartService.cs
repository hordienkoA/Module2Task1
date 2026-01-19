
using CartService.Contracts.Models;

namespace CartService.BLL.Interfaces
{
    public interface ICartService
    {
        Task<Cart?> GetCartAsync(string cartId, CancellationToken ct = default);
        Task<IEnumerable<CartItem>> GetItemsAsync(string cartId, CancellationToken ct = default);
        Task AddItemAsync(string cartId, CartItem item, CancellationToken ct = default);
        Task RemoveItemAsync(string cartId, int itemId, CancellationToken ct = default);
        Task<string> CreateCartAsync(CancellationToken ct = default);
        Task UpdateItemAsync(int itemId, string name, decimal price, CancellationToken ct = default);
    }
}