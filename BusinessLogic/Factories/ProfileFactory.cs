using Data.Entities;

namespace BusinessLogic.Factories;

public class ProfileFactory
{
    public static ProfileEntity NewProfileEntity()
    {
        var profile = new ProfileEntity()
        {
            Id = Guid.NewGuid(),
        };

        return profile;
    }
}
