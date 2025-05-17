using ChatService.Repository.Models;
using ChatService.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Account> AccountRepository {  get; }
        IGenericRepository<AccountRole> AccountRoleRepository {  get; }
        IGenericRepository<Role> RoleRepository {  get; }
        Task SaveChangesAsync();
    }
}
