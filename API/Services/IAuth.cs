

using Domain.Entities;

namespace API.Services;

public interface IAuth
    {
        byte[] CreateQR(ref User user);
        bool VerifyCode(string secret, string code);
        Task SendEmail(User User, byte[] QR);
    }