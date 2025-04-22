using Data.Entities;
using Domain.Models;
namespace Data.Interfaces;

public interface IJobTitleRepository : IBaseRepository<JobTitleEntity, JobTitle>
{
}