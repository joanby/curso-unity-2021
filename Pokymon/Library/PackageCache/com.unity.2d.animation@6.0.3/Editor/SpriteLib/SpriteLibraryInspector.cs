using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.U2D.Animation;

namespace UnityEditor.U2D.Animation
{
    [CustomEditor(typeof(SpriteLibrary))]
    [CanEditMultipleObjects]
    [MovedFrom("UnityEditor.Experimental.U2D.Animation")]
    internal class SpriteLibraryInspector : Editor
    {
        class SpriteLibCombineCache : ScriptableObject
        {
            [SerializeField]
            List<SpriteLibCategoryOverride> m_Library = new List<SpriteLibCategoryOverride>();

            public List<SpriteLibCategoryOverride> library
            {
                get =>  m_Library;
            }
        }

        static class Style
        {
            public static string libraryDifferentValue = L10n.Tr("Sprite Library has different values.");
        }
        
        private SpriteLibCombineCache m_SpriteLibCombineCache;
        private SerializedObject m_InternalCombineCacheSO;
        private SerializedProperty m_SpriteLibraryAsset;
        private SerializedProperty m_Library;
        private SerializedProperty m_LibraryOverride;
        private SpriteLibraryDataInspector m_SpriteLibraryDataInspector;
        public void OnEnable()
        {
            m_SpriteLibCombineCache = ScriptableObject.CreateInstance<SpriteLibCombineCache>();
            m_InternalCombineCacheSO = new SerializedObject(m_SpriteLibCombineCache);
            
            m_SpriteLibraryAsset = serializedObject.FindProperty("m_SpriteLibraryAsset");
            m_Library = serializedObject.FindProperty("m_Library");
            m_LibraryOverride = m_InternalCombineCacheSO.FindProperty("m_Library");
            InitSpriteLibraryDataCache();
        }

        void InitSpriteLibraryDataCache()
        {
            if (!m_Library.hasMultipleDifferentValues && !m_SpriteLibraryAsset.hasMultipleDifferentValues)
            {
                ConvertSpriteLibraryToOverride(m_SpriteLibCombineCache.library, m_Library);
                m_InternalCombineCacheSO.Update();
                SpriteLibraryDataInspector.UpdateLibraryWithNewMainLibrary((SpriteLibraryAsset)m_SpriteLibraryAsset.objectReferenceValue, m_LibraryOverride);
                m_SpriteLibraryDataInspector = new SpriteLibraryDataInspector(serializedObject, m_LibraryOverride);
                m_InternalCombineCacheSO.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_InternalCombineCacheSO.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_SpriteLibraryAsset);
            if (EditorGUI.EndChangeCheck())
            {
                InitSpriteLibraryDataCache();
                serializedObject.ApplyModifiedProperties();
                foreach (var t in targets)
                {
                    var srs = (t as SpriteLibrary).GetComponentsInChildren<SpriteResolver>();
                    foreach (var sr in srs)
                    {
                        sr.ResolveSpriteToSpriteRenderer();
                        sr.spriteLibChanged = true;
                    }
                }
            }

            if (m_SpriteLibraryDataInspector != null)
            {
                EditorGUI.BeginChangeCheck();
                m_SpriteLibraryDataInspector.OnGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    m_InternalCombineCacheSO.ApplyModifiedProperties();
                    ConvertOverrideToSpriteLibrary(m_SpriteLibCombineCache.library, m_Library);
                    serializedObject.ApplyModifiedProperties();

                    foreach (var t in targets)
                    {
                        var spriteLib = (SpriteLibrary) t;
                        spriteLib.CacheOverrides();
                        var srs = spriteLib.GetComponentsInChildren<SpriteResolver>();
                        foreach (var sr in srs)
                        {
                            sr.ResolveSpriteToSpriteRenderer();
                            sr.spriteLibChanged = true;
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(Style.libraryDifferentValue, MessageType.Info);
            }
        }

        void ConvertSpriteLibraryToOverride(List<SpriteLibCategoryOverride> libOverride, SerializedProperty lib)
        {
            libOverride.Clear();
            if (lib.arraySize > 0)
            {
                var categoryEntries = lib.GetArrayElementAtIndex(0);
                for (int i = 0; i < lib.arraySize; ++i)
                {
                    var overrideCategory = new SpriteLibCategoryOverride()
                    {
                        categoryList = new List<SpriteCategoryEntry>(),
                        entryOverrideCount = 0,
                        fromMain = false,
                        name = categoryEntries.FindPropertyRelative(SpriteLibraryPropertyString.name).stringValue,
                        overrideEntries = new List<SpriteCategoryEntryOverride>()
                    };
                    var entries = categoryEntries.FindPropertyRelative(SpriteLibraryPropertyString.categoryList);
                    var overrideCategoryEntries = overrideCategory.overrideEntries;
                    if (entries.arraySize > 0)
                    {
                        var entry = entries.GetArrayElementAtIndex(0); 
                        for (int j = 0; j < entries.arraySize; ++j)
                        {
                            overrideCategoryEntries.Add(new SpriteCategoryEntryOverride()
                            {
                                fromMain = false,
                                name = entry.FindPropertyRelative(SpriteLibraryPropertyString.name).stringValue,
                                sprite = (Sprite)entry.FindPropertyRelative(SpriteLibraryPropertyString.sprite).objectReferenceValue,
                                spriteOverride = (Sprite)entry.FindPropertyRelative(SpriteLibraryPropertyString.sprite).objectReferenceValue
                            });
                            entry.Next(false);
                        }
                    }
                    libOverride.Add(overrideCategory);
                    categoryEntries.Next(false);
                }
            }
        }
        
        void ConvertOverrideToSpriteLibrary(List<SpriteLibCategoryOverride> libOverride, SerializedProperty lib)
        {
            lib.arraySize = 0;
            if (libOverride.Count > 0)
            {
                for (int i = 0; i < libOverride.Count; ++i)
                {
                    var libOverrideElement = libOverride[i];
                    if (!libOverrideElement.fromMain || libOverrideElement.entryOverrideCount > 0)
                    {
                        lib.arraySize += 1;
                        var libElement = lib.GetArrayElementAtIndex(lib.arraySize - 1);
                        libElement.FindPropertyRelative(SpriteLibraryPropertyString.name).stringValue = libOverrideElement.name;
                        var overrideEntries = libOverrideElement.overrideEntries;
                        var entries = libElement.FindPropertyRelative(SpriteLibraryPropertyString.categoryList);
                        entries.arraySize = 0;
                        if (overrideEntries.Count > 0)
                        {
                            for (int j = 0; j < overrideEntries.Count; ++j)
                            {
                                var overrideEntry = overrideEntries[j];
                                if (!overrideEntry.fromMain ||
                                    overrideEntry.sprite != overrideEntry.spriteOverride)
                                {
                                    entries.arraySize += 1;
                                    var entry = entries.GetArrayElementAtIndex(entries.arraySize - 1);
                                    entry.FindPropertyRelative(SpriteLibraryPropertyString.name).stringValue = overrideEntry.name;
                                    entry.FindPropertyRelative(SpriteLibraryPropertyString.sprite).objectReferenceValue = overrideEntry.spriteOverride;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
