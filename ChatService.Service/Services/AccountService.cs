using Azure;
using ChatService.Repository.Models;
using ChatService.Repository.UnitOfWork;
using ChatService.Service.DTOs;
using ChatService.Service.MappingExtensions;
using ChatService.Service.Ultities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static System.Net.WebRequestMethods;

namespace ChatService.Service.Services
{
    public interface IAccountService
    {
        Task<ResponseDTO<AccountVM>> ChangeNameById(Guid id, string newName);
        Task<ResponseDTO<AccountVM>> CheckAccountExists(string phoneNumber);
        Task<ResponseDTO<AccountVM>> GetAccountById(Guid id);
        Task<ResponseDTO<AccountVM>> GetAccountByPhoneNumber(string phoneNumber);
        Task<IList<string>> GetUserRoles(Guid id);
        Task<ResponseDTO<AccountVM>> ResetPassword(string phoneNumber, string newPassword);
        Task<ResponseDTO<bool>> SendOtp(string phoneNumber);
        Task<ResponseDTO<Account>> SignIn(LoginDTO loginDTO);
        Task<ResponseDTO<AccountVM>> SignUp(AccountCM accountCM);
        Task<ResponseDTO<AccountVM>> UpdateAvatar(Guid accountId, IFormFile avatarFile);
        Task<ResponseDTO<AccountVM>> UpdateCoverPhoto(int accountId, IFormFile coverPhotoFile);
        Task<ResponseDTO<AccountVM>> UpdateProfile(Guid id, AccountUM accountUM);
        Task<ResponseDTO<bool>> VerifyOTP(string phoneNumber, int otp);
    }
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRedisService _redisService;
        private readonly string _phoneNumberOTPsKey;

        public AccountService(IUnitOfWork unitOfWork, 
                              ICloudinaryService cloudinaryService,
                              IRedisService redisService,
                              IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _redisService = redisService;
            _phoneNumberOTPsKey = configuration.GetValue<string>("RedisKeys:PhoneNumberOtps");
        }

