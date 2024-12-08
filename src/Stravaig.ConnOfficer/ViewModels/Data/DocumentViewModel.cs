using ReactiveUI;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public abstract class DocumentViewModel : ViewModelBase, IDisposable
{
    private readonly SideBarViewModel _sideBar;
    private SideBarNodeViewModel? _sideBarNode;

    protected DocumentViewModel(SideBarViewModel sideBar)
    {
        _sideBar = sideBar;
        sideBar.SelectedSideBarNodeChanged += SideBarOnSelectedSideBarNodeChanged;
    }

    public SideBarNodeViewModel? SideBarNode
    {
        get => _sideBarNode;
        set
        {
            this.RaiseAndSetIfChanged(ref _sideBarNode, value);
        }
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
