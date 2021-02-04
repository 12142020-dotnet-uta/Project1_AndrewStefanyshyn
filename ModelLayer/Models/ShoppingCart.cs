namespace ModelLayer.Models
{
    public interface IShoppingCart
    {
        public void AddToCart(long productId, int quantity);
        public void DeleteFromCart();
        public void DeleteFromCart(long productId);
    }
}