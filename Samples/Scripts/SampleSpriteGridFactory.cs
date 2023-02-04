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

                return pool.Get(idx, () => new Tuple<Texture2D, Rect?, Size2D, Size2D, float, Action, Action>(
                    textures[idx], null, new Size2D { Width = 32, Height = 32}, new Size2D { Width = 0, Height = 0}, 32, null, null
                ));
            }
        }
    }
}
