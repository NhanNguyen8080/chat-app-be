using ChatService.Repository.Data;
using ChatService.Repository.Models;
using ChatService.Repository.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private ChatDbContext _context;

        public UnitOfWork(ChatDbContext context)
        {
            _context = context;
        }

        private IGenericRepository<Account> _accountRepository;
        private IGenericRepository<AccountRole> _accountRoleRepository;
        private IGenericRepository<Role> _roleRepository;

        public IGenericRepository<Account> AccountRepository
        {
            get
            {
                if (_accountRepository == null)
                {
                    _accountRepository = new GenericRepository<Account>(_context);
                }
                return _accountRepository;
            }
        }

        public IGenericRepository<AccountRole> AccountRoleRepository
        {
            get
            {
                if (_accountRoleRepository == null)
                {
                    _accountRoleRepository = new GenericRepository<AccountRole>(_context);
                }
                return _accountRoleRepository;
            }
        }

        public IGenericRepository<Role> RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                {
                    _roleRepository = new GenericRepository<Role>(_context);
                }
                return _roleRepository;
            }
        }


        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveChangesAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
