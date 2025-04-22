using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class JobTitleRepository(AppDbContext context) : BaseRepository<JobTitleEntity, JobTitle>(context), IJobTitleRepository
{
}
