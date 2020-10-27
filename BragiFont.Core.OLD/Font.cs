using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BragiFont.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace BragiFont
{
    /// <summary>
    /// A Font that can be used to draw text to the screen
    /// </summary>
    /// <seealso cref="System.IEquatable{Font}" />
    /// <seealso cref="System.IDisposable" />
    public abstract class Font : IEquatable<Font>, IDisposable
    {
        /// <summary>
        /// The font face
        /// </summary>
        protected readonly Face FontFace;

        /// <summary>
        /// The font family
        /// </summary>
        protected readonly string FontFamilyName;

        /// <summary>
        /// The font size
        /// </summary>
        protected readonly int FontFaceSize;

        /// <summary>
        /// Caches of glyphs
        /// </summary>
        private readonly List<GlyphCache> _glyphCaches;

        /// <summary>
        /// The key for this Font
        /// </summary>
        protected readonly Tuple<string, int> KeyTuple;

        /// <summary>
        /// The characters that we currently have generated glyphs for
        /// </summary>
        internal Dictionary<char, Character> Characters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="face">The font face.</param>
        /// <param name="fontSize">Size of the font.</param>
        protected Font(Tuple<string, int> key, Face face, int fontSize)
        {
            KeyTuple = key;
            FontFamilyName = face.FamilyName;
            FontFaceSize = fontSize;
            FontFace = face;
            Characters = new Dictionary<char, Character>();
            _glyphCaches = new List<GlyphCache>();

            GlyphHeight = face.Size.Metrics.Height.Ceiling();
        }

        /// <summary>
        /// Gets the key for this Font.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Tuple<string, int> Key => KeyTuple;

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily => FontFamilyName;

        /// <summary>
        /// Gets the font face.
        /// </summary>
        /// <value>
        /// The face.
        /// </value>
        public Face Face => FontFace;

        /// <summary>
        /// Gets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public int FontSize => FontFaceSize;

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
        /// Gets or sets the number of spaces we'll use when a tab is requested.
        /// </summary>
        /// <value>
        /// The spaces in tab.
        /// </value>
        public int SpacesInTab { get; set; } = Constants.Settings.DefaultSpacesInTab;

        /// <summary>
        /// Gets the height of glyphs in the Font.
        /// </summary>
        /// <value>
        /// The height of the glyph.
        /// </value>
        public int GlyphHeight { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            FontFace.Dispose();
            Bragi.Core.RemoveFont(Key);
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void DisposeFinal()
        {
            FontFace.Dispose();
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
            return obj != null && (obj is Font || obj is FontImplementation) && Equals((Font) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (FontFamilyName.GetHashCode() * 397) ^ FontFaceSize.GetHashCode();
        }

        /// <summary>
        /// Converts the Font to a string to provide debug information on the font.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"font: [{FontFamilyName}], size: [{FontFaceSize}]";
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
                TryGetCharacter(text[i], out var cachedCharacter);

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

                spriteBatch.Draw(cachedCharacter.Texture.Texture, new Vector2(boundaries.X + offsetX, boundaries.Y + offsetY), cachedCharacter.Boundary, color);
                offsetX += cachedCharacter.Boundary.Width;

                // calculate kerning
                if (i != finalCharacterIndex)
                {
                    var nextCharacter = text[i + 1];
                    if (TryGetCharacter(nextCharacter, out var nextCachedCharacter))
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
            var finalSize = new Vector2(0, 0);

            var textResult = new TextImplementation(text, this);

            var offsetX = 0;
            var offsetY = 0;

            var underrun = 0;
            var finalCharacterIndex = text.Length - 1;

            for (var i = 0; i < text.Length; i++)
            {
                TryGetCharacter(text[i], out var cachedCharacter);
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
                    if (TryGetCharacter(nextCharacter, out var nextCachedCharacter))
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

            return textResult;
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
                TryGetCharacter(text[i], out var cachedCharacter);
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
                    if (TryGetCharacter(nextCharacter, out var nextCachedCharacter))
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
        /// Pre-generates the characters.
        /// </summary>
        /// <param name="charactersToPregenerate">The characters to pre-generate.</param>
        public void PregenerateCharacters(char[] charactersToPregenerate)
        {
            if (charactersToPregenerate == null)
            {
                charactersToPregenerate = Constants.Settings.DefaultCharacterList;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < charactersToPregenerate.Length; i++)
            {
                GenerateCharacter(charactersToPregenerate[i]);
            }
        }

        /// <summary>
        /// Gets the kerning between two characters.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The kerning between the characters.</returns>
        private int GetKerning(Character left, Character right)
        {
            if (left.Kerning.TryGetValue(right.Char, out var kerning))
            {
                left.Kerning[right.Char] = kerning =
                    (int)FontFace.GetKerning((uint) left.Index, (uint) right.Index, KerningMode.Default).X;
            }

            return kerning;
        }

        /// <summary>
        /// Tries to get a Character object representing a requested char, and if we haven't generated it already, we generate the character.
        /// </summary>
        /// <param name="character">The character we want to fetch.</param>
        /// <param name="bragiCharacter">The character object representing the char.</param>
        /// <returns>Whether we were able to fetch the character.</returns>
        private bool TryGetCharacter(char character, out Character bragiCharacter)
        {
            if (!Characters.TryGetValue(character, out bragiCharacter))
            {
                bragiCharacter = GenerateCharacter(character);
                Characters.Add(character, bragiCharacter);
            }

            return true;
        }

        /// <summary>
        /// Generates the character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>The generated character</returns>
        /// <exception cref="Exception">Could not generate character [{character}]!</exception>
        private Character GenerateCharacter(char character)
        {
            var glyphCache = _glyphCaches.FirstOrDefault(c => !c.Full);
            if (glyphCache == null)
            {
                glyphCache = new GlyphCache(this);
                _glyphCaches.Add(glyphCache);
            }

            if (!glyphCache.AddCharacterToCache(character, out var cachedCharacter))
            {
                glyphCache = new GlyphCache(this);
                var finalAttempt = glyphCache.AddCharacterToCache(character, out cachedCharacter);
                if (!finalAttempt)
                {
                    throw new Exception($"Could not generate character [{character}]!");
                }

                _glyphCaches.Add(glyphCache);
            }

            return cachedCharacter;
        }

    }
}
