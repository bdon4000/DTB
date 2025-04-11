using Microsoft.AspNetCore.SignalR.Client;
using DTB.Data.App.Status;
using DTB.Data.Devices;
using System.Threading.Tasks;
using System;

namespace DTB.Pages.Dashboard
{
    public partial class EquipOverView
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

                _hubConnection.On<DeviceStatusClass>("ReceiveDeviceStatus", async (status) =>
                {
                    if (status != null && status.DeviceInfo != null)
                    {
                        var existingStatus = deviceStatuses.FirstOrDefault(s => s.DeviceInfo?.Id == status.DeviceInfo.Id);
                        if (existingStatus != null)
                        {
                            var index = deviceStatuses.IndexOf(existingStatus);
                            deviceStatuses[index] = status;
                        }
                        else
                        {
                            deviceStatuses.Add(status);
                        }

                        // Update production counts and yield rate
                        await CalculateProductionStatistics();
                        
                        await InvokeAsync(StateHasChanged);
                    }
                });

                try
                {
                    await _hubConnection.StartAsync();
                    
                    // Join device groups for all devices
                    foreach (var device in allDevices)
                    {
                        await _hubConnection.SendAsync("JoinDeviceGroup", device.DeviceCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to SignalR hub: {ex}");
                }
            }
        }
    }
}
