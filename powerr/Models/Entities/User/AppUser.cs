

using Microsoft.AspNetCore.Identity;

namespace powerr.Api.Models.Entities.User
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }


        
    }
}
