using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace project02.Models
{
    public class Tag
    {
        [Key]
        public int id { get; set; }


        [Required]
        [MaxLength(20, ErrorMessage = "tag cannot exceed more than 20 characters")]
        public string name { get; set; }

        public string UserId { get; set; }

        public virtual IdentityUser? User { get; set; }

        public virtual ICollection<NoteTag> NoteTag { get; set; } = new List<NoteTag>();// this can be empty wehn creatd a tag obj
    }
}
