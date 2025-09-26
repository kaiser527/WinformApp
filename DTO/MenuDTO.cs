namespace WinFormApp.DTO
{
    internal class MenuDTO
    {
        public string FoodName { get; set; }
        public int Count { get; set; }
        public float Price { get; set; }
        public float TotalPrice { get; set; }

        public MenuDTO(string foodName, int count, float price, float totalPrice) 
        { 
            FoodName = foodName;
            Count = count;
            Price = price;
            TotalPrice = totalPrice;
        }
    }
}
