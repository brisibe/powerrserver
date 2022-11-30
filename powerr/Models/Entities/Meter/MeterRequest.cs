using powerr.Api.Models.Entities.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace powerr.Models.Entities.Meter
{
    public class MeterRequest
    {
        public int MeterRequestId { get; set; }

        public DateTime RequestedAt { get; set; }
        public Guid? UserId { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string LGA { get; set; }

        public int Status { get; set; }  //1 = new request, 2 = processing, 3 = approved

        public bool IsApproved { get; set; } = false;

        public virtual ICollection<Meter>? Meters { get; set; } = new List<Meter>();

    }
}



