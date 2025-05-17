## [1.0.0] – 2025-01-22
### First Release
- Added the first release of the package.
## [1.0.1] – 2025-01-26
### Fix
- Changed to only editor execution in assembly.
## [1.1.0] – 2025-04-02
### Fix
- DirtyMaker.
## [2.0.0] – 2025-05-11
### Feature
- Complete architecture overhaul with proper namespace organization
- Resource management system with ResourceLoader<T> and caching
- Asset loading framework with configuration system
- Audio framework with AudioHandler, AudioDatabase and pooling
- Storage system with multiple providers (Binary, Firebase, PlayerPrefs)
- Enhanced editor tools with consistent UI theming
- Object pooling system with PoolHandler<T>
- Custom menu framework for editor menu items
- Singleton implementations (multiple variants)
- Extension methods for Unity components
- Custom attribute system with drawers
- Assembly reference analyzer
- Enhanced all original tools with improved UI and functionality
## [2.0.1] – 2025-05-11
### Fix
- Added dependencies to other packages which are used.
## [2.0.2] – 2025-05-11
### Feature
- Localization.
- AnyType.
- MethodSerialization.
### Refactor
- Suppress some IDE warning.
- Moved from linq to zlinq
## [2.1.0] – 2025-05-17
### Feature
- CancellationSourceHelper.
- EditorProgressTracker to show the progress of the asynchronous operations.
- LockInspector toggles both the Unity Inspector's lock state and the "Constrain Proportions" setting on Transform components through shortcuts.
- Added LoadAsync and xml comments for ResourceLoader
### Refactor
- Replaced ReSharper disable to UsedImplicitly attribute