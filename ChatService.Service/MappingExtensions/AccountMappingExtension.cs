using ChatService.Repository.Models;
using ChatService.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Service.MappingExtensions
{
    public static class AccountMappingExtension
    {
        public static AccountVM MapToAccountVM(this Account entity)
        {
            return new AccountVM
            {
                AccountId = entity.Id,
                Avatar = entity.Avatar,
                CoverPhoto = entity.CoverPhoto,
                Bio = entity.Bio,
                PhoneNumber = entity.PhoneNumber,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
            };
        }

        public static Account MapToAccount(this AccountCM entity)
        {
            return new Account
            {
                PhoneNumber = entity.PhoneNumber,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
            };
        }

        public static Account MapToAccount(this AccountVM entity)
        {
            return new Account
            {
                Id = entity.AccountId,
                Avatar = entity.Avatar,
                CoverPhoto = entity.CoverPhoto,
                Bio = entity.Bio,
                PhoneNumber = entity.PhoneNumber,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
            };
        }

        public static Account MapToAccount(this AccountUM entity)
        {
            return new Account
            {
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
            };
        }
    }
}
