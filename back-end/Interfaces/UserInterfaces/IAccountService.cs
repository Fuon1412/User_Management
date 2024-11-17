using back_end.DTOs.UserDTOs;
using back_end.Models.Client;
namespace back_end.Interfaces.UserInterfaces
{
    public interface IAccountService
    {
        //Interface lien quan den cac tac vu cua admin
        Task<List<User>> GetAllUser();
        //Interface lien quan den cac service voi tai khoan
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByUsernameAsync(string username);
        Task<User?> FindAccountByIdAsync(string id);
        Task RegisterAsync(User account, string password);
        Task ChangePasswordAsync(User account, string newPassword);
        bool CheckPassword(User account, string password);

        //Interface lien quan den cac service voi thong tin nguoi dung
        Task<UserInfor?> FindInforByIdAsync(string id);
        Task<bool> ChangeUserInforAsync(ChangeInformationDTO userInfor, string id);
    }

}