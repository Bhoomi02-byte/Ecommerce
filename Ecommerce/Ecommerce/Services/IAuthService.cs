using Ecommerce.DTO;
using Ecommerce.Utilities;

namespace Ecommerce.Services
{
    public interface IAuthService
    {
        Task<string> SignupEmailAsync(EmailSignupDto dto);
        Task<string> SendOtpAsync(PhoneOtpRequestDto dto);
        Task<object> SignupPhoneAsync(PhoneSignupDto dto);
        Task<string> LoginEmailAsync(EmailLoginDto dto);
        Task<object?> LoginSendOtpAsync(PhoneOtpRequestDto dto);
        Task<string> LogoutAsync(LogoutDto dto);
        Task<object?> LoginPhoneAsync(PhoneLoginDto dto);
    }
}
