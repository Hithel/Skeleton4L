

using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
    public class UserRepository : GenericRepository<User>, IUser
    {
        private readonly ApiContext _context;

        public UserRepository(ApiContext context) : base(context)
        {
            _context = context;
        }

         public async virtual Task<bool> IsExists (string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var exists = await _context.Users.AnyAsync(p => p.Username == userName);
                return exists; 
            }

            return false;
        }


        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.Rols)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Rols)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .ToListAsync();
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
            .FirstOrDefaultAsync(p =>  p.Id == id);
        }

        public override async Task<(int totalRegistros, IEnumerable<User> registros)> GetAllAsync(int pageIndez, int pageSize, string search)
        {
            var query = _context.Users as IQueryable<User>;

            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Username.ToLower().Contains(search));
            }

            query = query.OrderBy(p => p.Id);
            var totalRegistros = await query.CountAsync();
            var registros = await query 
                .Include(u => u.Rols)
                .Skip((pageIndez - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalRegistros, registros);
        }
    }