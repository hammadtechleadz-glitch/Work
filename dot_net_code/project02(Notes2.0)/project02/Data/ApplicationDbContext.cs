using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using project02.Models;
namespace project02.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        //constructor allows program.cs to inject sql connection strings settings
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
   
        }
        //this property turns my Notes model into a sqlserver db table
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<NoteTag> NoteTag { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Notes>()
                .HasOne(n => n.category)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.categoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<NoteTag>()
                .HasKey(nt => new { nt.NoteId, nt.TagId });

            builder.Entity<NoteTag>()
                .HasOne(nt => nt.note)
                .WithMany(n => n.NoteTag)
                .HasForeignKey(nt => nt.NoteId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<NoteTag>()
                .HasOne(nt => nt.tag)
                .WithMany(t => t.NoteTag)
                .HasForeignKey(nt => nt.TagId)
                .OnDelete(DeleteBehavior.NoAction);// on delete no actions so manual delte to avoid any migratins amd databsesd issnneu of foeing key scontrations and ondelte cascsade issues
        }

    }
}
