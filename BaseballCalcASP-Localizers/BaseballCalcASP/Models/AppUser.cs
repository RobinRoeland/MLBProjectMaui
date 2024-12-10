using Microsoft.AspNetCore.Identity;

namespace BaseballCalcASP.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool deleted { get; set; }
    }
}
