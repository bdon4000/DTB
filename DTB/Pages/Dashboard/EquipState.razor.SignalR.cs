using Microsoft.AspNetCore.SignalR.Client;
using DTB.Data.App.Status;
using DTB.Data.Devices;

using Microsoft.EntityFrameworkCore;

namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private async Task InitializeSignalR()
        {
            if (_hubConnection == null)
            {
                var urls = Configuration["Urls"] ?? "http://localhost:5000";
                var baseUrl = urls.Split(';')[0]
                                 .TrimEnd('/')
                                 .Replace("*", "localhost");

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(baseUrl + "/devicehub")
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.On<int[][]>("ReceiveChartData", async (chartData) =>
                {

                    await InvokeAsync(StateHasChanged);
                });

                _hubConnection.On<DeviceStatusClass>("ReceiveDeviceStatus", async (status) =>
                {
                    if (status != null)
                    {
                        deviceStatus = status;
                        _deviceState = (DeviceState)status.Status;
                        statusInfo = _deviceState.GetStatusInfo();
                        if (deviceStatus?.Operator != null)
                        {
                            currentOperator = await UserManager.FindByNameAsync(deviceStatus.Operator);
                        }

                        UpdateNgChartOption();
                        UpdateElectricMeterChartOption();
                        UpdateProductionChartOption();
                        UpdateDeviceStateChartOption();
                        UpdateCpkChartOption();
                        await InvokeAsync(StateHasChanged);
                    }
                });

                try
                {
                    await _hubConnection.StartAsync();
                    await _hubConnection.SendAsync("JoinDeviceGroup", DeviceCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to SignalR hub: {ex}");
                }
            }
        }
        private async Task<DeviceModel> GetDeviceByCode(string deviceCode)
        {
            try
            {
                await using var context = await DbContextFactory.CreateDbContextAsync();
                return await context.Devices
                    .FirstOrDefaultAsync(d => d.DeviceCode == deviceCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting device by code: {ex}");
                return null;
            }
        }
    }
}
