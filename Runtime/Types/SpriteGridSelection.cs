using System;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Types
    {
        /// <summary>
        ///   A sprite grid selection wraps an existing <see cref="SpriteGrid"/>
        ///   and chooses only certain elements from it. Each selection provides
        ///   a different structure, depending on its type, and serves a different
        ///   purpose (e.g. to different appliers, expecting a particular selection
        ///   result type).
        /// </summary>
        /// <typeparam name="SelectionResult">The selection result type</typeparam>
        public abstract class SpriteGridSelection<SelectionResult>
        {
            /// <summary>
            ///   The grid this selection comes from.
            /// </summary>
            public readonly SpriteGrid SourceGrid;

            /// <summary>
            ///   Builds a selection by expecting a source grid.
            /// </summary>
            /// <param name="sourceGrid">The grid to select from</param>
            /// <exception cref="ArgumentNullException">The grid is null</exception>
            public SpriteGridSelection(SpriteGrid sourceGrid)
            {
                SourceGrid = sourceGrid ?? throw new ArgumentNullException(nameof(sourceGrid));
            }

            /// <summary>
            ///   Returns the expected selection. This value should
            ///   be read-only in the selection and use only types
            ///   that are also read-only. Ideally, the "leaves" of
            ///   this value are <see cref="Sprite"/> objects.
            /// </summary>
            /// <returns>The sprite(s) selection</returns>
            public abstract SelectionResult GetSelection();

            /// <summary>
            ///   Delegates the notification to the <see cref="SourceGrid"/>.
            /// </summary>
            internal void Used()
            {
                SourceGrid.Used();
            }

            /// <summary>
            ///   Delegates the notification to the <see cref="SourceGrid"/>.
            /// </summary>
            internal void Released()
            {
                SourceGrid.Released();
            }
        }
    }
}