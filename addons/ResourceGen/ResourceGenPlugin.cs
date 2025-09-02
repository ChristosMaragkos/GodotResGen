#if TOOLS
using Godot;

namespace ResourceGenerationFramework.ResourceGen;

[Tool]
public partial class ResourceGenPlugin : EditorPlugin
{
    private DockBaseControl _dock;

    public override void _EnterTree()
    {
        AddCustomType("DockBaseControl",
            "Control",
            GD.Load<Script>("res://addons/ResourceGen/DockBaseControl.cs"),
            null);

        _dock = (DockBaseControl)GD.Load<PackedScene>("res://addons/ResourceGen/control.tscn").Instantiate();
        
        ProviderManager.Initialize(_dock.ProviderContainer);
        
        AddControlToDock(DockSlot.LeftBr, _dock);
    }

    public override void _ExitTree()
    {
        RemoveCustomType("DockBaseControl");

        RemoveControlFromDocks(_dock);

        base._ExitTree();
    }
}

#endif