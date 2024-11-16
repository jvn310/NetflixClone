using NetflixClone.Models;
using NetflixClone.Data;

namespace NetflixClone.Services
{
    public class ProfileService
    {
        private readonly NetflixCloneDbContext _context;

        public ProfileService(NetflixCloneDbContext context)
        {
            _context = context;
        }

        public List<Profile> GetAllProfiles()
        {
            return _context.Profiles.ToList();
        }

        public List<Profile> GetProfilesByUserId(int userId)
        {
            var profiles = _context.Profiles.Where(p => p.UserId == userId).ToList();

            return profiles ?? new List<Profile>();
        }

        public Profile GetProfileById(int profileId)
        {
            var profile = _context.Profiles.FirstOrDefault(p => p.Id == profileId);
            return profile ?? throw new InvalidOperationException("Profile not found");
        }
    }
}
