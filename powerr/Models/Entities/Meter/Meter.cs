using powerr.Api.Models.Entities.User;
using powerr.Enums;

namespace powerr.Models.Entities.Meter
{
    public class Meter
    {
        public int Id { get; set; }
        public long MeterNumber { get; set; }

        public int Status { get; set; }  // 0 = Connected, 1 = Disconnected
        public DateTime Created { get; set; }

        public int AvailableUnit { get; set; }

        public bool IsAvailable { get; set; }

        public string DiscoName { get; set; }    
    }
}
