

using BackendRaith.Helper;

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
        public async Task<IActionResult> SaveShift(ShiftDTO shiftdto)
        {
            Shift shift = new();
            shift = shift.CopyPropertiesFrom(shiftdto);
            await _shiftService.AddShiftAsync(shift);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<List<ShiftDTO>> GetAllShiftsFromDB()
        {
            var shifts = _shiftService.GetAllShifts();
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