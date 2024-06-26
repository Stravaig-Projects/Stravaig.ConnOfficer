﻿using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SideBarViewModel SideBar { get; init; }

    public BreadcrumbsViewModel Breadcrumbs { get; init; }

    public MainWindowViewModel(SideBarViewModel sideBar)
    {
        SideBar = sideBar;
        Breadcrumbs = new BreadcrumbsViewModel(sideBar);
    }
}
