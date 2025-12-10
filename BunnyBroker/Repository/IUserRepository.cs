using BunnyBroker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BunnyBroker.Repository
{
    public interface IUserRepository {
	    Task<User> AddUserAsync(string userName, string password, CancellationToken ct = default);
	    Task UpdatePassword(string userName, string password, CancellationToken ct = default);
        Task<IEnumerable<User>> GetAllUsers(CancellationToken ct = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<bool> CheckUserAsync(string username, string password, CancellationToken ct = default);
    }

    public class UserRepository(BunnyDbContext context) : IUserRepository {

	    public async Task<User> AddUserAsync(string userName, string password, CancellationToken ct = default) {
		    var user = new User {
                Username = userName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            context.Users.Add(user);
            await context.SaveChangesAsync(ct);
            return user;
	    }
        public async Task UpdatePassword(string userName, string password, CancellationToken ct = default) {
		    var user = await context.Users.FirstOrDefaultAsync(u => u.Username == userName, ct);
            if(user == null) {
                throw new KeyNotFoundException($"User with username {userName} not found.");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await context.SaveChangesAsync(ct);
        }
        public async Task<IEnumerable<User>> GetAllUsers(CancellationToken ct = default) {
		    return await context.Users
			    .AsNoTracking()
			    .ToListAsync(ct);
        }
        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) {
		    return await context.Users
			    .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username, ct);
        }

        public async Task<bool> CheckUserAsync(string username, string password, CancellationToken ct = default) {
	        var hasUsers = await context.Users.AnyAsync(ct);
            if(!hasUsers) {
	            await AddUserAsync("admin", "admin", ct);
            }
            
            var user = await context.Users
	            .AsNoTracking()
	            .FirstOrDefaultAsync(u => u.Username == username, ct);
            if(user == null) {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}
