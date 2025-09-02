using System.Collections.Generic;
using Godot;

namespace ResourceGenerationFramework.ResourceGen;

public static class ResGenLogger
{
    private static List<string> Logs { get; } = [];
    
    public static void Log(string message)
    {
        Logs.Add(message);
        GD.Print(message);
    }

    public static void SaveConfigFile()
    {
        const string logPath = "res://";
        const string fileName = "ResourceGenLog.log";
        
        var dir = DirAccess.Open(logPath);
        
        if (dir.FileExists(fileName))
        {
            dir.Remove(fileName);
        }
        
        using var file = FileAccess.Open($"{logPath}/{fileName}", FileAccess.ModeFlags.Write);
        file.StoreLine("ResourceGen Log File");
        file.StoreLine("====================");
        foreach (var log in Logs)
        {
            file.StoreLine(log);
        }
        file.StoreString("Brought to you by ResourceGen.");
        file.Close();
        GD.Print($"Log file saved to {logPath}{fileName}");
        Clear();
    }

    public static void Clear()
    {
        Logs.Clear();
    }
}