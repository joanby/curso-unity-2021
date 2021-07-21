using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEngine.U2D.Animation
{
    [AddComponentMenu("")]
    [System.Obsolete]
    internal class SpriteSkinEntity : MonoBehaviour
    {
        void OnEnable()
        {
            Debug.LogWarning("SpriteSkinEntity will be removed in 2D Animation 7.0", this);
        }
    } 
}