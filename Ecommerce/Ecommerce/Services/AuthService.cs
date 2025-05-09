using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly GenerateJwtToken _token;
        private readonly EmailService _emailService;

        public AuthService(ApplicationDbContext context, GenerateJwtToken token,EmailService emailService)
        {
            _context = context;
            _token = token;
            _emailService = emailService;
        }

        public async Task<string> SignupEmailAsync(EmailSignupDto dto)
        {
            var existingEmail = await _context.Users.AnyAsync(x => x.Email == dto.Email);
            if (existingEmail) return "Email already registered";
            

            var existingPhone = await _context.Users.AnyAsync(x => x.PhoneNumber == dto.PhoneNumber);
            if (existingPhone)  return "Phone number already registered";
            

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                PhoneNumber=dto.PhoneNumber,
                Role = dto.Role
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return "User signup successfully";
        }
        public async Task<string> SendOtpAsync(PhoneOtpRequestDto dto)
        {
            var phoneNumber = await _context.Users.AnyAsync(x => x.PhoneNumber == dto.PhoneNumber);
            if (phoneNumber) return "PhoneNumber is already exist";

            var existingOtp = await _context.Otps
            .FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);

            var otp = "123456"; 
            var expiry = DateTime.UtcNow.AddMinutes(5);

            if (existingOtp != null)
            {
                existingOtp.OtpCode = otp;
                existingOtp.ExpiryTime = expiry;
                _context.Otps.Update(existingOtp);
            }
            else
            {
                var newOtp = new Otp
                {
                    PhoneNumber = dto.PhoneNumber,
                    OtpCode = otp,
                    ExpiryTime = expiry
                };

                _context.Otps.Add(newOtp);
            }

          
            await _context.SaveChangesAsync();
            return "OTP sent";
        }

        public async Task<object?> SignupPhoneAsync(PhoneSignupDto dto)
        {
            var otpEntry = await _context.Otps
                .Where(o => o.PhoneNumber == dto.PhoneNumber && o.OtpCode == dto.Otp)
                .OrderByDescending(o => o.ExpiryTime)
                .FirstOrDefaultAsync();

            if (otpEntry == null || otpEntry.ExpiryTime < DateTime.UtcNow)
                return null;

            var userExists = await _context.Users.AnyAsync(x => x.PhoneNumber == dto.PhoneNumber);
            if (userExists)
                return "Phone number already registered";

            var user = new User
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var accessToken = _token.GenerateToken(user, "Access");
            var refreshToken = _token.GenerateToken(user, "Refresh");

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                DeviceId = Guid.NewGuid().ToString()
            };
            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                user = new { user.Id, user.Name, user.PhoneNumber, user.Role },
                accessToken,
                refreshToken
            };

            return responseData;
        }

        public async Task<object>LoginEmailAsync(EmailLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !VerifyPassword(dto.Password, user.Password))
                return null;

            var accessToken = _token.GenerateToken(user, "Access");
            var refreshToken = _token.GenerateToken(user, "Refresh");

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                DeviceId = Guid.NewGuid().ToString()
            };
            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                user = new { user.Id, user.Name, user.PhoneNumber, user.Role },
                accessToken,
                refreshToken
            };

            return responseData;

        }
      
        public async Task<string> LogoutAsync(LogoutDto dto)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.DeviceId == dto.DeviceId && rt.UserId == dto.UserId);

            if (refreshToken == null)
                return null;

            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();

            return "Logged out from device successfully.";
        }
        public async Task<object?> LoginSendOtpAsync(PhoneOtpRequestDto dto)
        {
            var existingUser = await _context.Users
           .FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (existingUser == null)
                return null;

            var existingOtp = await _context.Otps
           .FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);

            var otp = "123456";
            var expiry = DateTime.UtcNow.AddMinutes(5);

            if (existingOtp != null)
            {
                existingOtp.OtpCode = otp;
                existingOtp.ExpiryTime = expiry;
                _context.Otps.Update(existingOtp);
            }
            else
            {
                var newOtp = new Otp
                {
                    PhoneNumber = dto.PhoneNumber,
                    OtpCode = otp,
                    ExpiryTime = expiry
                };

                _context.Otps.Add(newOtp);
            }
            await _context.SaveChangesAsync();
            return "OTP sent";
        }

        public async Task<object?> LoginPhoneAsync(PhoneLoginDto dto)
        {
            var otpEntry = await _context.Otps.FirstOrDefaultAsync(x =>
           x.PhoneNumber == dto.PhoneNumber &&
           x.OtpCode == dto.OtpCode &&
           x.ExpiryTime > DateTime.UtcNow);

            if (otpEntry == null)
                return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (user == null)
                return null;

            var accessToken = _token.GenerateToken(user, "Access");
            var refreshToken = _token.GenerateToken(user, "Refresh");

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                DeviceId = Guid.NewGuid().ToString()
            });

            await _context.SaveChangesAsync();

            var responseData = new
            {
                accessToken,
                refreshToken
            };

            return responseData;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return false;

            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddHours(1);

            var resetToken = new PasswordResetToken
            {
                Token = token,
                UserId = user.Id,
                ExpiryTime = expiry
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var resetLink = $"https://Ecommerce.com/reset-password?token={token}";
            await _emailService.SendEmailAsync(user.Email, "Reset Password", $"Click to reset: {resetLink}");

            return true;
        }
        public async Task<object?> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == dto.Token && x.ExpiryTime > DateTime.UtcNow);

            if (resetToken == null) return null;

            var user = resetToken.User;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.Password = hashedPassword;

            _context.PasswordResetTokens.Remove(resetToken);

            var existingTokens = _context.RefreshTokens.Where(rt => rt.UserId == user.Id);
            _context.RefreshTokens.RemoveRange(existingTokens);

            var accessToken = _token.GenerateToken(user, "Access");
            var refreshToken = _token.GenerateToken(user, "Refresh");

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                DeviceId = Guid.NewGuid().ToString()
            });

            await _context.SaveChangesAsync();
            var responseData = new
            {
                accessToken,
                refreshToken
            };

            return responseData;
        }

        private bool VerifyPassword(string enteredPassword, string storedHashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashPassword);
        }



    }
}
    
