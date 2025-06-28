namespace Cardify.Core.Services
{
    public interface IDashboardService
    {
        Task<object> GetDashboardDataAsync(int userId);
    }
} 