using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Migration_1_4_13_to_1_5_1 : PluginMigration
    {
        public Migration_1_4_13_to_1_5_1(Plugin plugin) : base(plugin)
        {
            order = 2;
        }

        public override SemanticVersion @from => "1.4.13";
        public override SemanticVersion to => "1.5.1";

        public override void Run()
        {
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

        private void MigrateProjectSettings()
        {
            // Bolt.State -> VisualScripting.State
            var stateProjectSettings = BoltState.Configuration.projectSettings;

            var boltStateProjectSettings = MigrationUtility_1_4_13_to_1_5_1.GetLegacyProjectSettingsAsset("Bolt.State");
            if (boltStateProjectSettings != null)
            {
                MigrationUtility_1_4_13_to_1_5_1.TransferSettings(boltStateProjectSettings, stateProjectSettings);
            }
        }
    }

    [Plugin(BoltState.ID)]
    internal class DeprecatedSavedVersionLoader_1_4_13_to_1_5_1 : PluginDeprecatedSavedVersionLoader
    {
        public DeprecatedSavedVersionLoader_1_4_13_to_1_5_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.4.13";

        public override bool Run(out SemanticVersion savedVersion)
        {
            var manuallyParsedVersion = MigrationUtility_1_4_13_to_1_5_1.TryManualParseSavedVersion("Bolt.State");
            savedVersion = manuallyParsedVersion;

            return savedVersion != "0.0.0";
        }
    }
}