        public async Task<ResponseDTO<AccountVM>> ChangeNameById(Guid id, string newName)
        {
            try
            {
                var changedNameAccount = await _unitOfWork.AccountRepository.GetByID(id);
                if (changedNameAccount is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Not found account",
                        Data = null,
                    };
                }

                changedNameAccount.FullName = newName;
                await _unitOfWork.AccountRepository.UpdateAsync(changedNameAccount);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Changed name successfully!",
                    Data = AccountMappingExtension.MapToAccountVM(changedNameAccount),
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseDTO<AccountVM>> CheckAccountExists(string phoneNumber)
        {
            try
            {
                var existedAccount = (await _unitOfWork.AccountRepository.Get(_ => _.PhoneNumber.Equals(phoneNumber)))
                                                                         .FirstOrDefault();
                if (existedAccount is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Account not found",
                        Data = null,
                    };
                }
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Account found",
                    Data = existedAccount.MapToAccountVM()
                };
            } catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseDTO<AccountVM>> GetAccountById(Guid id)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByID(id);
                if (account is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Message = "Account not found",
                        Data = null
                    };
                }
                var accountVM = account.MapToAccountVM();
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Account found",
                    Data = accountVM
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<AccountVM>> GetAccountByPhoneNumber(string phoneNumber)
        {
            try
            {
                var account = (await _unitOfWork.AccountRepository.Get(_ => _.PhoneNumber.Equals(phoneNumber))).FirstOrDefault();
                if (account is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Message = "Account not found",
                        Data = null
                    };
                }
                var accountVM = account.MapToAccountVM();
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Account found",
                    Data = accountVM
                };
            } catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<IList<string>> GetUserRoles(Guid id)
        {
            try
            {
                var roles = (await _unitOfWork.AccountRoleRepository.Get(_ => _.AccountId.Equals(id), null, "Role"))
                                                                    .Select(ac => ac.Role.RoleName)
                                                                    .ToList();
                return roles;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ResponseDTO<AccountVM>> ResetPassword(string phoneNumber, string newPassword)
        {
            try
            {
                var account = (await _unitOfWork.AccountRepository.Get(_ => _.PhoneNumber.Equals(phoneNumber))).FirstOrDefault();
                if (account is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Account not found",
                        Data = null
                    };
                }
                account.PasswordHash = PasswordHasher.HashPassword(newPassword);
                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Reset password successfully",
                    Data = AccountMappingExtension.MapToAccountVM(account),
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };  
            }
        }

        public async Task<ResponseDTO<bool>> SendOtp(string phoneNumber)
        {
            try
            {
                phoneNumber = ConvertToInternationalFormat(phoneNumber);
                var otp = GenerateOTP();

                DotNetEnv.Env.Load();

                var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNTSID");
                var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTHTOKEN");
                TwilioClient.Init(accountSid, authToken);

                var to = phoneNumber;
                var from = "+14433280816";

                var message = MessageResource.Create(
                    to: to,
                    from: from,
                    body: $"Mã xác thực của bạn là: {otp}");

                if (message.ErrorCode != null)
                {
                    return new ResponseDTO<bool>
                    {
                        IsSuccess = false,
                        StatusCode = 500,
                        Message = message.ErrorMessage,
                        Data = false
                    };
                }

                var listPhoneNumberOTPsInCache = _redisService.GetData<List<PhoneNumberOtpDTO>>(_phoneNumberOTPsKey)
                                                        ?? new List<PhoneNumberOtpDTO>();
                var existedPhoneNumberOTP = listPhoneNumberOTPsInCache.Find(_ => _.PhoneNumber == phoneNumber);
                if (existedPhoneNumberOTP is not null)
                {
                    return new ResponseDTO<bool>
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Message = "OTP already sent",
                        Data = false
                    };
                }
                else
                {
                    listPhoneNumberOTPsInCache.Add(new PhoneNumberOtpDTO()
                    {
                        OTP = otp,
                        PhoneNumber = phoneNumber
                    });
                    _redisService.SetData(_phoneNumberOTPsKey, listPhoneNumberOTPsInCache, TimeSpan.FromSeconds(62));
                    return new ResponseDTO<bool>
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "OTP sent successfully",
                        Data = true
                    };
                }
            } catch (Exception ex)
            {
                return new ResponseDTO<bool>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = false
                };
            }
        }
        

        public async Task<ResponseDTO<Account>> SignIn(LoginDTO loginDTO)
        {
            try
            {
                var signedInAccount = (await _unitOfWork.AccountRepository.Get(_ => _.PhoneNumber.Equals(loginDTO.PhoneNumber)))
                                                                          .FirstOrDefault();
                if (signedInAccount is null)
                {
                    return new ResponseDTO<Account>
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Message = "Not found user!",
                        Data = null,
                    };
                }

                if (!PasswordHasher.VerifyPassword(loginDTO.Password, signedInAccount.PasswordHash)) {
                    return new ResponseDTO<Account>
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Message = "Password is invalid!",
                        Data = null,
                    };
                }

                return new ResponseDTO<Account>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Signed in successfully!",
                    Data = signedInAccount,
                };
            } catch (Exception ex)
            {
                return new ResponseDTO<Account>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseDTO<AccountVM>> SignUp(AccountCM accountCM)
        {
            var response = new ResponseDTO<AccountVM>();
            try
            {
                var existingAccount = (await _unitOfWork.AccountRepository.Get(_ => _.PhoneNumber.Equals(accountCM.PhoneNumber)))
                                                                          .FirstOrDefault();
                if (existingAccount is not null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Account already exists",
                        Data = null
                    };
                }
                var account = accountCM.MapToAccount();
                account.PasswordHash = PasswordHasher.HashPassword(accountCM.Password);
                account.CoverPhoto = DefaultValue.DefaultCoverPhoto;
                account.Bio = DefaultValue.DefaultBio;
                if (account.Gender.Equals("Male"))
                {
                    account.Avatar = DefaultValue.DefaultMaleAvatar;
                } else
                {
                    account.Avatar = DefaultValue.DefaultFemaleAvatar;
                }
                // Insert the account into the database
                await _unitOfWork.AccountRepository.InsertAsync(account);

                // Assign default role to the account
                var role = (await _unitOfWork.RoleRepository.Get(_ => _.RoleName.Equals("User"))).FirstOrDefault();
                if (role is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Role not found",
                        Data = null
                    };
                }
                var accountRole = new AccountRole
                {
                    AccountId = account.Id,
                    Account = account,
                    RoleId = role.Id,
                    Role = role
                };

                // Insert the account into the database
                await _unitOfWork.AccountRoleRepository.InsertAsync(accountRole);

                // Save changes to the database
                await _unitOfWork.SaveChangesAsync();

                // Map the account to AccountVM and return the result
                var accountVM = account.MapToAccountVM();
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Account created successfully",
                    Data = accountVM
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<AccountVM>> UpdateAvatar(Guid accountId, IFormFile avatarFile)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByID(accountId);

                if (account is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Account not found",
                        Data = null
                    };
                }

                var uploadResult = await _cloudinaryService.UploadImageAsync(avatarFile);
                if (uploadResult is null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "Upload image to cloudinary failed!",
                        Data = null
                    };
                }

                account.Avatar = uploadResult.SecureUrl.AbsoluteUri;
                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();

                var accountVM = AccountMappingExtension.MapToAccountVM(account);
                return new ResponseDTO<AccountVM>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Update avatar successfully!",
                    Data = accountVM
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseDTO<AccountVM>> UpdateCoverPhoto(int accountId, IFormFile coverPhotoFile)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByID(accountId);

                if (account is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Account not found",
                        Data = null
                    };
                }

                var uploadResult = await _cloudinaryService.UploadImageAsync(coverPhotoFile);
                if (uploadResult is null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "Upload image to cloudinary failed!",
                        Data = null
                    };
                }

                account.CoverPhoto = uploadResult.SecureUrl.AbsoluteUri;
                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();

                var accountVM = AccountMappingExtension.MapToAccountVM(account);
                return new ResponseDTO<AccountVM>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Update avatar successfully!",
                    Data = accountVM
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseDTO<AccountVM>> UpdateProfile(Guid id, AccountUM accountUM)
        {
            try
            {
                var updatedAccount = await _unitOfWork.AccountRepository.GetByID(id);
                if (updatedAccount is null)
                {
                    return new ResponseDTO<AccountVM>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Account not found",
                        Data = null
                    };
                }
                updatedAccount = accountUM.MapToAccount();
                await _unitOfWork.AccountRepository.UpdateAsync(updatedAccount);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Account updated successfully",
                    Data = updatedAccount.MapToAccountVM()
                };
            } catch (Exception ex)
            {
                return new ResponseDTO<AccountVM>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<bool>> VerifyOTP(string phoneNumber, int otp)
        {
            try
            {
                var listPhoneNumberOTPsInCache = _redisService.GetData<List<PhoneNumberOtpDTO>>(_phoneNumberOTPsKey)
                                                       ?? new List<PhoneNumberOtpDTO>();
                var existedPhoneNumberOTP = listPhoneNumberOTPsInCache.Find(_ => _.PhoneNumber == phoneNumber);
                if (existedPhoneNumberOTP is null)
                {
                    return new ResponseDTO<bool>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "OTP not found",
                        Data = false
                    };
                }

                if (existedPhoneNumberOTP.OTP != otp)
                {
                    return new ResponseDTO<bool>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "OTP is invalid",
                        Data = false
                    };
                } else
                {
                    listPhoneNumberOTPsInCache.Remove(existedPhoneNumberOTP);
                    return new ResponseDTO<bool>
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "OTP is valid",
                        Data = true,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = false
                };  
            }
        }

        private int GenerateOTP()
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 1000000); // Generates a number between 100000 and 999999
            return otpNumber;
        }

        private string ConvertToInternationalFormat(string phoneNumber)
        {
            if (phoneNumber.StartsWith("0"))
            {
                return "+84" + phoneNumber.Substring(1);
            }
            return phoneNumber; // Return the original if it doesn't start with 0
        }
    }
}
