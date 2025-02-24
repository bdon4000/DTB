using DTB.Data;
using DTB.Data.Devices;
using DTB.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using DTB.Data.Devices;

namespace DTB.Controllers.HeartbeatController
{
    [Route("api/[controller]/{deviceCode}")]
    [ApiController]
    public class HeartbeatController : ControllerBase
    {
        private readonly IDeviceStateService _stateService;

        public HeartbeatController(IDeviceStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string deviceCode, [FromBody] HeartBeatModel model)
        {
            try
            {
                var status = await _stateService.UpdateHeartbeat(deviceCode, model);
                return Ok(ApiResponse<DeviceStatusClass>.Success(null, "Heartbeat updated successfully"));
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
   
    public class HeartBeatModel
    {
        public string? DeviceName { get; set; }
        public string? Operator { get; set; }
        public DateTime? DateTime { get; set; }
        public float? ElectricMeter { get; set; }
    }
}




    

