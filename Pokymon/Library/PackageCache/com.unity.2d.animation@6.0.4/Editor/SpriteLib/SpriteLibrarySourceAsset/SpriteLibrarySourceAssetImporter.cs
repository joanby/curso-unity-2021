using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace UnityEditor.U2D.Animation
{
    /// <summary>
    /// A ScriptedImporter that imports .spriteLib extension file to generate
    /// SpriteLibraryAsset
    /// </summary>
    [ScriptedImporter(1, "spriteLib")]
    public class SpriteLibrarySourceAssetImporter : ScriptedImporter
    {
        [SerializeField] 
        private SpriteLibraryAsset m_PrimaryLibrary;
        
        /// <summary>
        /// Implementation of ScriptedImporter.OnImportAsset
        /// </summary>
        /// <param name="ctx">
        /// This argument contains all the contextual information needed to process the import
        /// event and is also used by the custom importer to store the resulting Unity Asset.
        /// </param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var spriteLib = ScriptableObject.CreateInstance<SpriteLibraryAsset>();
            spriteLib.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            var sourceAsset = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(assetPath);
            if (sourceAsset?.Length > 0)
            {
                var sourceLibraryAsset = sourceAsset[0] as SpriteLibrarySourceAsset;
                if (sourceLibraryAsset != null)
                {
                    foreach (var cat in sourceLibraryAsset.library)
                    {
                        spriteLib.AddCategoryLabel(null, cat.name, null);
                        foreach (var entry in cat.overrideEntries)
                        {
                            spriteLib.AddCategoryLabel(entry.spriteOverride, cat.name, entry.name);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sourceLibraryAsset.primaryLibraryID))
                {
                    var primaryAssetPath = AssetDatabase.GUIDToAssetPath(sourceLibraryAsset.primaryLibraryID);
                    if (primaryAssetPath != assetPath)
                    {
                        ctx.DependsOnArtifact(AssetDatabase.GUIDToAssetPath(sourceLibraryAsset.primaryLibraryID));
                        m_PrimaryLibrary = AssetDatabase.LoadAssetAtPath<SpriteLibraryAsset>(primaryAssetPath);                    
                    }
                }
            }

            ctx.AddObjectToAsset("SpriteLib", spriteLib, IconUtility.LoadIconResource("Sprite Library", "Icons/Light", "Icons/Dark"));
        }

        internal static SpriteLibrarySourceAsset LoadSpriteLibrarySourceAsset(string path)
        {
            var loadedObjects = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(path);
            foreach (var obj in loadedObjects)
            {
                if (obj is SpriteLibrarySourceAsset)
                    return (SpriteLibrarySourceAsset)obj;
            }
            return null;
        }

        internal static void SaveSpriteLibrarySourceAsset(SpriteLibrarySourceAsset obj, string path)
        {
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new [] {obj}, path, true);
        }
            
        [MenuItem("internal:Assets/Convert to SpriteLibrarySourceAsset", true)]
        static bool ConvertToSpriteLibrarySourceAssetValidate()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is SpriteLibraryAsset)
                    return true;
            }
            return false;
        }
        
        [MenuItem("internal:Assets/Convert to SpriteLibrarySourceAsset")]
        static void ConvertToSourceAsset()
        {
        
            foreach (var obj in Selection.objects)
            {
                if (obj is SpriteLibraryAsset)
                {
                    var asset = (SpriteLibraryAsset) obj;
                    var path = AssetDatabase.GetAssetPath(asset);
                    var currentAssetPath = Path.GetDirectoryName(path);
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    var convertFileName = fileName + ".spriteLib";
                    convertFileName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(currentAssetPath, convertFileName));
                    var convertAsset = ScriptableObject.CreateInstance<SpriteLibrarySourceAsset>();
                    convertAsset.library = new List<SpriteLibCategoryOverride>(asset.categories.Count);
                    for (int i = 0; i < asset.categories.Count; ++i)
                    {
                        var category = asset.categories[i];
                        var newCategory = new SpriteLibCategoryOverride()
                        {
                            overrideEntries = new List<SpriteCategoryEntryOverride>(category.categoryList.Count),
                            name = category.name,
                            entryOverrideCount = 0,
                            fromMain = false
                        };
                        convertAsset.library.Add(newCategory);
                        for (int j = 0; j < category.categoryList.Count; ++j)
                        {
                            newCategory.overrideEntries.Add(new SpriteCategoryEntryOverride()
                            {
                                name = category.categoryList[j].name,
                                sprite = null,
                                fromMain = false,
                                spriteOverride = category.categoryList[j].sprite
                            });
                        }
                    }
                    SpriteLibrarySourceAssetImporter.SaveSpriteLibrarySourceAsset(convertAsset, convertFileName);
                }
            }
            AssetDatabase.Refresh();
        }
    }
    
    internal class SpriteLibrarySourceAssetPostProcessor: AssetPostprocessor
    {
        void OnPreprocessAsset()
        {
            if (assetImporter is SpriteLibrarySourceAssetImporter)
            {
                var obj = SpriteLibrarySourceAssetImporter.LoadSpriteLibrarySourceAsset(assetPath);
                if (obj != null)
                {
                    SpriteLibraryAsset mainLibraryAsset = null;
                    var mainLibraryAssetAssetPath = AssetDatabase.GUIDToAssetPath(obj.primaryLibraryID);
                    mainLibraryAsset =  AssetDatabase.LoadAssetAtPath<SpriteLibraryAsset>(mainLibraryAssetAssetPath);
                    var so = new SerializedObject(obj);
                    var library = so.FindProperty("m_Library");
                    SpriteLibraryDataInspector.UpdateLibraryWithNewMainLibrary(mainLibraryAsset, library);
                    if (so.hasModifiedProperties)
                    {
                        so.ApplyModifiedPropertiesWithoutUndo();
                        SpriteLibrarySourceAssetImporter.SaveSpriteLibrarySourceAsset(obj, assetPath);
                    }
                }   
            }
        }
    }
}