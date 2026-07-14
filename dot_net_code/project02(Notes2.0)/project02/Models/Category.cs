using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace project02.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }


        public string UserId { get; set; }


        [Required]
        [MaxLength(100, ErrorMessage = "category cannot exceed more than 100 characters")]
        public string category { get; set; }

        public virtual IdentityUser? User { get; set; }

        public virtual ICollection<Notes> Notes { get; set; } = new List<Notes>();

    }
}
