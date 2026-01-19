using CartService.Contracts.Models;
using CartService.DAL.Repositories;

namespace TestProject
{

    public class DBIntegrationTests
    {
        [Fact]
        public async Task AddAndGetItem_Works()
        {
            using var ms = new MemoryStream();
            using var repo = new CartRepository(ms);

            var cartId = "cart-test-1";
            var item = new CartItem { Id = 100, Name = "Product", Price = 9.99m, Quantity = 1 };

            await repo.AddOrUpdateItemAsync(cartId, item);
            var items = await repo.GetItemsAsync(cartId);

            Assert.Single(items);
            Assert.Equal(100, items.First().Id);
        }

        [Fact]
        public async Task RemoveItem_Works()
        {
            using var ms = new MemoryStream();
            using var repo = new CartRepository(ms);

            var cartId = "cart-test-2";
            var item = new CartItem { Id = 200, Name = "P", Price = 1m, Quantity = 1 };
            await repo.AddOrUpdateItemAsync(cartId, item);

            await repo.RemoveItemAsync(cartId, 200);
            var items = await repo.GetItemsAsync(cartId);
            Assert.Empty(items);
        }
    }
}