using Blazor.Server.UI.Components.Shared;
using Blazor.Server.UI.Services;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;
using Microsoft.AspNetCore.Components.Authorization;
using Toolbelt.Blazor.HotKeys2;

namespace Blazor.Server.UI.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    private bool _commandPaletteOpen;
    private HotKeysContext? _hotKeysContext;
    private bool _sideMenuDrawerOpen = true;
    private UserPreferences UserPreferences = new();
    [Inject]
    private LayoutService _layoutService { get; set; } = null!;
    private MudThemeProvider _mudThemeProvider=null!;
    private bool _themingDrawerOpen;
    private bool _defaultDarkMode;
    [Inject] private IDialogService _dialogService { get; set; } = default!;
    [Inject] private HotKeys _hotKeys { get; set; } = default!;
    
    
    
   
    public void Dispose()
    {
        _layoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        _hotKeysContext?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await ApplyUserPreferences();
            await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
            StateHasChanged();
        }
    }
    private async Task ApplyUserPreferences()
    {
        _defaultDarkMode =await _mudThemeProvider.GetSystemPreference();
        UserPreferences = await _layoutService.ApplyUserPreferences(_defaultDarkMode);
    }
    protected override void OnInitialized()
    {
        _layoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
        _layoutService.SetBaseTheme(Theme.ApplicationTheme());
        _hotKeysContext = _hotKeys.CreateContext().Add(ModKey.Ctrl, Key.K, async () => await OpenCommandPalette(), "Open command palette.");
       

    }
    private async Task OnSystemPreferenceChanged(bool newValue)
    {
        await _layoutService.OnSystemPreferenceChanged(newValue);
    }
    private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();



    protected void SideMenuDrawerOpenChangedHandler(bool state)
    {
        _sideMenuDrawerOpen = state;
    }
    protected void ThemingDrawerOpenChangedHandler(bool state)
    {
        _themingDrawerOpen = state;
    }
    protected void ToggleSideMenuDrawer()
    {
        _sideMenuDrawerOpen = !_sideMenuDrawerOpen;
    }
    private async Task OpenCommandPalette()
    {
        if (!_commandPaletteOpen)
        {
            var options = new DialogOptions
            {
                NoHeader = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            var commandPalette = _dialogService.Show<CommandPalette>("", options);
            _commandPaletteOpen = true;

            await commandPalette.Result;
            _commandPaletteOpen = false;
        }
    }


}