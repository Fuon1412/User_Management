using Microsoft.AspNetCore.Mvc;
using back_end.Interfaces.UserInterfaces;
using back_end.DTOs.UserDTOs;
using back_end.Utils;
using Microsoft.AspNetCore.Authorization;
using back_end.Models.Client;

namespace back_end.Controllers
{
    [ApiController]
    [Route("apiv1/user/[controller]")]
    public class UserController : BaseController
    {
        public UserController(IAccountService accountService, JwtTokenGenerator jwtTokenGenerator)
            : base(accountService, jwtTokenGenerator)
        {
        }

        // Register Method
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var validationResult = ValidateInput(registerDTO);
            if (validationResult != null)
            {
                return validationResult;
            }

            var existingAccount = await _accountService.FindByEmailAsync(registerDTO.Email);
            if (existingAccount != null)
            {
                return BadRequest("Email is already registered.");
            }
            else if (await _accountService.FindByUsernameAsync(registerDTO.Username) != null)
            {
                return BadRequest("Username is already taken.");
            }
            else if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                return BadRequest("Password and Confirm Password do not match.");
            }

            string userId = Guid.NewGuid().ToString();

            var newAccount = new User
            {
                Id = userId,
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                Password = registerDTO.Password,
                Status = "active"
            };

            var userInfor = new UserInfor
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = userId,
                Account = newAccount,
                FullName = registerDTO.FullName,
                Bussiness = registerDTO.Bussiness,
            };
            newAccount.UserInfor = userInfor;
            await _accountService.RegisterAsync(newAccount, registerDTO.Password);
            var token = _jwtTokenGenerator.GenerateJwtToken(newAccount);

            return Ok(new { token });
        }

        // Get User Information Method
        [Authorize]
        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("User not logged in.");
            }

            var userInfor = await _accountService.FindInforByIdAsync(userId);
            if (userInfor == null)
            {
                return BadRequest("User information not found.");
            }
            return Ok(new { userInfor });
        }
     
    }
}
