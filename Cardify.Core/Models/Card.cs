using System;
using System.Collections.Generic;

namespace Cardify.Core.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string CardType { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string CardColorStart { get; set; } = "#FFFFFF";
        public string CardColorEnd { get; set; } = "#CCCCCC";
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }
} 