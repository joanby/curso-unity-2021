using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.U2D.Animation
{
    internal class SpriteLibrarySourceAsset : ScriptableObject
    {
        [SerializeField]
        private List<SpriteLibCategoryOverride> m_Library = new List<SpriteLibCategoryOverride>();
        [SerializeField]
        private string m_PrimaryLibraryGUID;

        public List<SpriteLibCategoryOverride> library
        {
            get => m_Library;
            set => m_Library = value;
        }

        public string primaryLibraryID
        {
            get => m_PrimaryLibraryGUID;
            set => m_PrimaryLibraryGUID = value;
        }
    }
}