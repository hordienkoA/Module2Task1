using CartService.Contracts.Interfaces;
using CartService.Contracts.Models;
using Moq;
using Service = CartService.BLL.Services.CartService;
namespace TestProject
{
    public class CartServiceTests
    {
        [Fact]
        public async Task AddItem_ValidItem_CallsRepository()
        {
            var repoMock = new Mock<ICartRepository>();
            var service = new Service(repoMock.Object);
            var item = new CartItem { Id = 1, Name = "P", Price = 10m, Quantity = 2 };

            await service.AddItemAsync("cart-1", item);
            repoMock.Verify(r => r.AddOrUpdateItemAsync("cart-1", It.Is<CartItem>(ci => ci.Id == 1 && ci.Quantity == 2), default), Times.Once);


        }

        [Fact]
        public async Task AddItem_Invalid_Throws()
        {
            var repoMock = new Mock<ICartRepository>();
            var service = new Service(repoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddItemAsync("", new CartItem()));
        }

        [Fact]
        public async Task GetItems_ForwardedToRepo()
        {
            var repoMock = new Mock<ICartRepository>();
            repoMock.Setup(r => r.GetItemsAsync("c1", default)).ReturnsAsync(new[] { new CartItem { Id = 1, Name = "A", Price = 1m, Quantity = 1 } });

            var service = new Service(repoMock.Object);
            var items = await service.GetItemsAsync("c1");
            Assert.Single(items);
        }
    }
}