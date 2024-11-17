using back_end.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using back_end.Models.Client;
using back_end.Interfaces.UserInterfaces;
using back_end.DTOs.UserDTOs;

namespace back_end.Services.UserServices
{
    public class AccountServices(DatabaseContext context, IPasswordHasher<User> passwordHasher) : IAccountService
    {
        private readonly DatabaseContext _context = context;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        //Service lien quan den tai khoan
        //register service
        public async Task RegisterAsync(User account, string password)
        {
            account.PasswordHash = _passwordHasher.HashPassword(account, password);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Username == username);
        }
        //login service
        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
        }

        public bool CheckPassword(User account, string password)
        {
            if (string.IsNullOrEmpty(account.PasswordHash))
            {
                throw new ArgumentNullException(nameof(account.PasswordHash), "Password hash cannot be null or empty.");
            }
            var result = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }
        //change password service
        public async Task ChangePasswordAsync(User account, string newPassword)
        {
            account.PasswordHash = _passwordHasher.HashPassword(account, newPassword);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public Task<User?> FindAccountByIdAsync(string id)
        {
            var account = _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            return account;
        }

        //Service lien quan den thong tin nguoi dung  
        public async Task<UserInfor?> FindInforByIdAsync(string id)
        {
            var userInfor = await _context.UserInfors.FirstOrDefaultAsync(x => x.Id == id);
            return userInfor;
        }

        public async Task<bool> ChangeUserInforAsync(ChangeInformationDTO userInforDTO, string userId)
        {
            var existingUserInfor = await _context.UserInfors.FirstOrDefaultAsync(x => x.Id == userId);
            if (existingUserInfor == null)
            {
                throw new ArgumentNullException(nameof(existingUserInfor), "User information not found.");
            }

            bool isModified = false;

            var dtoProperties = typeof(ChangeInformationDTO).GetProperties();
            var userInforProperties = typeof(UserInfor).GetProperties().ToDictionary(p => p.Name);

            foreach (var dtoProperty in dtoProperties)
            {
                var newValue = dtoProperty.GetValue(userInforDTO);

                if (newValue != null && userInforProperties.TryGetValue(dtoProperty.Name, out var userInforProperty))
                {
                    var existingValue = userInforProperty.GetValue(existingUserInfor);

                    if (!Equals(newValue, existingValue))
                    {
                        userInforProperty.SetValue(existingUserInfor, newValue);
                        isModified = true;
                    }
                }
            }

            if (isModified)
            {
                _context.UserInfors.Update(existingUserInfor);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        //Service lien quan den admin
        //Lay tat ca cac user trong he thong
        public Task<List<User>> GetAllUser()
        {
            var accounts = _context.Accounts.ToListAsync();
            return accounts;
        }
    }
}