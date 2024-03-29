using System;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Types
    {
        /// <summary>
        ///   <para>
        ///     A SpriteGrid references a texture and also a set of related values to tell
        ///     how many frames it will have and how to render them.
        ///   </para>
        ///   <para>
        ///     There are two possible use cases, at least, for a SpriteGrid: The first one
        ///     is when it is loaded from a given texture directly. The second one is when
        ///     the texture this SpriteGrid comes from is instead composed from a dynamic
        ///     process instead.
        ///   </para>
        /// </summary>
        public abstract class SpriteGrid
        {
            /// <summary>
            ///   The width of each frame. Strictly greater than 0.
            /// </summary>
            public readonly uint FrameWidth;
            
            /// <summary>
            ///   The width of each frame. Strictly greater than 0.
            /// </summary>
            public readonly uint FrameHeight;

            /// <summary>
            ///   The width of the padding. Might be 0.
            /// </summary>
            public readonly uint PaddingWidth;
            
            /// <summary>
            ///   The height of the padding. Might be 0.
            /// </summary>
            public readonly uint PaddingHeight;

            /// <summary>
            ///   The pixels per unit for each sprite. Strictly greater than 0.
            /// </summary>
            public readonly float PixelsPerUnit;
            
            /// <summary>
            ///   The underlying 2D texture for this grid.
            /// </summary>
            public Texture2D Texture;

            // The rect to apply to this texture.
            private Rect _subSubRect;

            /// <summary>
            ///   The rect to apply to this texture.
            /// </summary>
            public Rect SubRect => _subSubRect;

            /// <summary>
            ///   The number of columns in the grid.
            /// </summary>
            public readonly uint FrameColumns;
            
            /// <summary>
            ///   The number of rows in the grid.
            /// </summary>
            public readonly uint FrameRows;

            // Tracks the sprites.
            private Sprite[] sprites;
            
            // What to do when this sprite grid is
            // finalized/destroyed.
            private Action onGarbageCollected;
            
            /// <summary>
            ///   Given a texture, and a setting for dimensions and PPU, it slices the grid.
            ///   The slicing starts at (0, 0), ends at the full size of the texture, requires
            ///   the texture to be of matching sizes (integer multiples of the frame dimensions)
            ///   and generates the slicing on demand.
            /// </summary>
            /// <param name="texture">The texture to slice</param>
            /// <param name="rect">An optional rect to use</param>
            /// <param name="frameWidth">The frame width</param>
            /// <param name="frameHeight">The frame height</param>
            /// <param name="paddingWidth">The padding width</param>
            /// <param name="paddingHeight">The padding height</param>
            /// <param name="pixelsPerUnit">The pixels per unit</param>
            /// <param name="onFinalized">What do do when this sprite grid is released</param>
            /// <exception cref="ArgumentNullException">The texture is null</exception>
            /// <exception cref="ArgumentException">The dimensions are inconsistent</exception>
            public SpriteGrid(
                Texture2D texture, Rect? rect, uint frameWidth, uint frameHeight,
                uint paddingWidth, uint paddingHeight, float pixelsPerUnit,
                Action onInitialized, Action onFinalized
            ) {
                if (texture == null) throw new ArgumentNullException(nameof(texture));
                if (rect == null) rect = new Rect(0, 0, texture.width, texture.height);
                if (rect.Value.x + rect.Value.width > texture.width ||
                    rect.Value.y + rect.Value.height > texture.height ||
                    rect.Value.x < 0 || rect.Value.y < 0 ||
                    rect.Value.width < 0 || rect.Value.height < 0)
                {
                    throw new ArgumentException(
                        "Invalid rect used to create a sprite grid. Rects must have " +
                        "strictly positive sizes, non-negative position, and the right/top " +
                        "boundaries must not pass texture's ones"
                    );
                }
                if (frameWidth == 0) throw new ArgumentException("The frame width cannot be 0");
                if (frameHeight == 0) throw new ArgumentException("The frame height cannot be 0");
                if (pixelsPerUnit <= 0) throw new ArgumentException("The pixels per unit must be positive");
                if (texture.width % (frameWidth + paddingWidth) != 0) throw new ArgumentException(
                    "The frame width must be an exact divider of the texture's width"
                );
                if (texture.height % (frameHeight + paddingHeight) != 0) throw new ArgumentException(
                    "The frame height must be an exact divider of the texture's height"
                );

                FrameWidth = frameWidth;
                FrameHeight = frameHeight;
                PaddingWidth = paddingWidth;
                PaddingHeight = paddingHeight;
                PixelsPerUnit = pixelsPerUnit;
                Texture = texture;
                FrameColumns = (uint)(rect.Value.width / frameWidth);
                FrameRows = (uint)(rect.Value.height / frameHeight);
                sprites = new Sprite[FrameColumns * FrameRows];
                onGarbageCollected = onFinalized;
                _subSubRect = rect.Value;
                
                if (onInitialized == null)
                {
                    Debug.LogWarning(
                        $"The given texture: {texture} will not be notified when " +
                        $"this object is initialized. Nobody is being notified right now when " +
                        $"this object (& texture) is just used by this sprite grid object. It " +
                        $"is recommended to always use a kind of initializer, that accounts " +
                        $"for the texture being used once more, so the texture can later be " +
                        $"accounted for destruction on the finalizer"
                    );
                }
                else
                {
                    onInitialized();
                }
                    
                if (onGarbageCollected == null)
                {
                    Debug.LogWarning(
                        $"The given texture: {texture} will not be notified when " +
                        $"this object is destroyed. You will have no mean of knowing when " +
                        $"this texture is no longer used by sprite grid objects being garbage " +
                        $"collected (destroyed, finalized). Add a finalizer function that " +
                        $"attempts to destroy the object (and catches when the object is, " +
                        $"instead, an asset and cannot be destroyed)"
                    );
                }
            }

            ~SpriteGrid()
            {
                onGarbageCollected?.Invoke();
            }

            /// <summary>
            ///   Gets a sprite at given column and row. The first call at certain coordinates,
            ///   if valid, instantiates the corresponding sprite.
            /// </summary>
            /// <param name="row">The row. Smaller than <see cref="FrameRows"/></param>
            /// <param name="column">The column. Smaller than <see cref="FrameColumns"/></param>
            /// <returns>A sprite at given coordinates</returns>
            public Sprite Get(uint row, uint column)
            {
                return Get(row, column, Vector2.zero);
            }
            
            /// <summary>
            ///   Gets a sprite at given column and row. The first call at certain coordinates,
            ///   if valid, instantiates the corresponding sprite. It also allows a custom
            ///   pivot to be used.
            /// </summary>
            /// <param name="row">The row. Smaller than <see cref="FrameRows"/></param>
            /// <param name="column">The column. Smaller than <see cref="FrameColumns"/></param>
            /// <param name="pivot">The pivot. Each dimension should be in [0, 1]</param>
            /// <returns>A sprite at given coordinates</returns>
            public Sprite Get(uint row, uint column, Vector2 pivot)
            {
                if (sprites[row * FrameColumns + column] == null)
                {
                    Rect rect = new Rect(
                        (FrameWidth + PaddingWidth) * column + _subSubRect.x, 
                        _subSubRect.height - (FrameHeight + PaddingHeight) * (row + 1) + _subSubRect.y,
                        FrameWidth, FrameHeight
                    );
                    sprites[row * FrameColumns + column] = Sprite.Create(Texture, rect, pivot, PixelsPerUnit);
                }
                return sprites[row * FrameColumns + column];
            }
            
            /// <summary>
            ///   Invoked when this sprite grid is "used". Invokes <see cref="OnUsed"/>.
            /// </summary>
            internal void Used() { OnUsed(); }
            
            /// <summary>
            ///   Invoked when this sprite grid is "released". Invokes <see cref="OnReleased"/>.
            /// </summary>
            internal void Released() { OnReleased(); }

            /// <summary>
            ///   Invoked when this sprite grid is "used".
            /// </summary>
            protected abstract void OnUsed();

            /// <summary>
            ///   Invoked when this sprite grid is "released".
            /// </summary>
            protected abstract void OnReleased();
        }
    }
}
