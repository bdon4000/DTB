using Microsoft.AspNetCore.SignalR.Client;
using DTB.Data.App.Status;
using DTB.Data.Devices;

using Microsoft.EntityFrameworkCore;
using DTB.Data.BatteryData.BaseModel;

using DTB.Services;



namespace DTB.Pages.Dashboard
{
    public partial class EquipState : ProComponentBase
    {
        [Parameter]
        public string DeviceCode { get; set; }
        private DeviceStatusClass deviceStatus;
        private HubConnection? _hubConnection;
        private DeviceState _deviceState;
        private DeviceStatus statusInfo;
        private bool isLoading = true;
        private AppUser currentOperator;       
        private bool ShowDetailDialog { get; set; }
        private List<DataTableHeader<FullBaseModel>> _batteryDataHeaders = new();       
        private DateTime ShiftStartTime { get; set; }
        private DateTime ShiftEndTime { get; set; }
        private TimeSpan TimeInterval { get; set; } = TimeSpan.FromHours(1);
        private bool isInitialized;


        protected override async Task OnInitializedAsync()
        {
            try
            {

                isInitialized = true;
                await base.OnInitializedAsync();
                await InitializeShiftDataAsync();
                var (start, end) = await GetCurrentShiftTimeAsync();

                ShiftStartTime = RoundToHalfHour(start, false);
                ShiftEndTime = RoundToHalfHour(end, true);

                UpdateProductionChartOption();
                UpdateElectricMeterChartOption();
                UpdateDeviceStateChartOption();

                _chartEvents["legendselectchanged"] = new Action<object>(args =>
                {
                    try
                    {
                        // 获取从图表返回的选择状态
                        var jsonElement = (System.Text.Json.JsonElement)args;
                        var selected = jsonElement.GetProperty("selected");

                        // 更新 ngChartSelectedStates
                        foreach (var item in selected.EnumerateObject())
                        {
                            if (ngChartSelectedStates.ContainsKey(item.Name))
                            {
                                ngChartSelectedStates[item.Name] = item.Value.GetBoolean();
                            }
                        }

                        // 强制更新 UI
                        InvokeAsync(StateHasChanged);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error handling legend select: {ex.Message}");
                    }
                });

                statusInfo = DeviceState.Running.GetStatusInfo();

                var device = await GetDeviceByCode(DeviceCode);
                if (device != null)
                {
                    var cache = CacheFactory.GetCache(device.Id);
                    deviceStatus = cache.GetStatus();
                    if (deviceStatus != null)
                    {
                        _deviceState = (DeviceState)deviceStatus.Status;
                        statusInfo = _deviceState.GetStatusInfo();
                        UpdateNgChartOption();
                    }
                    InitializeBatteryDataHeaders();
                }
                if (deviceStatus?.DeviceInfo?.DeviceData != null)
                {
                    var firstCpkField = deviceStatus.DeviceInfo.DeviceData.FirstOrDefault(f => f.isCPK);
                    if (firstCpkField != null)
                    {
                        _selectedCpkField = firstCpkField.fieldName;
                        UpdateCpkChartOption();
                    }
                }

                MasaBlazor.BreakpointChanged += BreakpointOnOnUpdate;
                MasaBlazor.Application.PropertyChanged += OnPropertyChanged;

                // _hubConnection.start should after chart init.
                await InitializeSignalR();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnInitializedAsync: {ex}");
                // 可以添加错误处理逻辑
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            try
            {
                MasaBlazor.BreakpointChanged -= BreakpointOnOnUpdate;
                MasaBlazor.Application.PropertyChanged -= OnPropertyChanged;
                if (_hubConnection is not null)
                {
                    _hubConnection.SendAsync("LeaveDeviceGroup", DeviceCode).Wait();
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