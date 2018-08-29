using Data.Entities;

namespace BLL
{
    public interface IApplicationUserService
    {
        UserEntity Get(string login);

        int AddUserIfNecessary(string login);
        void SaveTogglTokenForCurrentUser(string togglToken);
        string GetTogglTokenForCurrentUser();
        void SaveTempoTokenForCurrentUser(string tempoToken);
        string GetTempoTokenForCurrentUser();
    }
}