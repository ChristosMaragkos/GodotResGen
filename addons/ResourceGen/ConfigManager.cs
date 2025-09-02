#if TOOLS
using Godot;

namespace ResourceGenerationFramework.ResourceGen;

[Tool]
public static class ConfigManager
{
    private static ConfigFile _configFile = new();

    public static string GetOutputPath()
    {
        var config = GetOrCreateConfig();
        return config.GetValue("settings", "output_path", "res://Resources/Generated").ToString();
    }

    public static bool ShouldCreateLogFile()
    {
        var config = GetOrCreateConfig();
        return (bool)config.GetValue("settings", "log_generation", true);
    }

    private static ConfigFile GetOrCreateConfig()
    {
        var err = _configFile.Load("res://addons/ResourceGen/config.cfg");
        if (err == Error.Ok) return _configFile;
        GD.PrintErr($"Failed to load config file: {err}.");
        RegenerateConfigFile();
        return _configFile;
    }

    private static void RegenerateConfigFile()
    {
        GD.Print("Regenerating config file...");
        _configFile = new ConfigFile();
        _configFile.SetValue("settings", "output_path", "res://Resources/Generated");
        _configFile.SetValue("settings", "log_generation", false);
        _configFile.Save("res://addons/ResourceGen/config.cfg");
    }

    public static void RegenerateConfigFile(long id)
    {
        if (id != 0) return;
        RegenerateConfigFile();
    }
}
#endif