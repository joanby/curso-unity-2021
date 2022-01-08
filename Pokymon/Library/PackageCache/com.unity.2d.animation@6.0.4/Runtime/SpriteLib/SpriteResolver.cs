using System;
using UnityEngine.Animations;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.Animation
{
    /// <summary>
    /// Updates a SpriteRenderer's Sprite reference on the Category and Label value it is set
    /// </summary>
    /// <Description>
    /// By setting the SpriteResolver's Category and Label value, it will request for a Sprite from
    /// a SpriteLibrary Component the Sprite that is registered for the Category and Label.
    /// If a SpriteRenderer is present in the same GameObject, the SpriteResolver will update the
    /// SpriteRenderer's Sprite reference to the corresponding Sprite.
    /// </Description>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("2D Animation/Sprite Resolver")]
    [DefaultExecutionOrder(-2)]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.animation@latest/index.html?subfolder=/manual/SLAsset.html%23sprite-resolver-component")]
    [MovedFrom("UnityEngine.Experimental.U2D.Animation")]
    public class SpriteResolver : MonoBehaviour, ISerializationCallbackReceiver
    {
        // SpriteKey is the new animation key.
        // We are keeping the old ones so that the animation clip doesn't braek
        
        // These are for animation
        [SerializeField]
        private float m_CategoryHash = 0;
        [SerializeField]
        private float m_labelHash = 0;

        [SerializeField]
        private float m_SpriteKey = 0;
        
        // For comparing hash values
        private int m_CategoryHashInt;
        private int m_LabelHashInt;
        private int m_SpriteKeyInt;

        // For OnUpdate during animation playback
        private int m_PreviousCategoryHash;
        private int m_PreviouslabelHash;
        private int m_PreviousSpriteKeyInt;

#if UNITY_EDITOR
        bool m_SpriteLibChanged;
        public event Action onDeserializedCallback = () => { };
#endif

        void Reset()
        {
            // If the Sprite referred to by the SpriteRenderer exist in the library,
            // we select the Sprite
            if(spriteRenderer)
                SetSprite(spriteRenderer.sprite);
            
        }

        void SetSprite(Sprite sprite)
        {
            var sl = spriteLibrary;
            if (sl != null && sprite != null)
            {
                foreach (var cat in sl.categoryNames)
                {
                    var entries = sl.GetEntryNames(cat);
                    foreach (var ent in entries)
                    {
                        if (sl.GetSprite(cat, ent) == sprite)
                        {
                            spriteKeyInt = SpriteLibrary.GetHashForCategoryAndEntry(cat, ent);
                            return;
                        }
                    }
                }
            }
        }
        
        void OnEnable()
        {
            m_CategoryHashInt = ConvertFloatToInt(m_CategoryHash);
            m_PreviousCategoryHash = m_CategoryHashInt;
            m_LabelHashInt = ConvertFloatToInt(m_labelHash);
            m_PreviouslabelHash = m_LabelHashInt;
            
            m_SpriteKeyInt = ConvertFloatToInt(m_SpriteKey);
            if (m_SpriteKeyInt == 0)
            {
                m_SpriteKey = ConvertCategoryLabelHashToSpriteKey(spriteLibrary, m_CategoryHashInt, m_LabelHashInt);
                m_SpriteKeyInt = ConvertFloatToInt(m_SpriteKey);
            }
            m_PreviousSpriteKeyInt = m_SpriteKeyInt;
            ResolveSpriteToSpriteRenderer();
        }

        SpriteRenderer spriteRenderer
        {
            get { return GetComponent<SpriteRenderer>(); }
        }

        /// <summary>
        /// Set the Category and label to use
        /// </summary>
        /// <param name="category">Category to use</param>
        /// <param name="label">Label to use</param>
        public void SetCategoryAndLabel(string category, string label)
        {
            spriteKeyInt = SpriteLibrary.GetHashForCategoryAndEntry(category, label);
            m_PreviousSpriteKeyInt = spriteKeyInt;
            ResolveSpriteToSpriteRenderer();
        }

        /// <summary>
        /// Get the Category set for the SpriteResolver
        /// </summary>
        /// <returns>The Category's name</returns>
        public string GetCategory()
        {
            var returnString = "";
            var sl = spriteLibrary;
            if (sl)
            {
                sl.GetCategoryAndEntryNameFromHash(spriteKeyInt, out returnString, out _);
            }
                

            return returnString;
        }

        /// <summary>
        /// Get the Label set for the SpriteResolver
        /// </summary>
        /// <returns>The Label's name</returns>
        public string GetLabel()
        {
            var returnString = "";
            var sl = spriteLibrary;
            if (sl)
                sl.GetCategoryAndEntryNameFromHash(spriteKeyInt, out _, out returnString);

            return returnString;
        }

        /// <summary>
        /// Property to get the SpriteLibrary the SpriteResolver is resolving from
        /// </summary>
        public SpriteLibrary spriteLibrary => gameObject.GetComponentInParent<SpriteLibrary>(true);

        void LateUpdate()
        {
            m_SpriteKeyInt = ConvertFloatToInt(m_SpriteKey);
            if (m_SpriteKeyInt != m_PreviousSpriteKeyInt)
            {
                m_PreviousSpriteKeyInt = m_SpriteKeyInt;
                ResolveSpriteToSpriteRenderer();
            }
            else
            {
                m_CategoryHashInt = ConvertFloatToInt(m_CategoryHash);
                m_LabelHashInt = ConvertFloatToInt(m_labelHash);
                if (m_LabelHashInt != m_PreviouslabelHash || m_CategoryHashInt != m_PreviousCategoryHash)
                {
                    if (spriteLibrary != null)
                    {
                        m_PreviousCategoryHash = m_CategoryHashInt;
                        m_PreviouslabelHash = m_LabelHashInt;
                        m_SpriteKey = ConvertCategoryLabelHashToSpriteKey(spriteLibrary, m_CategoryHashInt, m_LabelHashInt);
                        m_SpriteKeyInt = ConvertFloatToInt(m_SpriteKey);
                        m_PreviousSpriteKeyInt = m_SpriteKeyInt;
                        ResolveSpriteToSpriteRenderer();
                    }
                }
            }
        }

        internal static float ConvertCategoryLabelHashToSpriteKey(SpriteLibrary library, int categoryHash, int labelHash)
        {
            if (library != null)
            {
                foreach(var category in library.categoryNames)
                {
                    if (categoryHash == SpriteLibraryAsset.GetStringHash(category))
                    {
                        var entries = library.GetEntryNames(category);
                        if (entries != null)
                        {
                            foreach (var entry in entries)
                            {
                                if (labelHash == SpriteLibraryAsset.GetStringHash(entry))
                                {
                                    var spriteKey = SpriteLibrary.GetHashForCategoryAndEntry(category, entry); 
                                    return ConvertIntToFloat(spriteKey);
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }
        
        internal Sprite GetSprite(out bool validEntry)
        {
            var lib = spriteLibrary;
            if (lib != null)
            {
                return lib.GetSpriteFromCategoryAndEntryHash(m_SpriteKeyInt, out validEntry);
            }
            validEntry = false;
            return null;
        }

        /// <summary>
        /// Set the Sprite in SpriteResolver to the SpriteRenderer component that is in the same GameObject.
        /// </summary>
        public void ResolveSpriteToSpriteRenderer()
        {
            m_PreviousSpriteKeyInt = m_SpriteKeyInt;
            bool validEntry;
            var sprite = GetSprite(out validEntry);
            var sr = spriteRenderer;
            if (sr != null && (sprite != null || validEntry))
                sr.sprite = sprite;
        }
        
        void OnTransformParentChanged()
        {
            ResolveSpriteToSpriteRenderer();
#if UNITY_EDITOR
            spriteLibChanged = true;
#endif
        }

        int spriteKeyInt
        {
            get { return m_SpriteKeyInt; }
            set
            {
                m_SpriteKeyInt = value;
                m_SpriteKey = ConvertIntToFloat(m_SpriteKeyInt);
            }
        }

        internal unsafe static int ConvertFloatToInt(float f)
        {
            float* fp = &f;
            int* i = (int*)fp;
            return *i;
        }

        internal unsafe static float ConvertIntToFloat(int f)
        {
            int* fp = &f;
            float* i = (float*)fp;
            return *i;
        }

#if UNITY_EDITOR
        internal bool spriteLibChanged
        {
            get {return m_SpriteLibChanged;}
            set { m_SpriteLibChanged = value; }
        }
#endif
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if UNITY_EDITOR
            onDeserializedCallback();
#endif            
        }
    }
}
