using BackendRaith.Helper;
using CustomDataBase;
using Microsoft.EntityFrameworkCore;

namespace BackendRaith.Services
{
    public class ShiftService
    {
        private readonly CustomDataBaseContext _db;

        public ShiftService(CustomDataBaseContext db)
        {
            _db = db;
        }

        public async Task AddShiftAsync(Shift shift)
        {
            _db.Shifts.Add(shift);
            await _db.SaveChangesAsync();
        }

        public List<ShiftDTO> GetAllShifts()
        {
            List<ShiftDTO> shifts = new List<ShiftDTO>();
            shifts = _db.Shifts.Select(x => new ShiftDTO().CopyPropertiesFrom(x)).ToList();
            return shifts;
        }

        internal void LoadShiftsIntoDB()
        {
            WebScrapingService webScrapingService = new WebScrapingService();
            var shifts = webScrapingService.ScrapeData();

            _db.Shifts.RemoveRange(_db.Shifts.ToList());

            foreach (var shift in shifts)
            {
                _db.Shifts.Add(shift);

            }
            _db.SaveChanges();
        }
    }
}
