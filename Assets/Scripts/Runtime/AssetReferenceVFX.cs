#if ADDRESSABLE

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Lib
{
    [global::System.Serializable]
    public class AssetReferenceVFX : AssetReferenceT<GameObject>
    { 
        /// <summary>
        /// Construct a new AssetReference object.
        /// </summary>
        /// <param name="guid">The guid of the asset.</param>
        public AssetReferenceVFX(string guid) : base(guid)
        {
        }

        /// <inheritdoc/>
        public override bool ValidateAsset(Object obj)
        {
#if UNITY_EDITOR
            var prefab = (GameObject)obj;
            return prefab.TryGetComponent<ParticleSystem>(out _);
#else
        return false;
#endif
        }

        /// <inheritdoc/>
        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(path) != typeof(GameObject))
            {
                return false;
            }

            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab.TryGetComponent<ParticleSystem>(out _);
#else
        return false;
#endif
        }
    }
}
#endif

