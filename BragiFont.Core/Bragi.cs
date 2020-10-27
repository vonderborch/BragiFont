﻿using System;
using System.Collections.Generic;
using System.IO;
using BragiFont.Internal;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace BragiFont
{
    /// <summary>
    /// The core font system.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public sealed class Bragi : IDisposable
    {
        /// <summary>
        /// The SharpFont library.
        /// </summary>
        private static readonly Library Library;

        /// <summary>
        /// The fonts we haved cached.
        /// </summary>
        private static readonly Dictionary<string, Typeface> Typefaces;

        /// <summary>
        /// Initializes the <see cref="Bragi"/> class.
        /// </summary>
        static Bragi()
        {
            Library = new Library();
            Typefaces = new Dictionary<string, Typeface>();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Bragi"/> class from being created.
        /// </summary>
        private Bragi() { }

        /// <summary>
        /// Gets the FontSystem.
        /// </summary>
        /// <value>
        /// The FontSystem.
        /// </value>
        public static Bragi Core { get; } = new Bragi();

        internal Library BragiLibrary => Library;

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>
        /// The graphics device.
        /// </value>
        internal GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var font in Typefaces.Values)
            {
                font.DisposeFinal();
            }

            Library.Dispose();
        }

        /// <summary>
        /// Initializes the specified graphics device.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Gets or loads the specified font with the specified size.
        /// </summary>
        /// <param name="path">The path to load the font from.</param>
        /// <param name="size">The size of the font.</param>
        /// <param name="graphicsDevice">The graphics device. Optional, but required if Bragi.Core.Initialize() hasn't been called yet.</param>
        /// <param name="preGenerateCharacters">if set to <c>true</c> [pre generate characters].</param>
        /// <param name="charactersToPregenerate">The characters to pregenerate.</param>
        /// <returns>The Font that matches the specified parameters.</returns>
        /// <exception cref="Exception">GraphicsDevice is not initialized! Please either initialize Bragi.Core or provide the GraphicsDevice when getting a new font.</exception>
        public Typeface GetTypeface(string path, GraphicsDevice graphicsDevice = null, bool preGenerateCharacters = false, char[] charactersToPregenerate = null)
        {
            return GetTypeface(path, File.ReadAllBytes(path), graphicsDevice, preGenerateCharacters, charactersToPregenerate);
        }

        public Typeface GetTypeface(string name, Stream fileStream, GraphicsDevice graphicsDevice = null, bool preGenerateCharacters = false, char[] charactersToPregenerate = null)
        {
            var buffer = Helpers.ReadStream(fileStream);

            return GetTypeface(name, buffer, graphicsDevice, preGenerateCharacters, charactersToPregenerate);
        }

        public Typeface GetTypeface(string name, byte[] fileData, GraphicsDevice graphicsDevice = null, bool preGenerateCharacters = false, char[] charactersToPregenerate = null)
        {
            if (graphicsDevice == null && GraphicsDevice == null)
            {
                throw new Exception("GraphicsDevice is not initialized! Please either initialize Bragi.Core or provide the GraphicsDevice when getting a new font.");
            }

            if (graphicsDevice != null)
            {
                GraphicsDevice = graphicsDevice;
            }

            if (!Typefaces.TryGetValue(name, out var typeface))
            {
                typeface = new TypefaceImplementation(name, fileData, preGenerateCharacters, charactersToPregenerate);
                
                Typefaces.Add(name, typeface);
            }

            return typeface;
        }

        /// <summary>
        /// Removes a font from the system.
        /// </summary>
        /// <param name="key">The key for the font we want to remove.</param>
        internal void RemoveTypeface(string name)
        {
            Typefaces.Remove(name);
        }
    }
}
