using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace powerr.Models.Entities.Wallet
{
    public class Wallet
    {
        public int Id { get; set; }

        [Precision(18,2)]
        public decimal Balance { get; set; }

        public string? UserId { get; set; }
    }
}
