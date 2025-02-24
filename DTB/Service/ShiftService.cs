using DTB.Data.DataBase;
using DTB.Data.Devices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace DTB.Service
{
    public class ShiftService : IShiftService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY = "SHIFTS_CACHE";
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _isInitialized;

        public ShiftService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            
        }

        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized) return;

            if (await _semaphore.WaitAsync(TimeSpan.FromSeconds(10)))
            {
                try
                {
                    if (!_isInitialized)
                    {
                        var shifts = await GetAllShiftsAsync();
                        var cacheOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromHours(1));
                        _cache.Set(CACHE_KEY, shifts, cacheOptions);
                        _isInitialized = true;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public async Task<List<Shift>> GetAllShiftsAsync()
        {
            return await _context.Shifts.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<Shift> GetShiftByIdAsync(int id)
        {
            return await _context.Shifts.FindAsync(id)
                ?? throw new KeyNotFoundException($"Shift with ID {id} not found.");
        }

        public async Task<Shift> CreateShiftAsync(Shift shift)
        {
            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();
            await RefreshCache();
            return shift;
        }

        public async Task<Shift> UpdateShiftAsync(Shift shift)
        {
            var existingShift = await GetShiftByIdAsync(shift.Id);
            _context.Entry(existingShift).CurrentValues.SetValues(shift);
            await _context.SaveChangesAsync();
            await RefreshCache();
            return shift;
        }

        public async Task DeleteShiftAsync(int id)
        {
            var shift = await GetShiftByIdAsync(id);
            shift.IsActive = false;
            await _context.SaveChangesAsync();
            await RefreshCache();
        }

        public async Task RefreshCache()
        {
            var shifts = await GetAllShiftsAsync();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            _cache.Set(CACHE_KEY, shifts, cacheOptions);
        }
        public async Task<List<Shift>> GetCachedShiftsAsync()
        {
            await EnsureInitializedAsync();
            return _cache.Get<List<Shift>>(CACHE_KEY) ?? new List<Shift>();
        }
        public List<Shift> GetCachedShifts()
        {
            return GetCachedShiftsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
