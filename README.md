# Godot Resource Generation Framework

This is an editor plugin for Godot 4.4 in C# that provides a framework 
for generating and managing resources in your Godot projects. 
It allows you to create custom resource generators that can produce 
resources programatically based on specific parameters, 
making it easier to manage complex resource creation workflows.

## Features
- **Resource Generators**: Create custom resource 
generators that can produce resources based on defined parameters.
- **Editor Integration**: Seamlessly integrates with the Godot editor, 
allowing you to manage and generate resources directly from the editor.
- **Parameter Management**: Define and manage parameters for resource generation.
- **Extensible**: Easily extend the framework to add new types of resource generators.

## Quick Start
1. Install the plugin into your Godot project.
2. Create a new Resource Generator script by extending the `AbstractResourceProvider` class.
3. Define the parameters and the generation logic in your script.
4. Use the editor interface to manage and generate resources.
5. Done! Your resources are now generated and managed through the framework.

## Example Usage
Here's a simple example of how to create a custom resource generator:
```csharp
using Godot;
using ResourceGenerationFramework.ResourceGen;

// Define a resource generator for any resource type. 
// We'll use a custom resource type here.

[Tool] // Yes, your custom resource needs to be a tool script.
[GlobalClass]
public partial class AchievementResource : Resource 
{
    [Export]
    public string AchievementName { get; set; } = "New Achievement";

    [Export]
    public string Description { get; set; } = "Achievement Description";

    [Export]
    public int Points { get; set; } = 10;
    
    public AchievementResource(string name, string description, int points) 
    {
        AchievementName = name;
        Description = description;
        Points = points;
    }
}

// Then, create a resource provider for it:

[Tool]
public class AchievementProvider : AbstractResourceProvider<AchievementResource>
{
    protected override string GetName()
    {
        return "Achievement Provider";
    }

    protected override void Generate()
    {
        AddResource(new AchievementResource
        {
            Title = "First Steps",
            Description = "Complete the tutorial.",
            Icon = GetIcon()
        });
    }
}
```

Then, you can use the editor interface to manage and generate your resources.
That's right, it's that simple! The plugin takes care of the rest thanks
to the power of Reflection.

## Configuration
You can configure the plugin settings by editing the `config.cfg` file located in the plugin directory.
This file allows you to customize the plugin's behavior,
such as setting default paths for generated resources and
enabling or disabling saving log files.

## Contributing
Contributions are welcome! If you find a bug or have a feature request, please open an issue here on GitHub.
Feel free to fork the repository and submit a pull request with your changes.