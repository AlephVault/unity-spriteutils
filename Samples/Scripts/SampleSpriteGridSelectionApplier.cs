using System;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using AlephVault.Unity.SpriteUtils.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Samples
    {
        [RequireComponent(typeof(SpriteRenderer))]
        public class SampleSpriteGridSelectionApplier : SpriteGridSelectionApplier<Sprite>
        {
            private int index = 0;
            private float timer = 0;
            private SpriteRenderer spriteRenderer;

            private void Awake()
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            private void Update()
            {
                timer += Time.deltaTime;
                if (timer > 1f)
                {
                    timer -= 1f;
                    index += 1;
                    if (index == 4) index = 0;
                    SetCurrentSprite();
                }
            }

            private void SetCurrentSprite()
            {
                if (CurrentSelection != null)
                {
                    spriteRenderer.sprite = CurrentSelection.GetSelection();
                }
            }

            protected override bool IsCompatible(SpriteGridSelection<Sprite> selection)
            {
                SpriteGrid sg = selection.SourceGrid;
                return sg.FrameWidth == 32 && sg.FrameHeight == 32 && sg.FrameColumns == 2 && sg.FrameRows == 2;
            }

            protected override void BeforeUse(SpriteGridSelection<Sprite> selection)
            {
                SpriteGrid sg = selection.SourceGrid;
                if (sg.FrameWidth != 32 || sg.FrameHeight != 32) throw new ArgumentException(
                    "Dimensions of a valid SpriteGrid for this applier must be 32x32"
                );
                if (sg.FrameRows != 2 || sg.FrameColumns != 2) throw new ArgumentException(
                    "Grid layout of a valid SpriteGrid for this applier must be 2x2"
                );
            }

            protected override void AfterUse(SpriteGridSelection<Sprite> sg)
            {
                SetCurrentSprite();
            }

            protected override void AfterRelease(SpriteGridSelection<Sprite> sg)
            {
                spriteRenderer.sprite = null;
            }
        }
    }
}