using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Authoring
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
                ///   The pixels per unit for each sprite. Strictly greater than 0.
                /// </summary>
                public readonly float PixelsPerUnit;

                /// <summary>
                ///   The underlying 2D texture for this grid.
                /// </summary>
                public Texture2D Texture;

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
                
                /// <summary>
                ///   Given a texture, and a setting for dimensions and PPU, it slices the grid.
                ///   The slicing starts at (0, 0), ends at the full size of the texture, requires
                ///   the texture to be of matching sizes (integer multiples of the frame dimensions)
                ///   and generates the slicing on demand.
                /// </summary>
                /// <param name="texture">The texture to slice</param>
                /// <param name="frameWidth">The frame width</param>
                /// <param name="frameHeight">The frame height</param>
                /// <param name="pixelsPerUnit">The pixels per unit</param>
                /// <exception cref="ArgumentNullException">The texture is null</exception>
                /// <exception cref="ArgumentException">The dimensions are inconsistent</exception>
                public SpriteGrid(Texture2D texture, uint frameWidth, uint frameHeight, float pixelsPerUnit)
                {
                    if (texture == null) throw new ArgumentNullException(nameof(texture));
                    if (frameWidth == 0) throw new ArgumentException("The frame width cannot be 0");
                    if (frameHeight == 0) throw new ArgumentException("The frame height cannot be 0");
                    if (pixelsPerUnit <= 0) throw new ArgumentException("The pixels per unit must be positive");
                    if (texture.width % frameWidth != 0) throw new ArgumentException(
                        "The frame width must be an exact divider of the texture's width"
                    );
                    if (texture.height % frameHeight != 0) throw new ArgumentException(
                        "The frame height must be an exact divider of the texture's height"
                    );

                    FrameWidth = frameWidth;
                    FrameHeight = frameHeight;
                    PixelsPerUnit = pixelsPerUnit;
                    Texture = texture;
                    FrameColumns = (uint)(texture.width / frameWidth);
                    FrameRows = (uint)(texture.height / frameHeight);
                    sprites = new Sprite[FrameColumns * FrameRows];
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
                    if (sprites[row * FrameColumns + column] == null)
                    {
                        Rect rect = new Rect(
                            FrameWidth * column, Texture.height - FrameHeight * (row + 1), FrameWidth, FrameHeight
                        );
                        sprites[row * FrameColumns + column] = Sprite.Create(Texture, rect, Vector2.zero, PixelsPerUnit);
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
}
