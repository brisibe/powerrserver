

using Microsoft.AspNetCore.Identity;
using powerr.Models.Entities.Meter;
using powerr.Models.Entities.Wallet;

namespace powerr.Api.Models.Entities.User
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }


       public virtual ICollection<MeterRequest> MeterRequest { get; set; }
       
    }
}
