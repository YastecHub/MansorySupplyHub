namespace MansorySupplyHub.Entities
{
    public class ShoppingCart : Product 
    {
        public int ProductId { get; set; }
        public int Sqft { get; set; }
        public int TempSqft { get; set; }
        public double PricePerSqFt { get; set; }


    }
}
