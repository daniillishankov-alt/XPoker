using Xunit;
using XPoker;
using System.Linq;

namespace XPoker.Tests
{
    public class ShopTests
    {
        [Fact]
        public void Shop_GetRandomShop_ReturnsCorrectCount()
        {
            var shop = new Shop();
            var items = shop.GetRandomShop(3);
            Assert.Equal(3, items.Count);
        }

        [Fact]
        public void Shop_GetRandomShop_ReturnsValidEnhancements()
        {
            var shop = new Shop();
            var items = shop.GetRandomShop(3);

            Assert.All(items, item => Assert.NotNull(item.Name));
            Assert.All(items, item => Assert.True(item.Cost > 0));
        }

        [Fact]
        public void Shop_GetRandomShop_NeverReturnsMoreThanAvailable()
        {
            var shop = new Shop();
            var items = shop.GetRandomShop(100);
            Assert.True(items.Count <= 8); // 8 items in shop
        }

        [Fact]
        public void Shop_GetRandomShop_ReturnsEmptyListForZeroCount()
        {
            var shop = new Shop();
            var items = shop.GetRandomShop(0);
            Assert.Empty(items);
        }

        [Fact]
        public void Shop_GetEnhancementByIndex_ReturnsCorrectItem()
        {
            var shop = new Shop();
            var items = shop.GetRandomShop(3);
            var item = shop.GetEnhancementByIndex(0, items);

            Assert.NotNull(item);
            Assert.Equal(items[0], item);
        }

        [Fact]
        public void Shop_GetEnhancementByIndex_ReturnsNullForInvalidIndex()
        {
            var shop = new Shop();
            var items = shop.GetRandomShop(3);
            var item = shop.GetEnhancementByIndex(10, items);

            Assert.Null(item);
        }
    }
}