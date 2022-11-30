using powerr.Api.Models.Entities.User;

namespace powerr.Models.Dtos
{
    public class MeterRequestDto
    {

        public Guid UserId { get; set; }    

        public string Address { get; set; }

        public string LGA { get; set; }

        
    }
}
