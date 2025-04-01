using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class ProjectEntity
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string? ProjectPhotoUrl { get; set; }


        public ICollection<AppUser> Users { get; set; } = [];
    }
}
