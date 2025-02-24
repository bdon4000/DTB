namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private Dictionary<string, bool> selectedStates = new()
        {
            { "OK", true }  // Default to showing OK data
        };


        private void ToggleOkData()
        {
            selectedStates["OK"] = !selectedStates["OK"];
            selectedProduction["OK"] = !selectedProduction["OK"];
            UpdateNgChartOption();
            UpdateProductionChartOption();
            StateHasChanged();
        }

        private void HandleCpkFieldChange(string value)
        {
            _selectedCpkField = value;
            UpdateCpkChartOption();
            StateHasChanged();
        }

        private void BreakpointOnOnUpdate(object? sender, BreakpointChangedEventArgs e)
        {
            OnPropertyChanged();
        }
        private void OnPropertyChanged()
        {
            if (NavHelper.CurrentUri.EndsWith("dashboard/equipstate"))
            {
                InvokeAsync(StateHasChanged);
            }
        }
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged();
        }
    }
}
