using Domain.Entities;

namespace Domain.Interfaces;
    public interface IUser : IGenericRepo<User> 
    { 
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByRefreshTokenAsync(string username);
        Task<bool> IsExists (string userName);
    }
