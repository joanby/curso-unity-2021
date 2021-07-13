using System.Collections.Generic;
using UnityEditor;

namespace Unity.VisualScripting
{
    public class VSProjectSettingsProvider : Editor
    {
        [SettingsProvider]
        public static SettingsProvider CreateProjectSettingProvider()
        {
            return new VSProjectSettingsProviderView();
        }
    }
}
