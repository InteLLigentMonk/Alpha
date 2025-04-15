using Data.Entities;
using Domain.Models;
namespace Data.Interfaces;

public interface IProfileRepository : IBaseRepository<ProfileEntity, Profile>
{
}