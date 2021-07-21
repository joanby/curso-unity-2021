using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.U2D.Animation
{
    public class SkeletonAsset : ScriptableObject
    {
        [SerializeField] private SpriteBone[] m_SpriteBones;

        public SpriteBone[] GetSpriteBones()
        {
            return m_SpriteBones;
        }

        public void SetSpriteBones(SpriteBone[] spriteBones)
        {
            m_SpriteBones = spriteBones;
        }
    }    
}

