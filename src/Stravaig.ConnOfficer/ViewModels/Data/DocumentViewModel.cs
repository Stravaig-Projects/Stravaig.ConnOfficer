using ReactiveUI;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System;
using System.Reactive.Linq;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public abstract class DocumentViewModel : ViewModelBase, IDisposable
{
    private readonly SideBarViewModel _sideBar;
    private SideBarNodeViewModel? _sideBarNode;
    private string _title;

    protected DocumentViewModel(SideBarViewModel sideBar)
    {
        _title = GenerateTitle(null);
        _sideBar = sideBar;
        sideBar.SelectedSideBarNodeChanged += SideBarOnSelectedSideBarNodeChanged;
        this.WhenAnyValue(vm => vm.SideBarNode)
            .Select(node => GenerateTitle(node?.Name))
            .ToProperty(this, nameof(Title));
    }

    private string GenerateTitle(string? name)
    {
        return $"{GetType().Name}: {name ?? "No Selection"}";
    }

    public SideBarNodeViewModel? SideBarNode
    {
        get => _sideBarNode;
        set
        {
            this.RaiseAndSetIfChanged(ref _sideBarNode, value);
        }
    }

    public string Title
    {
        get => _title;
        private set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void SideBarOnSelectedSideBarNodeChanged(SideBarNodeViewModel? selectedNode)
    {
        SideBarNode = selectedNode;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sideBar.SelectedSideBarNodeChanged -= SideBarOnSelectedSideBarNodeChanged;
        }
    }
}

public class WelcomeDocumentViewModel : DocumentViewModel
{
    public WelcomeDocumentViewModel(SideBarViewModel sideBar)
        : base(sideBar)
    {
    }
}
