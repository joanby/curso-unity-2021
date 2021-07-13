using UnityEngine;
using UnityEditor;

namespace Unity.VisualScripting
{
    public class VSEditorSettingsProviderView : SettingsProvider
    {
        private const string path = "Preferences/Visual Scripting";
        private const string title = "Visual Scripting";
        public const string ID = "Bolt";

        public VSEditorSettingsProviderView() : base(path, SettingsScope.User)
        {
            label = title;
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Space(5f);

            GUILayout.Space(10f);

            // happens when opening unity with the settings window already opened. there's a delay until the singleton is assigned
            if (BoltCore.instance == null)
            {
                EditorGUILayout.HelpBox("Loading Configuration...", MessageType.Info);
                return;
            }

            BoltProduct instance = (BoltProduct)ProductContainer.GetProduct(ID);

            instance.configurationPanel.PreferenceItem();
        }
    }
}
