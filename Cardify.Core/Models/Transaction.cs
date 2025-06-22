using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardify.Core.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = ""; // income, expense, debt, etc.
    }
}
