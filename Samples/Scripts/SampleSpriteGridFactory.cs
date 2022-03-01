using System;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
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

                return pool.Get(idx, () => new Tuple<Texture2D, uint, uint, float>(
                    textures[idx], 32, 32, 32
                ));
            }
        }
    }
}
