using back_end.Interfaces.UserInterfaces;
using back_end.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Authorize]
    [Route("apiv1/admin/[controller]")]
    public class AdminController(IAccountService accountService, JwtTokenGenerator jwtTokenGenerator) : BaseController(accountService, jwtTokenGenerator)
    {
        public async Task<IActionResult> GetAllUser(){
            var users = await _accountService.GetAllUser();
            return Ok(users);
        }
    }
}