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
                if (selection.x < 0 || selection.y < 0 || selection.x >= sourceGrid.FrameColumns ||
                    selection.y >= sourceGrid.FrameRows)
                {
                    throw new ArgumentException($"Invalid selection {selection} for a sprite grid of " +
                                                $"dimension: ({sourceGrid.FrameColumns}, {sourceGrid.FrameRows})");
                }

                return sourceGrid.Get((uint)selection.y, (uint)selection.x);
            }
        }
    }
}