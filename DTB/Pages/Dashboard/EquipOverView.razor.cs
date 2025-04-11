using Microsoft.AspNetCore.SignalR.Client;
using DTB.Data.App.Status;
using DTB.Data.Devices;
using Microsoft.EntityFrameworkCore;
using DTB.Data.BatteryData.BaseModel;
using DTB.Data.BatteryData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System;

namespace DTB.Pages.Dashboard
{
    public partial class EquipOverView : ProComponentBase
    {
        private HubConnection? _hubConnection;
        private bool isLoading = true;
        private bool isInitialized;
        private List<DeviceModel> allDevices = new();
        private List<DeviceStatusClass> deviceStatuses = new();
        private Shift? currentShift;
        private int totalJellyFeedingCount = 0;
        private int totalPreChargeCount = 0;
        private float totalYieldRate = 0;
        private int totalNgCount = 0;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                
                // Get all devices
                allDevices = CacheFactory.GetAllDevices().ToList();
                
                // Initialize shift data
                await InitializeShiftDataAsync();
                
                // Initialize SignalR
                await InitializeSignalR();
                
                // Get initial device statuses
                await RefreshDeviceStatuses();
                
                // Register for UI updates
                MasaBlazor.BreakpointChanged += BreakpointOnOnUpdate;
                MasaBlazor.Application.PropertyChanged += OnPropertyChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnInitializedAsync: {ex}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        /*private async Task InitializeShiftDataAsync()
        {
            try
            {
                await ShiftService.RefreshCache();
                isInitialized = true;
                
                var shifts = await ShiftService.GetCachedShiftsAsync();
                if (!shifts.Any())
                {
                    return;
                }

                var currentTime = DateTime.Now.TimeOfDay;
                currentShift = shifts.FirstOrDefault(shift =>
                {
                    if (!TimeSpan.TryParse(shift.StartTime, out var startTime) ||
                        !TimeSpan.TryParse(shift.EndTime, out var endTime))
                    {
                        return false;
                    }
                    if (endTime < startTime)
                    {
                        return currentTime >= startTime || currentTime < endTime;
                    }
                    return currentTime >= startTime && currentTime < endTime;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing shift data: {ex.Message}");
            }
        }*/

        /*private async Task InitializeSignalR()
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
        }*/

        /*private async Task RefreshDeviceStatuses()
        {
            deviceStatuses.Clear();
            
            foreach (var device in allDevices)
            {
                var cache = CacheFactory.GetCache(device.Id);
                var status = cache.GetStatus();
                if (status != null)
                {
                    deviceStatuses.Add(status);
                }
            }
            
            await CalculateProductionStatistics();
        }*/

        /*private async Task CalculateProductionStatistics()
        {
            try
            {
                using var context = await DbContextFactory.CreateDbContextAsync();
                
                // Get current shift time range
                DateTime shiftStartTime = DateTime.Now.Date;
                DateTime shiftEndTime = DateTime.Now;
                
                if (currentShift != null && 
                    TimeSpan.TryParse(currentShift.StartTime, out var startTimeSpan) && 
                    TimeSpan.TryParse(currentShift.EndTime, out var endTimeSpan))
                {
                    var now = DateTime.Now;
                    shiftStartTime = now.Date.Add(startTimeSpan);
                    shiftEndTime = now.Date.Add(endTimeSpan);
                    
                    // Handle overnight shifts
                    if (endTimeSpan < startTimeSpan && now.TimeOfDay < endTimeSpan)
                    {
                        shiftStartTime = shiftStartTime.AddDays(-1);
                    }
                    else if (endTimeSpan < startTimeSpan && now.TimeOfDay >= startTimeSpan)
                    {
                        shiftEndTime = shiftEndTime.AddDays(1);
                    }
                }
                
                // Get JellyFeeding count
                totalJellyFeedingCount = await context.JellyFeedingDatas
                    .Where(j => j.uploadTime >= shiftStartTime && j.uploadTime <= shiftEndTime)
                    .CountAsync();
                
                // Get PreCharge count
                totalPreChargeCount = await context.PreChargeDatas
                    .Where(p => p.uploadTime >= shiftStartTime && p.uploadTime <= shiftEndTime)
                    .CountAsync();
                
                // Calculate total NG count from all devices
                totalNgCount = deviceStatuses
                    .SelectMany(s => s.deviceChartDatas ?? new List<DeviceChartData>())
                    .Sum(d => d.NgOutput);
                
                // Calculate yield rate
                if (totalJellyFeedingCount > 0)
                {
                    totalYieldRate = 100 * (1 - (float)totalNgCount / totalJellyFeedingCount);
                }
                else
                {
                    totalYieldRate = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating production statistics: {ex.Message}");
            }
        }

        private float CalculateDeviceYieldRate(DeviceStatusClass deviceStatus)
        {
            if (deviceStatus?.deviceChartDatas == null || !deviceStatus.deviceChartDatas.Any())
            {
                return 0;
            }
            
            var totalOutput = deviceStatus.deviceChartDatas.Sum(x => x.OkOutput + x.NgOutput);
            var ngOutput = deviceStatus.deviceChartDatas.Sum(x => x.NgOutput);
            
            if (totalOutput > 0)
            {
                return 100 * (1 - (float)ngOutput / totalOutput);
            }
            
            return 0;
        }

        private void BreakpointOnOnUpdate(object? sender, BreakpointChangedEventArgs e)
        {
            OnPropertyChanged();
        }
        
        private void OnPropertyChanged()
        {
            if (NavHelper.CurrentUri.EndsWith("dashboard/equipoverview"))
            {
                InvokeAsync(StateHasChanged);
            }
        }*/

        public void Dispose()
        {
            try
            {
                MasaBlazor.BreakpointChanged -= BreakpointOnOnUpdate;
                MasaBlazor.Application.PropertyChanged -= OnPropertyChanged;
                
                if (_hubConnection is not null)
                {
                    foreach (var device in allDevices)
                    {
                        _hubConnection.SendAsync("LeaveDeviceGroup", device.DeviceCode).Wait();
                    }
                    
                    _hubConnection.DisposeAsync().AsTask().Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Dispose: {ex}");
            }
        }
    }
}
