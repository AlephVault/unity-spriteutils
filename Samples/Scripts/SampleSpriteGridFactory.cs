using System;
using AlephVault.Unity.SpriteUtils.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Samples
    {
        public class SampleSpriteGridFactory : MonoBehaviour
        {
            [SerializeField]
            private Texture2D[] textures;

            private IdentifiedSpriteGridPool<int> pool = new IdentifiedSpriteGridPool<int>();
            
            public IdentifiedSpriteGrid<int> Get(int idx)
            {
                if (idx >= textures.Length || idx < 0) throw new IndexOutOfRangeException(
                    "Choose a valid texture index among the configured textures"
                );

                return pool.Get(idx, () => new Tuple<Texture2D, Rect?, uint, uint, float, Action, Action>(
                    textures[idx], null, 32, 32, 32, null, null
                ));
            }
        }
    }
}
