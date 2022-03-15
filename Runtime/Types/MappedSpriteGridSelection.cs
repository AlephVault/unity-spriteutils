using System;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Types
    {
        /// <summary>
        ///   A sprite grid selection which has means to map appropriately
        ///   on construction to the intended selection data. The mapping
        ///   also validates the selection accordingly.
        /// </summary>
        /// <typeparam name="SelectionResult">The selection result type</typeparam>
        public abstract class MappedSpriteGridSelection<SelectionSpec, SelectionResult>
            : SpriteGridSelection<SelectionResult>
        {
            /// <summary>
            ///   The grid this selection comes from.
            /// </summary>
            public readonly SpriteGrid SourceGrid;

            // The spec.
            private SelectionSpec spec;

            // The cached result.
            private SelectionResult result;

            // Whether it must validate and map the spec or not.
            // This stands for the first attempt of retrieving
            // the results.
            private bool mustValidateAndMap = true;

            /// <summary>
            ///   Builds a selection by expecting a source grid.
            /// </summary>
            /// <param name="sourceGrid">The grid to select from</param>
            /// <param name="selection">The selection data to use</param>
            /// <exception cref="ArgumentNullException">The grid is null</exception>
            public MappedSpriteGridSelection(SpriteGrid sourceGrid, SelectionSpec selection) : base(sourceGrid)
            {
                SourceGrid = sourceGrid ?? throw new ArgumentNullException(nameof(sourceGrid));
                spec = selection;
            }

            /// <summary>
            ///   Validates the selection spec against the source grid,
            ///   and then transforms & assigns it accordingly.
            /// </summary>
            /// <param name="sourceGrid">The grid to select from</param>
            /// <param name="selection">The selection data to use</param>
            /// <returns></returns>
            protected abstract SelectionResult ValidateAndMap(SpriteGrid sourceGrid, SelectionSpec selection);

            /// <summary>
            ///   Returns the expected selection. This value should
            ///   be read-only in the selection and use only types
            ///   that are also read-only. Ideally, the "leaves" of
            ///   this value are <see cref="Sprite"/> objects.
            /// </summary>
            /// <returns>The sprite(s) selection</returns>
            public override SelectionResult GetSelection()
            {
                if (mustValidateAndMap)
                {
                    result = ValidateAndMap(SourceGrid, spec);
                    mustValidateAndMap = false;
                }
                return result;
            }
        }
    }
}