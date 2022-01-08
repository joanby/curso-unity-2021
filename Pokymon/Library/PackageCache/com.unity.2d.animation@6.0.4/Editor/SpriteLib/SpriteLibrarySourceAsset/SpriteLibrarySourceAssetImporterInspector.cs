using System;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Object = UnityEngine.Object;

namespace UnityEditor.U2D.Animation
{
    [CustomEditor(typeof(SpriteLibrarySourceAssetImporter))]
    internal class SpriteLibrarySourceAssetImporterInspector : ScriptedImporterEditor
    {
        private SerializedProperty m_PrimaryLibraryGUID;
        private SerializedProperty m_Library;
        private SpriteLibraryAsset m_MainSpriteLibraryAsset;
        private SpriteLibraryDataInspector m_SpriteLibraryDataInspector;
        public override void OnEnable()
        {
            base.OnEnable();
            m_PrimaryLibraryGUID = extraDataSerializedObject.FindProperty("m_PrimaryLibraryGUID");
            if (!m_PrimaryLibraryGUID.hasMultipleDifferentValues && !string.IsNullOrEmpty(m_PrimaryLibraryGUID.stringValue))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(m_PrimaryLibraryGUID.stringValue);
                m_MainSpriteLibraryAsset =  AssetDatabase.LoadAssetAtPath<SpriteLibraryAsset>(assetPath);
            }
            m_Library = extraDataSerializedObject.FindProperty("m_Library");
            m_SpriteLibraryDataInspector = new SpriteLibraryDataInspector(extraDataSerializedObject, m_Library);
            
        }

        protected override Type extraDataType => typeof(SpriteLibrarySourceAsset);
        
        protected override void InitializeExtraDataInstance(Object extraTarget, int targetIndex)
        {
            var obj = SpriteLibrarySourceAssetImporter.LoadSpriteLibrarySourceAsset(((AssetImporter) targets[targetIndex]).assetPath);
            if (obj != null)
            {
                var extraTargetSourceAsset = extraTarget as SpriteLibrarySourceAsset;
                extraTargetSourceAsset.library = obj.library;
                extraTargetSourceAsset.primaryLibraryID = obj.primaryLibraryID;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            extraDataSerializedObject.Update();
            DoMainAssetGUI();
            DoLibraryGUI();
            serializedObject.ApplyModifiedProperties();
            extraDataSerializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        protected override void Apply()
        {
            base.Apply();
            for (int i = 0; i < targets.Length; i++)
            {
                string path = ((AssetImporter)targets[i]).assetPath;
                var sourceAsset = (SpriteLibrarySourceAsset) extraDataTargets[i];
                SpriteLibrarySourceAssetImporter.SaveSpriteLibrarySourceAsset(sourceAsset, path);
            }
        }
        
        void DoMainAssetGUI()
        {
            EditorGUI.BeginChangeCheck();
            if (m_PrimaryLibraryGUID.hasMultipleDifferentValues)
                EditorGUI.showMixedValue = true;
            m_MainSpriteLibraryAsset = AssetDatabase.LoadAssetAtPath<SpriteLibraryAsset>(AssetDatabase.GUIDToAssetPath(m_PrimaryLibraryGUID.stringValue));
            m_MainSpriteLibraryAsset = EditorGUILayout.ObjectField(Style.mainAssetLabel, m_MainSpriteLibraryAsset, typeof(SpriteLibraryAsset), false) as SpriteLibraryAsset;
            if (EditorGUI.EndChangeCheck())
            {
                m_PrimaryLibraryGUID.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_MainSpriteLibraryAsset));
                SpriteLibraryDataInspector.UpdateLibraryWithNewMainLibrary(m_MainSpriteLibraryAsset, m_Library);
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.showMixedValue = false;
        }

        void DoLibraryGUI()
        {
            m_SpriteLibraryDataInspector.OnGUI();
        }
        
        public override bool showImportedObject
        {
            get { return false; }
        }
        
        static class Style
        {
            public static GUIContent mainAssetLabel = new GUIContent("Main Library");
        }
    }
    
    internal class CreateSpriteLibrarySourceAsset : ProjectWindowCallback.EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var asset = ScriptableObject.CreateInstance<SpriteLibrarySourceAsset>();
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { asset }, pathName, true);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        [MenuItem("Assets/Create/2D/Sprite Library Asset", priority = 9)]
        static private void CreateSpriteLibrarySourceAssetMenu()
        {
            var action = ScriptableObject.CreateInstance<CreateSpriteLibrarySourceAsset>();
            var icon = IconUtility.LoadIconResource("Sprite Library", "Icons/Light", "Icons/Dark");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, "SpriteLib.spriteLib", icon, null);
        }
    }
}