using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Authoring
    {
        namespace Types
        {
            /// <summary>
            ///   Sprite grid users will make use of a sprite grid (even
            ///   triggering their internal cache system) into them, but
            ///   according to their own logic and even filtering out
            ///   the invalid sprite grids (i.e. unsuitable for them).
            /// </summary>
            public abstract class SpriteGridApplier : MonoBehaviour
            {
                /// <summary>
                ///   The current sprite grid.
                /// </summary>
                public SpriteGrid CurrentSpriteGrid { get; private set; }

                /// <summary>
                ///   Tells whether this applier can use a given sprite grid.
                ///   It returns <c>true</c> on <c>null</c> values. The actual
                ///   implementation is inside <see cref="IsCompatible"/>.
                /// </summary>
                /// <param name="sg">The sprite grid to test</param>
                public bool CanUse(SpriteGrid sg)
                {
                    if (sg == null) return true;
                    return IsCompatible(sg);
                }

                /// <summary>
                ///   Tests whether the given sprite grid instance is compatible
                ///   with this applier.
                /// </summary>
                /// <param name="sg">The sprite grid to test</param>
                protected abstract bool IsCompatible(SpriteGrid sg);
                
                /// <summary>
                ///   Performs logic before having the current sprite grid
                ///   replaced by a new one. It is encouraged to raise any
                ///   needed exception if the given sprite grid is not meant
                ///   to be accepted by this applier, but this has to be done
                ///   always in sync with <see cref="IsCompatible"/>.
                /// </summary>
                /// <param name="sg">The new sprite grid</param>
                protected virtual void BeforeUse(SpriteGrid sg) {}

                /// <summary>
                ///   Performs logic after having the previous sprite grid
                ///   replaced by the current one.
                /// </summary>
                /// <param name="sg">The new sprite grid</param>
                protected abstract void AfterUse(SpriteGrid sg);

                /// <summary>
                ///   Performs logic before having the current sprite grid
                ///   released.
                /// </summary>
                /// <param name="sg">The current sprite grid</param>
                protected virtual void BeforeRelease(SpriteGrid sg) {}

                /// <summary>
                ///   Performs logic after having the current sprite grid
                ///   released. Ideally, no reference(s) should be kept to
                ///   that released sprite grid.
                /// </summary>
                /// <param name="sg">The just-removed sprite grid</param>
                protected abstract void AfterRelease(SpriteGrid sg);

                /// <summary>
                ///   Uses a sprite grid, releasing the previous one.
                /// </summary>
                /// <param name="sg">The sprite grid to use</param>
                public void UseSpriteGrid(SpriteGrid sg)
                {
                    if (CurrentSpriteGrid == sg) return;
                    
                    ReleaseSpriteGrid();                        
                    
                    if (sg != null)
                    {
                        BeforeUse(sg);
                        CurrentSpriteGrid = sg;
                        sg.Used();
                        AfterUse(sg);
                    }
                }

                /// <summary>
                ///   Releases the current sprite grid, if any.
                /// </summary>
                public void ReleaseSpriteGrid()
                {
                    if (CurrentSpriteGrid != null)
                    {
                        BeforeRelease(CurrentSpriteGrid);
                        SpriteGrid sg = CurrentSpriteGrid;
                        CurrentSpriteGrid = null;
                        sg.Released();
                        AfterRelease(sg);
                    }
                }
            }
        }
    }
}
