namespace Cardify.Core.Models
{
    public class MonthlyData
    {
        public string Month { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }

        public MonthlyData(string month, decimal income, decimal expenses)
        {
            Month = month;
            Income = income;
            Expenses = expenses;
        }
    }

}