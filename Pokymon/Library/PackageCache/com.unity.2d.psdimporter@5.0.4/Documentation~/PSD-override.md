# PSD File Importer Override

From Unity 2019.30f1 onwards, the PSD Importer can be customized to import files with the .psd extension. To do that you will need to have custom scripts that allows you to do that by calling the `AssetDatabaseExperimental.SetImporterOverride` method. The following are examples on how to use the API:

## PSDImporterOverride.cs
```
using UnityEngine;

namespace UnityEditor.U2D.PSD
{
    [ScriptedImporter(1, "psd", AutoSelect = false)]
    internal class PSDImporterOverride : PSDImporter
    {

        [MenuItem("Assets/2D Importer", false, 30)]
        [MenuItem("Assets/2D Importer/Change PSD File Importer", false, 30)]
        static void ChangeImporter()
        {
            foreach (var obj in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                var ext = System.IO.Path.GetExtension(path);
                if (ext == ".psd")
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer is PSDImporterOverride)
                    {
                        Debug.Log(string.Format("{0} is now imported with TextureImporter", path));
                        AssetDatabaseExperimental.ClearImporterOverride(path);
                    }
                    else
                    {
                        Debug.Log(string.Format("{0} is now imported with PSDImporter", path));
                        AssetDatabaseExperimental.SetImporterOverride<PSDImporterOverride>(path);
                    }
                }
            }
        }
    }
}
```

## PSDImporterOverrideEditor.cs
```
namespace UnityEditor.U2D.PSD
{
    [CustomEditor(typeof(UnityEditor.U2D.PSD.PSDImporterOverride))]
    internal class PSDImporterOverrideEditor : PSDImporterEditor
    {
    }

}
```
