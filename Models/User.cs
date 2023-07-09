using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AnimeVnInfoBackend.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
        public bool IsDisabled { get; set; }

        // List of tables connected
        public UserInfo? UserInfo { get; set; }
        public List<Review>? Review { get; set; }
    }
}
