using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class MigrationUtility_1_4_13_to_1_5_1
    {
        public static DictionaryAsset GetLegacyProjectSettingsAsset(string pluginId)
        {
            try
            {
                var rootPath = GetLegacyRootPath(pluginId);
                var settingsFullPath = Path.Combine(rootPath, "Generated", "ProjectSettings.asset");
                var settingsAssetPath = Path.Combine("Assets", PathUtility.FromAssets(settingsFullPath));
                var asset = AssetDatabase.LoadAssetAtPath<DictionaryAsset>(settingsAssetPath);
                return asset;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetLegacyIconMapAssetPath(string pluginId)
        {
            try
            {
                var rootPath = GetLegacyRootPath(pluginId);
                var iconMapFullPath = Path.Combine(rootPath, "IconMap");
                var iconMapAssetPath = Path.Combine("Assets", PathUtility.FromAssets(iconMapFullPath));
                return iconMapAssetPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void TransferSettings(DictionaryAsset legacySettingsDict, List<ProjectSettingMetadata> settingList)
        {
            var skippedKeys = new HashSet<string>() { "savedVersion", "projectSetupCompleted" };

            foreach (var legacySetting in legacySettingsDict)
            {
                if (!skippedKeys.Contains(legacySetting.Key))
                {
                    foreach (var setting in settingList)
                    {
                        if (setting.key == legacySetting.Key)
                        {
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                            try
                            {
                                setting.value = legacySetting.Value;
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                                throw;
                            }
#else
                            setting.value = legacySetting.Value;
#endif
                            setting.Save();
                            break;
                        }
                    }
                }
            }
        }

        public static string GetLegacyRootPath(string pluginId)
        {
            var rootFileName = $"{pluginId}.root";
            var defaultRootFolderPath = Path.Combine(Paths.assets, "Ludiq", pluginId);
            // Quick & dirty optimization: looking in all directories is expensive,
            // so if the user left the plugin in the default directory that we ship
            // (directly under Plugins), we'll use this path directly.

            string rootFilePath;

            var defaultRootFilePath = Path.Combine(defaultRootFolderPath, rootFileName);

            if (File.Exists(defaultRootFilePath))
            {
                rootFilePath = defaultRootFilePath;
            }
            else
            {
                var rootFiles = Directory.GetFiles(Paths.assets, rootFileName, SearchOption.AllDirectories);

                if (rootFiles.Length > 1)
                {
                    throw new IOException($"More than one root files found ('{rootFileName}'). Cannot determine root path.");
                }
                else if (rootFiles.Length <= 0)
                {
                    throw new FileNotFoundException($"No root file found ('{rootFileName}'). Cannot determine root path.");
                }
                else // if (rootFiles.Length == 1)
                {
                    rootFilePath = rootFiles[0];
                }
            }

            return Directory.GetParent(rootFilePath).FullName;
        }

        public static SemanticVersion TryManualParseSavedVersion(string pluginId)
        {
            try
            {
                var oldProjectRootPath = MigrationUtility_1_4_13_to_1_5_1.GetLegacyRootPath(pluginId);
                var oldProjectSettingsPath = Path.Combine(oldProjectRootPath, "Generated", "ProjectSettings.asset");

                if (!File.Exists(oldProjectSettingsPath))
                {
                    return new SemanticVersion();
                }

                string projectSettingsText = System.IO.File.ReadAllText(oldProjectSettingsPath);
                int savedVersionIndex = projectSettingsText.IndexOf("savedVersion", StringComparison.Ordinal);
                if (savedVersionIndex == -1)
                {
                    return new SemanticVersion();
                }

                Match majorVersionMatch = new Regex(@"""major"":([0-9]*),").Match(projectSettingsText, savedVersionIndex);
                Match minorVersionMatch = new Regex(@"""minor"":([0-9]*),").Match(projectSettingsText, savedVersionIndex);
                Match patchVersionMatch = new Regex(@"""patch"":([0-9]*),").Match(projectSettingsText, savedVersionIndex);

                int majorVersion = int.Parse(majorVersionMatch.Groups[1].Value);
                int minorVersion = int.Parse(minorVersionMatch.Groups[1].Value);
                int patchVersion = int.Parse(patchVersionMatch.Groups[1].Value);

                return new SemanticVersion(majorVersion, minorVersion, patchVersion, null, 0);
            }
            catch (Exception)
            {
                return new SemanticVersion();
            }
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetEditorPrefMigrationsForPlugin(Plugin p)
        {
            var fieldInfo = p.GetType().GetField("ID", BindingFlags.Public | BindingFlags.Static);
            var renamedFromAttributes = fieldInfo.GetCustomAttributes(typeof(RenamedFromAttribute), true)
                .Cast<RenamedFromAttribute>();
            foreach (var renamed in renamedFromAttributes)
            {
                foreach (var editorPref in p.configuration.editorPrefs)
                {
                    var previousKey = EditorPrefMetadata.GetNamespacedKey(renamed.previousName, editorPref.key);
                    if (EditorPrefs.HasKey(previousKey))
                    {
                        yield return new KeyValuePair<string, string>(previousKey, editorPref.namespacedKey);
                    }
                }
            }
        }

        internal static void MigrateEditorPref(string fromKey, string toKey)
        {
            if (!EditorPrefs.HasKey(fromKey))
                throw new InvalidOperationException($"No Editor Pref with key {fromKey} found, could not perform migration from {fromKey} to {toKey}");

            var value = new SerializationData(EditorPrefs.GetString(fromKey)).Deserialize();

            EditorPrefs.SetString(toKey, value.Serialize().json);
        }

        public static void MigrateEditorPreferences(Plugin p)
        {
            var editorPrefMigrations = GetEditorPrefMigrationsForPlugin(p);
            foreach (var migration in editorPrefMigrations)
            {
                MigrateEditorPref(migration.Key, migration.Value);
            }

            // Now that our editor prefs have been migrated on the machine, re-load our editor prefs to memory
            foreach (var editorPref in p.configuration.editorPrefs)
            {
                editorPref.Load();
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_1_4_13_to_1_5_1 : PluginMigration
    {
        public Migration_1_4_13_to_1_5_1(Plugin plugin) : base(plugin)
        {
            order = 1;
        }

        public override SemanticVersion @from => "1.4.13";
        public override SemanticVersion to => "1.5.1";

        public override void Run()
        {
            RemoveLegacyPackageFiles();

            // We need to clear our cached types so that legacy types (Bolt.x, Ludiq.y, etc) aren't held in memory
            // by name. When we deserialize our graphs anew, we need to deserialize them into their new types (with new
            // namespaces) and the cached type lookup will interfere with that. See RuntimeCodebase.TryDeserializeType()
            RuntimeCodebase.ClearCachedTypes();

            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Core.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Core.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Flow.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Flow.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.State.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.State.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Core.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Core.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Graphs.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Graphs.Runtime");

            ScriptReferenceResolver.Run();

            plugin.configuration.Initialize();

            try
            {
                MigrateProjectSettings();
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting project settings. Be sure to check them in Edit -> Project Settings -> Visual Scripting");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }

            try
            {
                MigrationUtility_1_4_13_to_1_5_1.MigrateEditorPreferences(this.plugin);
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting editor preferences. Be sure to check them in Edit -> Preferences -> Visual Scripting");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }
        }

        private static void RemoveLegacyPackageFiles()
        {
            // Todo: This partially fails because we can't delete the loaded sqlite3 dll.
            // Causes no problems for the migration here, but leaves files for the user to delete

            // Remove Assemblies
            var rootPath = MigrationUtility_1_4_13_to_1_5_1.GetLegacyRootPath("Bolt.Core");
            var assembliesFullPath = $"{Directory.GetParent(rootPath).FullName}/Assemblies";
            var assembliesAssetPath = Path.Combine("Assets", PathUtility.FromAssets(assembliesFullPath));

            // Todo: This currently fails because of the sqlite dll. Deletes everything else
            AssetDatabase.DeleteAsset(assembliesAssetPath);

            // Remove icon map files
            AssetDatabase.DeleteAsset(MigrationUtility_1_4_13_to_1_5_1.GetLegacyIconMapAssetPath("Bolt.Core"));
            AssetDatabase.DeleteAsset(MigrationUtility_1_4_13_to_1_5_1.GetLegacyIconMapAssetPath("Bolt.Flow"));
            AssetDatabase.DeleteAsset(MigrationUtility_1_4_13_to_1_5_1.GetLegacyIconMapAssetPath("Bolt.State"));
        }

        private static void MigrateProjectSettings()
        {
            // Doing a manual migration of project settings because Ludiq.Graphs and Ludiq.Core were merged into Bolt.Core
            // and each plugin has some shared project settings (so we need to merge them as well)

            // Ludiq.Graphs + Ludiq.Core + Bolt.Core -> VisualScripting.Core
            var coreProjectSettings = BoltCore.Configuration.projectSettings;

            var ludiqGraphsProjectSettings = MigrationUtility_1_4_13_to_1_5_1.GetLegacyProjectSettingsAsset("Ludiq.Graphs");
            if (ludiqGraphsProjectSettings != null)
            {
                MigrationUtility_1_4_13_to_1_5_1.TransferSettings(ludiqGraphsProjectSettings, coreProjectSettings);
            }

            var ludiqCoreProjectSettings = MigrationUtility_1_4_13_to_1_5_1.GetLegacyProjectSettingsAsset("Ludiq.Core");
            if (ludiqCoreProjectSettings != null)
            {
                MigrationUtility_1_4_13_to_1_5_1.TransferSettings(ludiqCoreProjectSettings, coreProjectSettings);
            }

            var boltCoreProjectSettings = MigrationUtility_1_4_13_to_1_5_1.GetLegacyProjectSettingsAsset("Bolt.Core");
            if (boltCoreProjectSettings != null)
            {
                MigrationUtility_1_4_13_to_1_5_1.TransferSettings(boltCoreProjectSettings, coreProjectSettings);
            }

            BoltCore.Configuration.Save();
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_1_4_13_to_1_5_1_Post : PluginMigration
    {
        public Migration_1_4_13_to_1_5_1_Post(Plugin plugin) : base(plugin)
        {
            order = 3;
        }

        public override SemanticVersion @from => "1.4.13";
        public override SemanticVersion to => "1.5.1";

        public override void Run()
        {
            CleanupLegacyUserFiles();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private static void CleanupLegacyUserFiles()
        {
            // Todo: This partially fails because we can't delete the loaded sqlite3 dll.
            // Causes no problems for the migration here, but leaves files for the user to delete

            // Remove Old Ludiq folder, including project settings and unit database
            var rootPath = MigrationUtility_1_4_13_to_1_5_1.GetLegacyRootPath("Bolt.Core");
            var ludiqFolderFullPath = Directory.GetParent(rootPath).FullName;
            var ludiqFolderAssetPath = Path.Combine("Assets", PathUtility.FromAssets(ludiqFolderFullPath));

            AssetDatabase.DeleteAsset(ludiqFolderAssetPath);
        }
    }

    [Plugin(BoltCore.ID)]
    internal class DeprecatedSavedVersionLoader_1_4_13_to_1_5_1 : PluginDeprecatedSavedVersionLoader
    {
        public DeprecatedSavedVersionLoader_1_4_13_to_1_5_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.4.13";

        public override bool Run(out SemanticVersion savedVersion)
        {
            var manuallyParsedVersion = MigrationUtility_1_4_13_to_1_5_1.TryManualParseSavedVersion("Bolt.Core");
            savedVersion = manuallyParsedVersion;

            return savedVersion != "0.0.0";
        }
    }
}
