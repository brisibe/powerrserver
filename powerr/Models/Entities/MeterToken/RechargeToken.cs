namespace powerr.Models.Entities.MeterToken
{
    public class RechargeToken
    {
        public int RechargeTokenId { get; set; }
        public long Token { get; set; }
        public int Value { get; set; }
        public bool HasBeenUsed { get; set; }  

    }
}
