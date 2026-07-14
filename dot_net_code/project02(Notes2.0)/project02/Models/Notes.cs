using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
// this is for the validattion keywords
namespace project02.Models
{
    public class Notes
    {
        [Key]// EF core will make it a primary key espciealli whe it int and name is id
        public int id { get; set; }

        [Required]
        [MaxLength(100,ErrorMessage="title cannot be more than 100 characters")]
        public string title { get; set; }

        [Required]
        [MaxLength(200 ,ErrorMessage="content cannot exceed more than 200 characters")]
        public string content { get; set; }

        
        public string UserId { get; set; }

        public virtual IdentityUser? User { get; set; }

        public int? categoryId { get; set; }

        public virtual Category? category { get; set; }

        public virtual ICollection<NoteTag> NoteTag { get; set; } = new List<NoteTag>();// we can creae a note without lnin it ot a tag
    }
}
