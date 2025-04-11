using DTB.Data.Devices;
using System;

namespace DTB.Pages.Dashboard
{
    public partial class EquipOverView
    {
        private void BreakpointOnOnUpdate(object? sender, BreakpointChangedEventArgs e)
        {
            OnPropertyChanged();
        }
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged();
        }
        private void OnPropertyChanged()
        {
            if (NavHelper.CurrentUri.EndsWith("dashboard/equipoverview"))
            {
                InvokeAsync(StateHasChanged);
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
    }
}
