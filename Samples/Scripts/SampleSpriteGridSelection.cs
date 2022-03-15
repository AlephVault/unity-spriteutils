using System;
using AlephVault.Unity.SpriteUtils.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Samples
    {
        /// <summary>
        ///   A sample grid selection
        /// </summary>
        public class SampleSpriteGridSelection : MappedSpriteGridSelection<Vector2Int, Sprite>
        {
            public SampleSpriteGridSelection(SpriteGrid sourceGrid, Vector2Int spec) : base(sourceGrid, spec)
            {
            }

            protected override Sprite ValidateAndMap(SpriteGrid sourceGrid, Vector2Int selection)
            {
                return ValidateAndMapSprite(sourceGrid, selection);
            }
        }
    }
}