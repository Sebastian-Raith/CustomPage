using BackendRaith.Services;
using CustomDataBase;
using Microsoft.AspNetCore.Mvc;

namespace BackendRaith.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly ShiftService _shiftService;

        public ShiftController(ShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveShift(Shift shift)
        {
            await _shiftService.AddShiftAsync(shift);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<List<Shift>> GetAllShiftsFromDB()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            return shifts.ToList();
        }
        [HttpPost("[action]")]
        public string LoadShiftsIntoDB()
        {
            try
            {
                _shiftService.LoadShiftsIntoDB();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return Ok().ToString();
        }
    }
}