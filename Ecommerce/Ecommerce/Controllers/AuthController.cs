using System.Numerics;
using Ecommerce.DTO;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup/email")]
        public async Task<IActionResult> SignupEmail([FromBody] EmailSignupDto dto)
        {
            var user = await _authService.SignupEmailAsync(dto);

            if (user == "User signup successfully")
            {
                return Ok(new ApiResponse(200,true,user,null));
            }

            return BadRequest(new ApiResponse(400, false, user, null));
        }
        [HttpPost("signup/send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] PhoneOtpRequestDto dto)
        {
            var result = await _authService.SendOtpAsync(dto);

            if (result == "OTP sent to console")
                return Ok(new ApiResponse(200, true, result, null));

            return BadRequest(new ApiResponse(400, false, result, null));
        }

        [HttpPost("signup/phone")]
        public async Task<IActionResult> SignupPhone([FromBody] PhoneSignupDto dto)
        {
            var result = await _authService.SignupPhoneAsync(dto);

            if (result == null)
                return BadRequest(new ApiResponse(400, false, "Invalid or expired OTP", null));

            if(result == "Phone number already registered")
                return BadRequest(new ApiResponse(400, false, "Phone number already registered", null));


            return Ok(new ApiResponse(201, true, "User registered successfully", result));
  
        }

        [HttpPost("login/email")]
        public async Task<IActionResult> LoginEmail([FromBody] EmailLoginDto dto)
        {
            var response = await _authService.LoginEmailAsync(dto);
            if(response == null) return BadRequest(new ApiResponse(400, false, "User does not exist!", null));

            return Ok(new ApiResponse(201, true, "User login successfully", response));
        }
        [HttpPost("login/send-otp")]
        public async Task<IActionResult> LoginSendOtp([FromBody] PhoneOtpRequestDto dto)
        {
            var result = await _authService.LoginSendOtpAsync(dto);

            if (result == null)
                return Ok(new ApiResponse(200, true, "OTP sent successfully", result));

            return BadRequest(new ApiResponse(400, false, "Failed", result));
        }
        [HttpPost("login/phone")]
        public async Task<IActionResult> LoginPhone([FromBody] PhoneLoginDto dto)
        {
            var result = await _authService.LoginPhoneAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid phone number or OTP" });

            return Ok(result);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var result = await _authService.LogoutAsync(dto);

            if (result == null)
                return NotFound("No active session found for this device.");

            return Ok(new { message = result });
        }

    }
}

