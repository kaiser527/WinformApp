using System;

namespace WinFormApp.DTO
{
    internal class BillDTO
    {
        public string TableName { get; set; }
        public float Discount { get; set; }
        public float TotalPrice { get; set; }
        public DateTime DateCheckIn { get; set; }
        public DateTime DateCheckOut { get; set; }

        public BillDTO(string tableName, float discount, float totalPrice, DateTime dateCheckIn, DateTime dateCheckOut) 
        {
            TableName = tableName;
            Discount = discount;
            DateCheckIn = dateCheckIn;
            DateCheckOut = dateCheckOut;
            TotalPrice = totalPrice;
        }
    }
}
