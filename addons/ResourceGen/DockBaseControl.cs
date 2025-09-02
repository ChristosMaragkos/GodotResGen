#if TOOLS
using Godot;

namespace ResourceGenerationFramework.ResourceGen;

[Tool]
public partial class DockBaseControl : Control
{
    [Export] public Button GenerateButton;
    [Export] public MenuButton ConfigMenuButton;
    [Export] public Button RefreshProvidersButton;
    [Export] public VBoxContainer ProviderContainer;

    public override void _EnterTree()
    {
        base._EnterTree();
        
        ConfigMenuButton.GetPopup().IdPressed += ConfigManager.RegenerateConfigFile;
        RefreshProvidersButton.Pressed += ProviderManager.RefreshProviders;
        GenerateButton.Pressed += ProviderManager.GenerateResources;
    }
    
    public override void _ExitTree()
    {
        ConfigMenuButton.GetPopup().IdPressed -= ConfigManager.RegenerateConfigFile;
        RefreshProvidersButton.Pressed -= ProviderManager.RefreshProviders;
        GenerateButton.Pressed -= ProviderManager.GenerateResources;
        
        base._ExitTree();
    }
}
#endif