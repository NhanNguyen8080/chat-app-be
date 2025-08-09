using ChatService.API.Attributes;
using ChatService.Repository.UnitOfWork;
using ChatService.Service.DTOs;
using ChatService.Service.MappingExtensions;
using ChatService.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ChatService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IRedisService _redisService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IAccountService accountService, 
                                 IJwtTokenFactory jwtTokenFactory, 
                                 IRedisService redisService, 
                                 IUnitOfWork unitOfWork)
        {
            _accountService = accountService;
            _jwtTokenFactory = jwtTokenFactory;
            _redisService = redisService;
            _unitOfWork = unitOfWork;
        }

        //Doing send OTP through Twillio
        [Route("send-otp/{phoneNumber}")]
        [HttpPost]
        public async Task<IActionResult> SendOtp(string phoneNumber)
        {
            try
            {
                var response = await _accountService.SendOtp(phoneNumber);
                if (!response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message); // Bad Request or Not Found
                }
                return Ok(new { Message = response.Message, Data = response.Data });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [Route("verify-otp")]
        [HttpPost]
        public async Task<IActionResult> VerifyOtp([FromQuery] string phoneNumber, [FromQuery] int otp)
        {
            try
            {
                var response = await _accountService.VerifyOTP(phoneNumber, otp);
                if (!response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message); // Bad Request or Not Found
                }
                return Ok(new { Message = response.Message, Data = response.Data });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [Route("check-account-exists/{phoneNumber}")]
        [HttpPost]
        public async Task<IActionResult> CheckAccountExists(string phoneNumber)
        {
            try
            {
                var response = await _accountService.CheckAccountExists(phoneNumber);
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }
                return Ok(new { Message = response.Message, Data = response.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [Route("reset-password/{phoneNumber}")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string phoneNumber, ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var response = await _accountService.ResetPassword(phoneNumber, resetPasswordDTO.Password);
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }
                return Ok(new { Message = response.Message, Data = response.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [Route("sign-up")]
        [HttpPost]
        public async Task<IActionResult> SignUp(AccountCM accountCM)
        {
            try
            {
                var response = await _accountService.SignUp(accountCM);
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }
                return Ok(new { Message = response.Message, Data = response.Data } );
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [Route("sign-in")]
        [HttpPost]
        public async Task<IActionResult> SignIn(LoginDTO loginDTO)
        {
            try
            {
                var response = await _accountService.SignIn(loginDTO);
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }

                var token = await _jwtTokenFactory.CreateTokenAsync(response.Data);
                return Ok(new { Message = response.Message, Data = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        [RoleAuthorize("User")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                if (_jwtTokenFactory.ValidateRefreshToken(refreshToken, out var phoneNumber))
                {
                    var accountVM = await _accountService.GetAccountByPhoneNumber(phoneNumber);
                    if (accountVM.Data == null)
                    {
                        return Unauthorized("Invalid refresh token!");
                    }

                    var account = AccountMappingExtension.MapToAccount(accountVM.Data);
                    var token = await _jwtTokenFactory.CreateTokenAsync(account);
                    return Ok(token);
                }
                return Unauthorized("Invalid refresh token!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("log-out")]
        [RoleAuthorize("User")]
        public IActionResult Logout([FromBody] string refreshToken)
        {
            try
            {
                _jwtTokenFactory.RevokeRefreshToken(refreshToken);
                return Ok("Logged out successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("get-account/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetAccountById(Guid id)
        {

            try
            {
                var response = await _accountService.GetAccountById(id);
                return Ok(new { Message = response.Message, Data = response.Data });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [Route("update-profile/{id}")]
        [HttpPut]
        [RoleAuthorize("User")]
        public async Task<IActionResult> UpdateProfile(Guid id, AccountUM accountUM)
        {
            try
            {
                var response = await _accountService.UpdateProfile(id, accountUM);
                if (!response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message); // Bad Request or Not Found
                }
                return Ok(new { Message = response.Message, Data = response.Data });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }


        [Route("get-account-by-phonenumber/{phoneNumber}")]
        [HttpGet]
        public async Task<IActionResult> GetAccountByPhoneNumber(string phoneNumber)
        {
            try
            {
                var response = await _accountService.GetAccountByPhoneNumber(phoneNumber);
                return Ok(new { Message = response.Message, Data = response.Data });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        } // This controller needs to be optimized (Optimize or elasticsearch)

        [Route("change-name/{accountId}")]
        [HttpPut]
        [RoleAuthorize("User")]
        public async Task<IActionResult> ChangeName(Guid accountId, string newName)
        {
            try
            {
                var response = await _accountService.ChangeNameById(accountId, newName);
                if (!response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message);
                }
                return Ok(new { Message = response.Message, Data = response.Data });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [RoleAuthorize("User")]
        [HttpPut]
        [Route("update-avatar/{accountId}")]
        public async Task<IActionResult> UpdateAvatar(Guid accountId, IFormFile avatarFile)
        {
            try
            {
                var response = await _accountService.UpdateAvatar(accountId, avatarFile);
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }

                return Ok(new { Message = response.Message, Data = response.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [RoleAuthorize("User")]
        [HttpPut]
        [Route("update-cover-photo/{accountId}")]
        public async Task<IActionResult> UpdateCoverPhoto(int accountId, IFormFile coverPhotoFile)
        {
            try
            {
                var response = await _accountService.UpdateCoverPhoto(accountId, coverPhotoFile);
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }

                await _unitOfWork.SaveChangesAsync();
                return Ok(new { Message = response.Message, Data = response.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
