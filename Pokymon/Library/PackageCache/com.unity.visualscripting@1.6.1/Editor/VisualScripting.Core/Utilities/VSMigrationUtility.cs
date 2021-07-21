using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class VSMigrationUtility
    {
        private readonly List<Plugin> plugins;
        private readonly List<MigrationStep> steps;

        public VSMigrationUtility()
        {
            IEnumerable<Plugin> allPlugins = PluginContainer.GetAllPlugins();

            plugins = allPlugins.OrderByDependencies().ToList();

            steps = this.plugins
                .SelectMany(plugin =>
                    plugin.resources.pendingMigrations.Select(migration => new MigrationStep(plugin, migration)))
                .OrderBy(step => step.migration.order)
                .ToList();
        }

        public void OnUpdate()
        {
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                EditorUtility.DisplayDialog("Unity Visual Scripting Upgrade",
                    "We've detected an older version of Unity Visual Scripting (Bolt).\n\n" +
                    "We can't migrate your project unless you use ForceText as your serialization mode. Go to Edit -> Project Settings -> Editor -> Asset Serialization -> Mode to set it.\n\nRe-initiate the migration by installing the package.",
                    "OK / Uninstall");

                Client.Remove("com.unity.visualscripting");
                return;
            }

            var ok = EditorUtility.DisplayDialog("Unity Visual Scripting Upgrade",
                "We've detected an older version of Unity Visual Scripting (Bolt).\n\n" +
                "Your project and bolt assets will be backed up and migrated to work with the newest version. This can take a few minutes.",
                "Migrate My Project", "Cancel / Uninstall");

            if (!ok)
            {
                Client.Remove("com.unity.visualscripting");
                return;
            }

            VSBackupUtility.Backup();

            // ClearLog();

            foreach (var step in steps)
            {
                step.Reset();
                step.Run();

                if (step.state == MigrationStep.State.Failure)
                {
                    Debug.LogWarning(
                        $"VisualScripting - A migration step for {step.plugin.id} failed! Your project might be in an invalid state, restore your backup and try again...");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                    throw step.exception;
#endif
                }
            }

            Complete();
        }

        protected void Complete()
        {
            // Make sure all plugins are set to their latest version, even if they
            // don't have a migration to it.

            foreach (var plugin in plugins)
            {
                plugin.manifest.savedVersion = plugin.manifest.currentVersion;
                plugin.configuration.Save();
            }

            AssetDatabase.SaveAssets();

            var ok = EditorUtility.DisplayDialog("Unity Visual Scripting Upgrade",
                "Migration complete!", "OK");
        }

        private static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        internal class MigrationStep
        {
            public enum State
            {
                Idle,
                Migrating,
                Success,
                Failure
            }

            public MigrationStep(Plugin plugin, PluginMigration migration)
            {
                this.plugin = plugin;
                this.migration = migration;
            }

            internal readonly Plugin plugin;
            internal readonly PluginMigration migration;
            internal Exception exception;

            public State state { get; private set; }

            private EditorTexture GetStateIcon(State state)
            {
                switch (state)
                {
                    case State.Idle:
                        return BoltCore.Icons.empty;
                    case State.Migrating:
                        return BoltCore.Icons.progress;
                    case State.Success:
                        return BoltCore.Icons.successState;
                    case State.Failure:
                        return BoltCore.Icons.errorState;
                    default:
                        throw new UnexpectedEnumValueException<State>(state);
                }
            }

            public void Run()
            {
                state = State.Migrating;
                try
                {
                    migration.Run();
                    exception = null;
                    state = State.Success;
                    plugin.manifest.savedVersion = migration.to;
                    InternalEditorUtility.RepaintAllViews();
                }
                catch (Exception ex)
                {
                    state = State.Failure;
                    exception = ex;
                }
            }

            public void Reset()
            {
                state = State.Idle;
                exception = null;
            }
        }
    }
}
