# Unity Custom Utilities Package

A comprehensive collection of utilities, frameworks, and editor tools designed to streamline Unity development workflows. This package provides solutions for resource management, audio handling, data persistence, and various editor productivity enhancements.

## Features

### Core Frameworks

#### Resource Management
- **ResourceLoader<T>**: Streamlined asset loading with automatic caching
- **ResourceAttribute**: Define resource paths using attributes
- **DontDestroyOnLoad**: Components and automatic persistence management

#### Audio System
- **AudioHandler**: Centralized audio management singleton
- **AudioDatabase**: Organized sound and music containers with custom editors
- **Audio Pooling**: Optimized audio source usage with pooling system
- **Customizable Audio**: Support for random pitch, volume, and cooldowns

#### Storage System
- **Multiple Providers**: Binary file, Firebase, and PlayerPrefs implementations
- **IStorageProvider**: Interface for custom storage solutions
- **PersistentReactiveProperty**: Reactive properties with automatic persistence
- **Data Transformers**: Transformers for serialization and encryption

#### Object Pooling
- **PoolHandler<T>**: Generic object pooling system
- **Configurable Callbacks**: Hooks for create, get, release, and destroy operations

### Singleton Implementations
- **Singleton<T>**: Basic singleton for non-MonoBehaviour classes
- **SingletonBehaviour<T>**: MonoBehaviour-based singleton
- **SingletonScriptableObject<T>**: ScriptableObject-based singleton
- **PersistentSingletonBehavior<T>**: DontDestroyOnLoad singleton for MonoBehaviours

### Editor Tools

#### Enhanced Editor Framework
- **EditorBase** & **WindowBase**: Base classes for consistent editor UI
- **ThemedEditorSettings**: Centralized UI styling for consistent look and feel
- **EditorVisualControls**: Streamlined creation of editor UI elements
- **EditorStateControls**: Automatic Undo support for editor operations

#### Custom Menu Framework
- **CustomMenuSettings**: Configuration-based menu item generation
- **Scene, Asset, and Method Menus**: Multiple menu types
- **ScriptingSymbolHandler**: Toggling scripting symbols via menu items

#### Utility Windows
- **DirtyMaker**: Force Unity to save specific assets
- **FlatScriptsCreator**: Create flat script structures from nested directories
- **SpriteAlphaAdder**: Convert RGB sprites to RGBA format
- **SpriteResizer**: Resize sprites to meet power-of-two requirements
- **AssemblyReferenceAnalyzer**: Find unused assembly references

### Custom Attributes & Drawers
- **DistinctEnumAttribute**: Ensure enum fields contain unique values
- **InspectorReadOnlyAttribute**: Make fields read-only in the inspector
- **RequiredFieldAttribute**: Mark fields as required with validation

### Extension Methods
- **CanvasExtensions**: Show/hide CanvasGroup components
- **ConvertExtension**: Convert between async types
- **StringExtensions**: String manipulation utilities
- **JsonExtension**: JSON serialization helpers
- **ReflectionExtensions**: Type reflection utilities

## Installation

1. Clone or download this repository
2. Copy the package into your Unity project's Assets folder
3. The utilities will be available immediately through their respective namespaces
4. Editor tools can be accessed via the "Tools" menu in the Unity Editor

## Usage Examples

### Resource Loading
```csharp
// Define a resource using the ResourceAttribute
[Resource(name: "MyConfig", resourcePath: "Configs")]
public class MyConfig : ScriptableObject 
{
    // Your configuration properties here
}

// Load the resource
var config = ResourceLoader<MyConfig>.Load();
```

### Audio System
```csharp
// Play a sound
AudioHandler.Instance.PlaySound(SoundType.ButtonClick);

// Play music
AudioHandler.Instance.PlayMusic(MusicType.MainMenu);
```

### Persistent Storage
```csharp
// Create a storage provider
var storageProvider = new BinaryFileProvider();

// Create a persistent property
var playerScore = new PersistentReactiveProperty<int>("player_score", storageProvider, 0);

// Subscribe to changes
playerScore.Subscribe(newScore => Debug.Log($"Score changed to {newScore}"));

// Use the property (automatically saved on change)
playerScore.Value = 100;
```

### Editor Tool Usage

Access the editor tools through the Tools menu in the Unity Editor:

- **DirtyMaker**: Tools > Utils > Dirty Maker
- **FlatScriptCreator**: Tools > Utils > Flat Script
- **SpriteAlphaAdder**: Tools > Utils > Sprite Alpha Adder
- **SpriteResizer**: Tools > Utils > Fix NPOT
- **AssemblyReferenceAnalyzer**: Tools > Utils > Analyze Unused Assembly References

## Notes
- Always backup your project before using batch processing tools
- Some operations may take time depending on the number of files being processed
- Extension methods and attributes can be used immediately without additional setup