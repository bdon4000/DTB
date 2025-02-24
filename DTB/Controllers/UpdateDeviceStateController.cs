using DTB.Data.Devices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;



namespace DTB.Controllers
{
    [Route("api/[controller]/{deviceCode}")]
    [ApiController]

    public class UpdateDeviceStateController : ControllerBase
    {
        private readonly IDeviceStateService _stateService;

        public UpdateDeviceStateController(IDeviceStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string deviceCode, [FromBody] DeviceStateModel model)
        {
            try
            {
                var status = await _stateService.UpdateDeviceState(deviceCode, model);
                return Ok(ApiResponse<DeviceStatusClass>.Success(null, "State updated successfully"));
            }
            catch (DeviceNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, ex.Message));
            }
        }
    }

    public class DeviceStateModel
    {
        public string? DeviceName { get; set; }
        public string? DeviceState { get; set; }
        public string? Operator { get; set; }
        public DateTime? DateTime { get; set; }
        public string[]? ErrorMessage { get; set; }
    }
}
