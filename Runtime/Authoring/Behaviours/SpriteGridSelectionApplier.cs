using AlephVault.Unity.SpriteUtils.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Authoring
    {
        namespace Types
        {
            /// <summary>
            ///   Sprite grid users will make use of a selection (even
            ///   triggering their internal cache system) into them, but
            ///   according to their own logic and even filtering out
            ///   the invalid sprite grids (i.e. unsuitable for them).
            /// </summary>
            public abstract class SpriteGridSelectionApplier<SelectionResult> : MonoBehaviour
            {
                /// <summary>
                ///   The current sprite grid.
                /// </summary>
                public SpriteGridSelection<SelectionResult> CurrentSelection { get; private set; }

                /// <summary>
                ///   Tells whether this applier can use a given selection.
                ///   It returns <c>true</c> on <c>null</c> values. The actual
                ///   implementation is inside <see cref="IsCompatible"/>.
                /// </summary>
                /// <param name="selection">The sprite grid selection to test</param>
                public bool CanUse(SpriteGridSelection<SelectionResult> selection)
                {
                    if (selection == null) return true;
                    return IsCompatible(selection);
                }

                /// <summary>
                ///   Tests whether the given sprite grid selection is compatible
                ///   with this applier.
                /// </summary>
                /// <param name="selection">The selection to test</param>
                protected virtual bool IsCompatible(SpriteGridSelection<SelectionResult> selection)
                {
                    return true;
                }
                
                /// <summary>
                ///   Performs logic before having the current selection
                ///   replaced by a new one. It is encouraged to raise any
                ///   needed exception if the given sprite grid is not meant
                ///   to be accepted by this applier, but this has to be done
                ///   always in sync with <see cref="IsCompatible"/>.
                /// </summary>
                /// <param name="selection">The new selection</param>
                protected virtual void BeforeUse(SpriteGridSelection<SelectionResult> selection) {}

                /// <summary>
                ///   Performs logic after having the previous selection
                ///   replaced by the current one.
                /// </summary>
                /// <param name="selection">The new selection</param>
                protected abstract void AfterUse(SpriteGridSelection<SelectionResult> selection);

                /// <summary>
                ///   Performs logic before having the current selection
                ///   released.
                /// </summary>
                /// <param name="selection">The current selection</param>
                protected virtual void BeforeRelease(SpriteGridSelection<SelectionResult> selection) {}

                /// <summary>
                ///   Performs logic after having the current selection
                ///   released. Ideally, no reference(s) should be kept
                ///   to that released sprite grid.
                /// </summary>
                /// <param name="selection">The just-removed selection</param>
                protected abstract void AfterRelease(SpriteGridSelection<SelectionResult> selection);

                /// <summary>
                ///   Uses a selection, releasing the previous one.
                /// </summary>
                /// <param name="selection">The selection to use</param>
                public void UseSelection(SpriteGridSelection<SelectionResult> selection)
                {
                    if (CurrentSelection == selection) return;
                    
                    ReleaseSelection();                        
                    
                    if (selection != null)
                    {
                        BeforeUse(selection);
                        CurrentSelection = selection;
                        selection.Used();
                        AfterUse(selection);
                    }
                }

                /// <summary>
                ///   Releases the current selection, if any.
                /// </summary>
                public void ReleaseSelection()
                {
                    if (CurrentSelection != null)
                    {
                        BeforeRelease(CurrentSelection);
                        SpriteGridSelection<SelectionResult> selection = CurrentSelection;
                        CurrentSelection = null;
                        selection.Released();
                        AfterRelease(selection);
                    }
                }
            }
        }
    }
}
