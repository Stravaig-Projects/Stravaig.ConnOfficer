using CommunityToolkit.Mvvm.Input;

namespace Stravaig.ConnOfficer.Commands;

public interface IConvertToAsyncRelayCommand
{
    AsyncRelayCommand AsyncRelayCommand { get; }
}