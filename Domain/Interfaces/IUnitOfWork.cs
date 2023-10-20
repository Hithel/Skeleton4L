

namespace Domain.Interfaces;
    public interface  IUnitOfWork
    {
        IRol Roles { get; }
        IUser Users { get; }
        Task<int> SaveAsync();
    }
