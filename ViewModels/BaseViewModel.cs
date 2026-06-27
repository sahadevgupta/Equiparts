using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    string title = string.Empty;
}