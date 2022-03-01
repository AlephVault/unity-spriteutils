using System;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Authoring
    {
        namespace Types
        {
            /// <summary>
            ///   <para>
            ///     An identified sprite grid is like a sprite grid but it is also identified
            ///     by a particular key (which must be implemented as immutable) and it is
            ///     also internally cached to avoid a lot of unnecessary loads and disposals
            ///     of a given resource when being used/unused.
            ///   </para>
            /// </summary>
            public class IdentifiedSpriteGrid<T> : SpriteGrid
            {
                /// <summary>
                ///   The key this sprite grid is associated to.
                /// </summary>
                public readonly T Key;

                /// <summary>
                ///   The pool this sprite grid belongs to.
                /// </summary>
                public readonly IdentifiedSpriteGridPool<T> Pool;

                internal IdentifiedSpriteGrid(
                    IdentifiedSpriteGridPool<T> pool, T key, Texture2D texture, uint frameWidth,
                    uint frameHeight, float pixelsPerUnit, Action onFinalized = null
                ) : base(texture, frameWidth, frameHeight, pixelsPerUnit, onFinalized)
                {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    Key = key;
                    Pool = pool;
                }
                
                /// <summary>
                ///   Invoked when this sprite grid is "used".
                /// </summary>
                protected override void OnUsed()
                {
                    Pool.OnUsed(this);
                }

                /// <summary>
                ///   Invoked when this sprite grid is "released".
                /// </summary>
                protected override void OnReleased()
                {
                    Pool.OnReleased(this);
                }
            }
        }
    }
}
