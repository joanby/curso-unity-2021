# Changelog
All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [1.6.1] - 2021-03-30
### Fixed
- Fixed bug caused by Editor API transitioning from private to public

## [1.6.0] - 2021-03-23
### Changed
- Updated graph migration process

## [1.5.2] - 2021-03-05
### Changed
- User interface updated
- Names in different UI elements made to be more consistent with new naming schemes

## [1.5.1] - 2021-02-23
### Added
- Warn the user when an Input System Package event is referencing an action of the wrong type for that event
- A warning is raised when adding more than one Input unit in a SuperUnit
- "Open" inspector button and double clicking a graph in the project browser now opens the visual scripting editor
- A warning is raised when the step's default value of the For unit is set to 0.

### Fixed
- Fixed "Restore to Defaults" buttons in the Project Settings window
- Fixed ThreadAbortException when entering Play Mode while searching in the Fuzzy Finder
- Fixed Visual Scripting Preferences being searchable [BOLT-1218](https://issuetracker.unity3d.com/issues/visual-scripting-preferences-are-not-searchable-when-using-search-in-the-preferences-window)
- Fixed ScalarAdd unit migration from 1.4.13 to 1.4.14 and above
- Fixed Open the graph window no longer causes Unity UI stop processing mouse click [BOLT-1159](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1159),
- Fixed Fuzzy finder no longer blinks when trying to add a node [BOLT-1157](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1157),
- Fixed Fuzzy search no longer drops keyboard inputs and respond slowly [BOLT-1214](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1214),
- Fixed Fuzzy finder search window no longer remains above all other windows [BOLT-1197](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1197)"
- Fixed Dropdown icon is not clipped with TextField under "Get Variable"
- Fixed Scale groups when zoom is not at 1x
- Fixed graph getting corrupted when adding "Get Action Map" unit
- Fixed node description being sometimes clipped
- Fixed warnings overflow in the console when deleting and adding a boolean variable in the blackboard
- Fixed warnings when entering play mode when the "Script Changes While Playing" is set to Recompile And Continue Playing
- Fixed resize cursor rect on group when graph window is zoomed
- Fixed VisualScripting.Generated folder is removed when removing the VisualScripting package.
- Fixed error when executing "Fix Missing Scripts" in a HDRP project
- Visual Scripting Preferences spacing has been adjusted to avoid overlaps
- Fixed rendering of inactive ObjectFields
- Fixed sidebar (graph inspector/blackboard) resize when a vertical scrollbar is needed
- Fixed variable type reset to Enum when changing from Enum to GameObject when both Blackbaord and Variables inspector are displayed
- Help button in the visual scripting Assets and Behaviours inspector now link to the package documentation.
- FlowMachine type is now back in usable types.
- Fixed GraphPointerException occurs when nesting graph within itself [BOLT-1257](https://issuetracker.unity3d.com/issues/visual-scripting-graphpointerexception-occurs-when-nesting-graph-within-itself)
- Fixed RenamedFrom attribute does not function correctly on array references to a renamed type [BOLT-1149](https://issuetracker.unity3d.com/product/unity/issues/guid/BOLT-1149)
- Fixed error message when custom inspectors are generated
- Fixed missing succession for Cooldown. Output of Cooldown completed is treated as unentered.  [BOLT-725](https://issuetracker.unity3d.com/issues/bolt-1-output-of-cooldown-completed-is-treated-as-unentered)
- Fixed infinite loop when setting the For unit's step's default value to 0. Instead, the unit won't be executed and the exit output will be triggered directly.
- Fixed Object Variables tabs not updated when creating a Prefab
- Fixed console errors when deleting a Prefab with a Visual Script
- Fixed console errors when editing nested graphs during Play Mode
- Fixed console errors when opening the standalone profiler window

## [1.5.1-pre.5] - 2021-01-20
### Changed
- Removed code referring to an unused SceneManagement.PrefabStage API

## [1.5.1-pre.3] - 2020-12-07
### Added
- Added Visual Scripting as built-in package as of Unity 2021.1
- Added New Input System Support. You can import the Input System package, activate the back-end and regenerate units to use.
- Added AOT Pre-Compile to automatically run when building AOT platforms
- Improved UI for deprecated built-in nodes
- Added automatic unit generation the first time the graph window is opened
### Changed
- Switched to delivering source instead of pre-built .NET 3/4 assemblies
- Updated Documentation
- Renamed assemblies to match Unity.VisualScripting naming scheme (Ex: Bolt.Core -> Unity.VisualScripting.Core)
- Merged Ludiq.Core and Ludiq.Graphs into Unity.VisualScripting.Core
- Moved Setup Wizard contents from pop-up on Editor startup to Player Settings. You can change the default settings from "Player Settings > Visual Scripting"
- Renamed "Assembly Options" to "Node Library"
- Renamed "Flow Graph" to "Script Graph"
- Renamed "Flow Machine" to "Script Machine"
- Renamed "Macro" graphs to "Graph" in machine source configuration and "GraphAsset" in Assets
- Renamed "Control Input/Output" to "Trigger Input/Output"
- Renamed "Value Input/Output" to "Data Input/Output"
- Updated built-in nodes. The Fuzzy Finder still accepts earlier version names of nodes.
- Renamed "Branch" node to "If"
- Renamed "Self" node to "This"
- Deprecated the previous Add unit. The Sum unit has been renamed to Add.
- Updated Window Naming   
- Changed "Variables" window to "Blackboard"
- Changed "Graph" window to "Script Graph" and "State Graph"
- Updated Bolt Preferences
- Renamed Bolt Preferences to "Visual Scripting"
- Removed BoltEx
- Moved settings previously accessed from "Window > Bolt" to preferences
- Renamed Control Schemes from "Unity/Unreal" to "Default/Alternate" (Neither control scheme currently matches their respective editors' controls and will be updated in a future release)
- Consolidated Graph editor, Blackboard and Graph Inspector into a single window
- Updated Third-Party Notices
- Plugin version information has been removed from the Visual Scripting settings window. This information can be retrieved from the Package Manager.
### Fixed
- Corrected UGUI event management to trickle down correctly when the hierarchy contains a Unity Message Listener [BOLT-2](https://issuetracker.unity3d.com/issues/bolt-1-unity-message-listener-blocks-proper-trickling-of-ugui-events-in-hierarchies)
- Fixed backup failures with large projects [BOLT-10](https://issuetracker.unity3d.com/issues/bolt-1-backup-fails-to-complete)
- Fixed "Null Reference" when opening the Graph Window for the first time [BOLT-996](https://issuetracker.unity3d.com/issues/nullreferenceexception-when-graph-window-is-opened-on-a-new-project)
- Fixed IL2CPP build crash on startup [BOLT-1036](https://issuetracker.unity3d.com/issues/bolt-bolt-1-il2cpp-release-build-crashes-on-startup-when-there-is-at-least-1-node-present-in-a-graph)
- Fixed IL2CPP issue around converting certain managed types [BOLT-8](https://issuetracker.unity3d.com/issues/bolt-1-il2cpp-encountered-a-managed-type-which-it-cannot-convert-ahead-of-time)
- Fixed deserialization issues when undoing graphs with Wait nodes [BOLT-679](https://issuetracker.unity3d.com/issues/bolt-deserialization-error-and-nodes-missing-after-pressing-undo-when-update-coroutine-with-wait-node-is-present-in-graph)
- Fixed "SelectOnEnum" node behavior enums containing non-unique values e.g. "RuntimePlatform" [BOLT-688](https://issuetracker.unity3d.com/issues/select-on-enum-doesnt-work-with-the-runtimeplatform-enum)
