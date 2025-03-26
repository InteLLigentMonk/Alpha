using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class ProjectsRepository(AppDbContext context) : BaseRepository<ProjectEntity>(context), IProjectsRepository
{
}
