using System;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Samples
    {
        [RequireComponent(typeof(SpriteRenderer))]
        public class SampleSpriteGridApplier : SpriteGridApplier
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
                if (CurrentSpriteGrid != null)
                {
                    spriteRenderer.sprite = CurrentSpriteGrid.Get((uint)index / 2, (uint)index % 2);
                }
            }

            protected override bool IsCompatible(SpriteGrid sg)
            {
                return sg.FrameWidth == 32 && sg.FrameHeight == 32 && sg.FrameColumns == 2 && sg.FrameRows == 2;
            }

            protected override void BeforeUse(SpriteGrid sg)
            {
                if (sg.FrameWidth != 32 || sg.FrameHeight != 32) throw new ArgumentException(
                    "Dimensions of a valid SpriteGrid for this applier must be 32x32"
                );
                if (sg.FrameRows != 2 || sg.FrameColumns != 2) throw new ArgumentException(
                    "Grid layout of a valid SpriteGrid for this applier must be 2x2"
                );
            }

            protected override void AfterUse(SpriteGrid sg)
            {
                SetCurrentSprite();
            }

            protected override void AfterRelease(SpriteGrid sg)
            {
                spriteRenderer.sprite = null;
            }
        }
    }
}