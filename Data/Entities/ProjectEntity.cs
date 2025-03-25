using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class ProjectEntity
    {
        public Guid Id { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }


        public IEnumerable<AppUser> Users { get; } = [];
    }
}
