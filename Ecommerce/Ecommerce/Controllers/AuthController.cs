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

            if (user == JsonHelper.GetMessage(103))
            {
                return Ok(new ApiResponse(200,true,user,null));
            }

            return BadRequest(new ApiResponse(400, false, user, null));
        }
        [HttpPost("signup/send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] PhoneOtpRequestDto dto)
        {
            var result = await _authService.SendOtpAsync(dto);

            if (result == JsonHelper.GetMessage(105))
                return Ok(new ApiResponse(200, true, result, null));

            return BadRequest(new ApiResponse(400, false, result, null));
        }

        [HttpPost("signup/phone")]
        public async Task<IActionResult> SignupPhone([FromBody] PhoneSignupDto dto)
        {
            var result = await _authService.SignupPhoneAsync(dto);

            if (result == null)
                return BadRequest(new ApiResponse(400, false, JsonHelper.GetMessage(106), null));

            if(result == JsonHelper.GetMessage(107))
                return BadRequest(new ApiResponse(400, false, JsonHelper.GetMessage(107), null));


            return Ok(new ApiResponse(201, true, JsonHelper.GetMessage(108), result));
  
        }

        [HttpPost("login/email")]
        public async Task<IActionResult> LoginEmail([FromBody] EmailLoginDto dto)
        {
            var response = await _authService.LoginEmailAsync(dto);
            if(response == null) return BadRequest(new ApiResponse(400, false, JsonHelper.GetMessage(105), null));

            return Ok(new ApiResponse(201, true, JsonHelper.GetMessage(110) , response));
        }
        [HttpPost("login/send-otp")]
        public async Task<IActionResult> LoginSendOtp([FromBody] PhoneOtpRequestDto dto)
        {
            var result = await _authService.LoginSendOtpAsync(dto);

            if (result == null)
                return BadRequest(new ApiResponse(400, false, "Failed", result));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(105), result));
           
        }
        [HttpPost("login/phone")]
        public async Task<IActionResult> LoginPhone([FromBody] PhoneLoginDto dto)
        {
            var result = await _authService.LoginPhoneAsync(dto);

            if (result == null)
                return BadRequest(new ApiResponse(401, false, JsonHelper.GetMessage(111), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(110), result));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var result = await _authService.LogoutAsync(dto);

            if (result == null)
                return NotFound(new ApiResponse(404, false, JsonHelper.GetMessage(112), null));

            return Ok(new ApiResponse(200, true, result, null));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);
            if (!result)
                return NotFound(new ApiResponse(404, false, JsonHelper.GetMessage(114), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(115), null));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (result == null)
                return BadRequest(new ApiResponse(400, false, JsonHelper.GetMessage(116), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(117), result));
        }


    }
}

