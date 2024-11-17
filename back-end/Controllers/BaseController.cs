using back_end.DTOs.UserDTOs;
using back_end.Interfaces.UserInterfaces;
using back_end.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    public abstract class BaseController(IAccountService accountService, JwtTokenGenerator jwtTokenGenerator) : ControllerBase
    {
        protected readonly IAccountService _accountService = accountService;
        protected readonly JwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

        // Validate input for SQL injection
        protected BadRequestObjectResult? ValidateInput<T>(T dto)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var value = property.GetValue(dto) as string;
                if (!string.IsNullOrEmpty(value) && InputValidator.ContainsSqlInjection(value))
                {
                    return BadRequest("Invalid input detected.");
                }
            }
            return null;
        }

        // Get user ID from the JWT token
        protected string? GetUserIdFromToken()
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var claimsPrincipal = _jwtTokenGenerator.ValidateJwtToken(token);
            return claimsPrincipal?.FindFirst("Id")?.Value;
        }

        // Common Login method
        protected async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var validationResult = ValidateInput(loginDTO);
            if (validationResult != null)
            {
                return validationResult;
            }

            var account = await _accountService.FindByUsernameAsync(loginDTO.Username);
            if (account == null || !_accountService.CheckPassword(account, loginDTO.Password))
            {
                return BadRequest("Invalid username or password.");
            }

            var token = _jwtTokenGenerator.GenerateJwtToken(account);
            return Ok(new { token });
        }

        // Common Change Password method
        protected async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var validationResult = ValidateInput(changePasswordDTO);
            if (validationResult != null)
            {
                return validationResult;
            }

            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized();
            }

            var account = await _accountService.FindAccountByIdAsync(userId);
            if (account == null || !_accountService.CheckPassword(account, changePasswordDTO.OldPassword))
            {
                return BadRequest("Invalid old password.");
            }

            await _accountService.ChangePasswordAsync(account, changePasswordDTO.NewPassword);
            return Ok(new { message = "Password changed successfully." });
        }

        // Change Information Method
        [Authorize]
        [HttpPut("change-infor")]
        public async Task<IActionResult> ChangeInformation([FromBody] ChangeInformationDTO changeInformationDTO)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("User not logged in.");
            }
            var changeResult = await _accountService.ChangeUserInforAsync(changeInformationDTO, userId);
            return Ok(new { message = "Change information successfully." });
        }
    }
}
