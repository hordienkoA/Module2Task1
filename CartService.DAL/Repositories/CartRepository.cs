using CartService.Contracts.Interfaces;
using CartService.Contracts.Models;
using LiteDB;

namespace CartService.DAL.Repositories
{
    public class CartRepository : ICartRepository, IDisposable
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<Cart> _carts;

        public CartRepository(string filename)
        {
            _db = new LiteDatabase(filename);
            _carts = _db.GetCollection<Cart>("carts");
        }

        public CartRepository(Stream stream)
        {
            _db = new LiteDatabase(stream);
            _carts = _db.GetCollection<Cart>("carts");
        }

        public Task AddOrUpdateItemAsync(string cartId, CartItem item, CancellationToken ct = default)
        {
            var cart = _carts.FindOne(c => c.Id == cartId) ?? new Cart { Id = cartId };
            var existing = cart.Items.FirstOrDefault(i => i.Id == item.Id);

            if (existing != null)
            {
                existing.Quantity = item.Quantity;
                existing.Name = item.Name;
                existing.Price = item.Price;
                existing.Currency = item.Currency;
                existing.Image = item.Image;
            }

            else
            {
                cart.Items.Add(item);
            }

            _carts.Upsert(cart);
            return Task.CompletedTask;
        }

        public Task<string> CreateCartAsync(CancellationToken ct)
        {
            var cart = new Cart { Id = Guid.NewGuid().ToString() };
            _carts.Insert(cart);
            return Task.FromResult(cart.Id);
        }

        public Task DeleteCartAsync(string cartId, CancellationToken ct = default)
        {
            _carts.DeleteMany(c => c.Id == cartId);
            return Task.CompletedTask;
        }

        public void Dispose() => _db?.Dispose();


        public Task<Cart?> GetCartAsync(string cartId, CancellationToken ct = default)
        {
            var cart = _carts.FindOne(c => c.Id == cartId);
            return Task.FromResult(cart);
        }

        public Task<IEnumerable<CartItem>> GetItemsAsync(string cartId, CancellationToken ct = default)
        {
            var cart = _carts.FindOne(c => c.Id == cartId);
            return Task.FromResult(cart?.Items ?? Enumerable.Empty<CartItem>());
        }

        public Task RemoveItemAsync(string cartId, int itemId, CancellationToken ct = default)
        {
            var cart = _carts.FindOne(c => c.Id == cartId);
            if (cart == null) return Task.CompletedTask;

            var item = cart.Items.FirstOrDefault(c => c.Id == itemId);
            if (item != null)
            {
                cart.Items.Remove(item);
                _carts.Update(cart);
            }
            return Task.CompletedTask;
        }

        public Task UpdateItemsByProductIdAsync(int productId, string newName, decimal newPrice, CancellationToken ct = default)
        {
            var carts = _carts.FindAll().ToList();

            bool changed = false;

            foreach (var cart in carts)
            {
                foreach (var item in cart.Items)
                {
                    if (item.Id == productId)
                    {
                        item.Name = newName;
                        item.Price = newPrice;

                        changed = true;
                    }
                }

                if (changed)
                {
                    _carts.Update(cart);
                    changed = false;
                }
            }

            return Task.CompletedTask;
        }
    }
}