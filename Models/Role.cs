using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AnimeVnInfoBackend.Models
{
    public class Role : IdentityRole<int>
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // List of tables connected
        public List<AdminNavigation>? AdminNavigation { get; set; }
    }
}
