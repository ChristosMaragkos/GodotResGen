#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace ResourceGenerationFramework.ResourceGen;

[Tool]
public static class ProviderManager
{
    private static readonly List<string> ProviderTypeNames = [];
    private static VBoxContainer _container;

    public static void Initialize(VBoxContainer container)
    {
        _container = container;
    }

    public static void RefreshProviders()
    {
        if (_container == null)
        {
            GD.PrintErr("Provider manager not initialized correctly. Please restart the plugin.");
            return;
        }
        
        ResGenLogger.Clear();
        ClearProviderButtons();
        ProviderTypeNames.Clear();

        var start = DateTime.Now;
        ResGenLogger.Log($"Time: {start}");
        ResGenLogger.Log("Initializing provider discovery...");

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException reflection) { return reflection.Types.Where(t => t != null)!; }
            })
            .Where(t => t is { IsAbstract: false } &&
                        typeof(AbstractResourceProvider).IsAssignableFrom(t));

        foreach (var t in types)
        {
            var aqn = t.AssemblyQualifiedName;
            if (aqn == null) continue;
            ProviderTypeNames.Add(aqn);
            AddProviderButton(t.Name, aqn);
            ResGenLogger.Log($"Discovered provider: {t.FullName}");
        }

        ResGenLogger.Log($"Discovered {ProviderTypeNames.Count} providers.");
        FinishTask(start);
    }

    public static void GenerateResources()
    {
        ResGenLogger.Clear();
        var start = DateTime.Now;

        RefreshProviders();

        ResGenLogger.Log($"Time: {start}");
        ResGenLogger.Log("Starting resource generation...");

        if (ProviderTypeNames.Count == 0)
        {
            ResGenLogger.Log("No providers found.");
            FinishTask(start);
            return;
        }

        var totalNew = 0;
        var totalChanged = 0;

        foreach (var aqn in ProviderTypeNames.ToArray())
        {
            var (newRes, changedRes) = RunSingleProvider(aqn);
            totalNew += newRes;
            totalChanged += changedRes;
        }

        var totalAffected = totalNew + totalChanged;
        ResGenLogger.Log("All providers completed.");
        ResGenLogger.Log("--------------------------------");
        ResGenLogger.Log("GENERATION SUMMARY");
        ResGenLogger.Log($"[NEW] {totalNew}");
        ResGenLogger.Log($"[CHANGED] {totalChanged}");
        ResGenLogger.Log($"[TOTAL] {totalAffected}");
        FinishTask(start);
        MaybeWriteLog();
        CleanupAfterRun();
    }

    private static (int newRes, int changedRes) RunSingleProvider(string assemblyQualifiedName)
    {
        var type = Type.GetType(assemblyQualifiedName, false);
        if (type == null)
        {
            ResGenLogger.Log($"Type not found: {assemblyQualifiedName}");
            return (0, 0);
        }

        var newRes = 0;
        var changedRes = 0;
        AbstractResourceProvider instance = null;
        try
        {
            instance = (AbstractResourceProvider)Activator.CreateInstance(type);
            instance?.GenerateAndSave(out newRes, out changedRes);
            ResGenLogger.Log($"Provider {type.Name} completed. New={newRes}, Changed={changedRes}, Affected={newRes + changedRes}");
        }
        catch (Exception ex)
        {
            ResGenLogger.Log($"Error in {type.FullName}: {ex.Message}");
        }
        finally
        {
            if (instance is IDisposable d) d.Dispose();
        }
        return (newRes, changedRes);
    }

    private static void AddProviderButton(string displayName, string aqn)
    {
        if (_container == null)
        {
            GD.PrintErr("Provider manager not initialized correctly. Please restart the plugin.");
            return;
        }
        
        var button = new Button
        {
            Text = displayName,
            TooltipText = "Click to run this provider"
        };
        button.SetMeta("ProviderAqn", aqn);
        button.Pressed += () =>
        {
            var meta = button.GetMeta("ProviderAqn");
            var name = (string)meta;
            ResGenLogger.Clear();
            var start = DateTime.Now;
            ResGenLogger.Log($"Time: {start}");
            ResGenLogger.Log($"Starting resource generation for provider: {displayName}");
            var (n, c) = RunSingleProvider(name);
            ResGenLogger.Log($"Provider {displayName} run complete. New={n}, Changed={c}, Affected={n + c}");
            FinishTask(start);
            MaybeWriteLog();
        };
        _container.AddChild(button);
    }

    private static void ClearProviderButtons()
    {
        if (_container == null) return;
        
        foreach (var child in _container.GetChildren().OfType<Button>().ToArray())
        {
            _container.RemoveChild(child);
            child.QueueFree();
        }
    }

    private static void CleanupAfterRun()
    {
        ProviderTypeNames.Clear();
    }

    private static void MaybeWriteLog()
    {
        if (ConfigManager.ShouldCreateLogFile())
            ResGenLogger.SaveConfigFile();
    }

    private static void FinishTask(DateTime start)
    {
        var end = DateTime.Now;
        ResGenLogger.Log($"Task completed in {(end - start).TotalSeconds} seconds.");
    }
}
#endif