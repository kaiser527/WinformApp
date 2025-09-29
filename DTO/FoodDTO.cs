namespace WinFormApp.DTO
{
    internal class FoodDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public float Price { get; set; }

        public FoodDTO(int id, string name, string category, float price) 
        { 
            Id = id;
            Name = name;
            Category = category;
            Price = price;
        }
    }
}
