using DTB.Data.Devices;

namespace DTB.Service
{
    public interface IShiftService
    {
        Task<List<Shift>> GetAllShiftsAsync();
        Task<Shift> GetShiftByIdAsync(int id);
        Task<Shift> CreateShiftAsync(Shift shift);
        Task<Shift> UpdateShiftAsync(Shift shift);
        Task<List<Shift>> GetCachedShiftsAsync();
        Task DeleteShiftAsync(int id);
        Task RefreshCache();

        List<Shift> GetCachedShifts();
    }
}
