using System.Threading.Tasks;

namespace Pouya.Fund.Abstractions
{
    public interface IUserValidator
    {
        /// <summary>True when the user exists in the current database and is active.</summary>
        Task<bool> IsValidAsync(int userId);
    }
}
