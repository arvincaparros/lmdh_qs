

namespace LMDH_QS.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; } // ⚠️ Hash this in production

        public string Role { get; set; } // e.g., "Staff", "Doctor"

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // Optional: FullName computed property
        public string FullName => $"{FirstName} {LastName}";

        public string? Department { get; set; }  // Specialist Optional (e.g., OPD, Dental, etc.)
    }

}
