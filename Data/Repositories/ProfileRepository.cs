using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class ProfileRepository(AppDbContext context) : BaseRepository<ProfileEntity>(context), IProfileRepository
{
}
