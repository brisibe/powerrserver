namespace powerr.Models.Dtos
{
    public class GenerateRechargeTokenDto
    {
        public int WalletId { get; set; }
        public int Value { get; set; } //how much meter units is being bought
    }
}
