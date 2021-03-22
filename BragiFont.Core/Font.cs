﻿using BragiFont.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BragiFont
{

    public abstract class Font : IEquatable<Font>, IDisposable
    {

        /// <summary>
        /// Caches of glyphs
        /// </summary>
        private readonly List<GlyphCache> _glyphCaches = new List<GlyphCache>();

        /// <summary>
        /// The characters that we currently have generated glyphs for
        /// </summary>
        internal Dictionary<char, Internal.Glyph> CharacterGlyphs = new Dictionary<char, Internal.Glyph>();

        /// <summary>
        /// The text cache
        /// </summary>
        internal Cache<string, Text> TextCache = new Cache<string, Text>(Constants.Settings.MaxTextCacheSize);

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="size">Size of the font.</param>
        /// <param name="face">The font face.</param>
        /// <param name="typefaceName">The typefaceName.</param>
        protected Font(int size, Face face, string typefaceName)
        {
            Size = size;
            TypefaceName = typefaceName;
            Face = face;
            FontFamily = face.FamilyName;

            GlyphHeight = face.Size.Metrics.Height.Ceiling();
        }

        /// <summary>
        /// Gets the font face.
        /// </summary>
        /// <value>
        /// The face.
        /// </value>
        public Face Face { get; }

        /// <summary>
        /// Gets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public string FontFamily { get; }

        /// <summary>
        /// Gets the height of glyphs in the Font.
        /// </summary>
        /// <value>
        /// The height of the glyph.
        /// </value>
        public int GlyphHeight { get; }

        /// <summary>
        /// Gets the key representing the font.
        /// </summary>
        /// <value>
        /// The font's key.
        /// </value>
        public Tuple<string, int> Key => new Tuple<string, int>(FontFamily, Size);

        /// <summary>
        /// Gets or sets the load flags.
        /// </summary>
        /// <value>
        /// The load flags.
        /// </value>
        public LoadFlags LoadFlags { get; set; } = Constants.Settings.DefaultLoadFlags;

        /// <summary>
        /// Gets or sets the load target.
        /// </summary>
        /// <value>
        /// The load target.
        /// </value>
        public LoadTarget LoadTarget { get; set; } = Constants.Settings.DefaultLoadTarget;

        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        public RenderMode RenderMode { get; set; } = Constants.Settings.DefaultRenderMode;

        /// <summary>
        /// Gets the size of the Font.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size { get; }

        /// <summary>
        /// Gets or sets the number of spaces we'll use when a tab is requested.
        /// </summary>
        /// <value>
        /// The spaces in tab.
        /// </value>
        public int SpacesInTab { get; set; } = Constants.Settings.DefaultSpacesInTab;

        /// <summary>
        /// Gets the typeface the font is associated with.
        /// </summary>
        /// <value>
        /// The typeface.
        /// </value>
        public Typeface Typeface => Bragi.Core.GetStoredTypeface(TypefaceName);

        /// <summary>
        /// Gets the name of the typeface the font is associated with.
        /// </summary>
        /// <value>
        /// The typeface name.
        /// </value>
        public string TypefaceName { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Face.Dispose();
            Typeface.RemoveFont(Size);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void DisposeFinal()
        {
            Face.Dispose();
        }

        /// <summary>
        /// Draws the text to the screen at the specified position and with the specified color.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="boundaries">The boundaries.</param>
        public void Draw(SpriteBatch spriteBatch, string text, Color color, Rectangle boundaries)
        {
            var warpLine = boundaries.Width > 0;
            var offsetX = 0;
            var offsetY = 0;

            var width = warpLine ? boundaries.Width : spriteBatch.GraphicsDevice.Viewport.Width;
            var height = (boundaries.Height > 0 ? boundaries.Height : spriteBatch.GraphicsDevice.Viewport.Height) - boundaries.Y;

            var countX = 0;
            var underrun = 0;
            var finalCharacterIndex = text.Length - 1;

            for (var i = 0; i < text.Length; i++)
            {
                TryGetGlyph(text[i], out var cachedCharacter);

                if (warpLine && offsetX + cachedCharacter.Boundary.Width + countX > width || text[i] == '\n')
                {
                    offsetX = 0;
                    underrun = 0;
                    offsetY += cachedCharacter.AdvanceY;
                }

                if (text[i] == '\r' || text[i] == '\n')
                {
                    continue;
                }

                if (offsetY > height || !warpLine && offsetX > width)
                {
                    return;
                }

                // calculate underrun
                underrun += -cachedCharacter.BearingX;
                if (offsetX == 0)
                {
                    offsetX += underrun;
                }

                if (underrun <= 0)
                {
                    underrun = 0;
                }

                spriteBatch.Draw(cachedCharacter.GlyphCache.Texture, new Vector2(boundaries.X + offsetX, boundaries.Y + offsetY), cachedCharacter.Boundary, color);
                offsetX += cachedCharacter.Boundary.Width;

                // calculate kerning
                if (i != finalCharacterIndex)
                {
                    var nextCharacter = text[i + 1];
                    if (TryGetGlyph(nextCharacter, out var nextCachedCharacter))
                    {
                        var kerning = GetKerning(cachedCharacter, nextCachedCharacter);
                        var maxBounds = cachedCharacter.AdvanceX * Constants.Settings.KerningSanityMultiplier;
                        if (kerning <= maxBounds && kerning >= -maxBounds)
                        {
                            offsetX += kerning;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, string text, Color color, Rectangle boundaries, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            // calculate our transformation matrix
            var flipAdjustment = Vector2.Zero;
            var flippedVertically = effects.HasFlag(SpriteEffects.FlipVertically);
            var flippedHorizontally = effects.HasFlag(SpriteEffects.FlipHorizontally);

            // if we've flipped, handle adjusting our location as required
            if (flippedVertically || flippedHorizontally)
            {
                var size = MeasureText(text);

                if (flippedHorizontally)
                {
                    origin.X *= -1;
                    flipAdjustment.X -= size.X;
                }

                if (flippedVertically)
                {
                    origin.Y *= -1;
                    flipAdjustment.Y = GlyphHeight - size.Y;
                }
            }

            // Handle our rotation as required
            var transformation = Matrix.Identity;
            float cos, sin = 0;
            var xScale = flippedHorizontally ? -scale.X : scale.X;
            var yScale = flippedVertically ? -scale.Y : scale.Y;
            var xOrigin = flipAdjustment.X - origin.X;
            var yOrigin = flipAdjustment.Y - origin.Y;
            if (Helpers.FloatsAreEqual(rotation, 0) || Helpers.FloatsAreEqual(rotation / Constants.TWO_PI, 1))
            {
                transformation.M11 = xScale;
                transformation.M22 = yScale;
                transformation.M41 = xOrigin * transformation.M11 + boundaries.X;
                transformation.M42 = yOrigin * transformation.M22 + boundaries.Y;
            }
            else
            {
                cos = (float)Math.Cos(rotation);
                sin = (float)Math.Sin(rotation);
                transformation.M11 = xScale * cos;
                transformation.M12 = xScale * sin;
                transformation.M21 = yScale * -sin;
                transformation.M22 = yScale * cos;
                transformation.M41 = (xOrigin * transformation.M11 + yOrigin * transformation.M21) + boundaries.X;
                transformation.M42 = (xOrigin * transformation.M12 + yOrigin * transformation.M22) + boundaries.Y;
            }

            // calculate the rest of the text position
            var warpLine = boundaries.Width > 0;
            var offsetX = 0;
            var offsetY = 0;

            var width = warpLine ? boundaries.Width : spriteBatch.GraphicsDevice.Viewport.Width;
            var height = (boundaries.Height > 0 ? boundaries.Height : spriteBatch.GraphicsDevice.Viewport.Height) - boundaries.Y;

            var countX = 0;
            var underrun = 0;
            var finalCharacterIndex = text.Length - 1;

            for (var i = 0; i < text.Length; i++)
            {
                TryGetGlyph(text[i], out var cachedCharacter);

                if (warpLine && offsetX + cachedCharacter.Boundary.Width + countX > width || text[i] == '\n')
                {
                    offsetX = 0;
                    underrun = 0;
                    offsetY += cachedCharacter.AdvanceY;
                }

                if (text[i] == '\r' || text[i] == '\n')
                {
                    continue;
                }

                if (offsetY > height || !warpLine && offsetX > width)
                {
                    return;
                }

                // calculate underrun
                underrun += -cachedCharacter.BearingX;
                if (offsetX == 0)
                {
                    offsetX += underrun;
                }

                if (underrun <= 0)
                {
                    underrun = 0;
                }

                var characterPosition = new Vector2(boundaries.X + offsetX, boundaries.Y + offsetY);
                Vector2.Transform(ref characterPosition, ref transformation, out characterPosition);
                spriteBatch.Draw(cachedCharacter.GlyphCache.Texture, characterPosition, cachedCharacter.Boundary, color, rotation, origin, scale, effects, layerDepth);
                offsetX += cachedCharacter.Boundary.Width;

                // calculate kerning
                if (i != finalCharacterIndex)
                {
                    var nextCharacter = text[i + 1];
                    if (TryGetGlyph(nextCharacter, out var nextCachedCharacter))
                    {
                        var kerning = GetKerning(cachedCharacter, nextCachedCharacter);
                        var maxBounds = cachedCharacter.AdvanceX * Constants.Settings.KerningSanityMultiplier;
                        if (kerning <= maxBounds && kerning >= -maxBounds)
                        {
                            offsetX += kerning;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pre-generates a list of Glyphs to draw to the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A Text object representing the Glyphs we need to draw to the screen for the input string.</returns>
        public Text MakeText(StringBuilder text)
        {
            return MakeText(text.ToString());
        }

        /// <summary>
        /// Pre-generates a list of Glyphs to draw to the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A Text object representing the Glyphs we need to draw to the screen for the input string.</returns>
        public Text MakeText(string text)
        {
            Text textResult;
            if (!TextCache.TryGetItem(text, out textResult))
            {
                var finalSize = new Vector2(0, 0);

                textResult = new TextImplementation(text, this);

                var offsetX = 0;
                var offsetY = 0;

                var underrun = 0;
                var finalCharacterIndex = text.Length - 1;

                for (var i = 0; i < text.Length; i++)
                {
                    TryGetGlyph(text[i], out var cachedCharacter);
                    if (i == 0)
                    {
                        finalSize.Y += cachedCharacter.Boundary.Height;
                    }

                    if (text[i] == '\n')
                    {
                        finalSize.X = Math.Max(offsetX, finalSize.X);
                        offsetX = 0;
                        underrun = 0;
                        offsetY += cachedCharacter.AdvanceY;
                        if (i != finalCharacterIndex)
                        {
                            finalSize.Y += cachedCharacter.AdvanceY;
                        }
                    }

                    if (text[i] == '\r' || text[i] == '\n')
                    {
                        continue;
                    }

                    // calculate underrun
                    underrun += -cachedCharacter.BearingX;
                    if (offsetX == 0)
                    {
                        offsetX += underrun;
                    }

                    if (underrun <= 0)
                    {
                        underrun = 0;
                    }

                    textResult.AddCharacter(new TextCharacter(cachedCharacter, new Vector2(offsetX, offsetY)));
                    offsetX += cachedCharacter.Boundary.Width;

                    // calculate kerning
                    if (i != finalCharacterIndex)
                    {
                        var nextCharacter = text[i + 1];
                        if (TryGetGlyph(nextCharacter, out var nextCachedCharacter))
                        {
                            var kerning = GetKerning(cachedCharacter, nextCachedCharacter);
                            var maxBounds = cachedCharacter.AdvanceX * Constants.Settings.KerningSanityMultiplier;
                            if (kerning <= maxBounds && kerning >= -maxBounds)
                            {
                                offsetX += kerning;
                            }
                        }
                    }
                    else
                    {
                        finalSize.X = Math.Max(offsetX, finalSize.X);
                    }
                }

                textResult.Width = finalSize.X;
                textResult.Height = finalSize.Y;
                textResult.Size = new Vector2(textResult.Width, textResult.Height);

                TextCache.AddItemToCache(text, textResult);
            }

            return new TextImplementation(textResult);
        }

        /// <summary>
        /// Measures the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The size of the text.</returns>
        public Vector2 MeasureText(StringBuilder text)
        {
            return MeasureText(text.ToString());
        }

        /// <summary>
        /// Measures the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The size of the text.</returns>
        public Vector2 MeasureText(string text)
        {
            var finalSize = new Vector2(0, 0);

            var offsetX = 0;

            var underrun = 0;
            var finalCharacterIndex = text.Length - 1;

            for (var i = 0; i < text.Length; i++)
            {
                TryGetGlyph(text[i], out var cachedCharacter);
                if (i == 0)
                {
                    finalSize.Y += cachedCharacter.Boundary.Height;
                }

                if (text[i] == '\n')
                {
                    finalSize.X = Math.Max(offsetX, finalSize.X);
                    offsetX = 0;
                    underrun = 0;
                    if (i != finalCharacterIndex)
                    {
                        finalSize.Y += cachedCharacter.AdvanceY;
                    }
                }

                if (text[i] == '\r' || text[i] == '\n')
                {
                    continue;
                }

                // calculate underrun
                underrun += -cachedCharacter.BearingX;
                if (offsetX == 0)
                {
                    offsetX += underrun;
                }

                if (underrun <= 0)
                {
                    underrun = 0;
                }

                offsetX += cachedCharacter.Boundary.Width;

                // calculate kerning
                if (i != finalCharacterIndex)
                {
                    var nextCharacter = text[i + 1];
                    if (TryGetGlyph(nextCharacter, out var nextCachedCharacter))
                    {
                        var kerning = GetKerning(cachedCharacter, nextCachedCharacter);
                        var maxBounds = cachedCharacter.AdvanceX * Constants.Settings.KerningSanityMultiplier;
                        if (kerning <= maxBounds && kerning >= -maxBounds)
                        {
                            offsetX += kerning;
                        }
                    }
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Font other)
        {
            return !(other is null) && Equals(Key, other.Key);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return obj != null && (obj is Font || obj is FontImplementation) && Equals((Font)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (FontFamily.GetHashCode() * 397) ^ Size.GetHashCode();
        }

        /// <summary>
        /// Resizes the text cache.
        /// </summary>
        /// <param name="newCacheSize">New size of the cache.</param>
        public void ResizeTextCache(int newCacheSize)
        {
            TextCache.MaxCacheSize = newCacheSize;
        }

        /// <summary>
        /// Converts the Font to a string to provide debug information on the font.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"font: [{FontFamily}], size: [{Size}]";
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Font left, Font right)
        {
            return !(left is null) && left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Font left, Font right)
        {
            return !(left is null) && !left.Equals(right);
        }

        internal void PreGenerateCharacterGlyphs(char[] characters)
        {
            if (characters == null)
            {
                characters = Constants.Settings.DefaultCharacterList;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < characters.Length; i++)
            {
                GenerateGlyph(characters[i]);
            }
        }

        /// <summary>
        /// Gets the kerning between two characters.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The kerning between the characters.</returns>
        private int GetKerning(Internal.Glyph left, Internal.Glyph right)
        {
            if (left.Kerning.TryGetValue(right.Character, out var kerning))
            {
                left.Kerning[right.Character] = kerning =
                    (int)Face.GetKerning((uint)left.Index, (uint)right.Index, KerningMode.Default).X;
            }

            return kerning;
        }

        private Internal.Glyph GenerateGlyph(char character)
        {
            var cache = _glyphCaches.FirstOrDefault(c => !c.Full);
            if (cache == null)
            {
                cache = new GlyphCache(this);
                _glyphCaches.Add(cache);
            }

            if (!cache.AddCharacterToCache(character, out var cachedGlyph))
            {
                cache = new GlyphCache(this);
                _glyphCaches.Add(cache);
                if (!cache.AddCharacterToCache(character, out cachedGlyph))
                {
                    throw new Exception($"Could not generate character [{character}]!");
                }
            }

            return cachedGlyph;
        }

        private bool TryGetGlyph(char character, out Internal.Glyph glyph)
        {
            if (!CharacterGlyphs.TryGetValue(character, out glyph))
            {
                glyph = GenerateGlyph(character);
                CharacterGlyphs.Add(character, glyph);
            }

            return true;
        }
    }
}
