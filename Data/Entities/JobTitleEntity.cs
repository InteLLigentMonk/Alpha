namespace Data.Entities;

public class JobTitleEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public ICollection<ProfileEntity> Profiles { get; set; } = [];
}
