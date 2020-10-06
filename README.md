# BragiFont
An alternative solution for Monogame/FNA/XNA-derived frameworks that utilizes SharpFont to draw text rather than the traditional SpriteFont approach.

# Installation
There are nuget packages available for Monogame and FNA.
- Monogame: BragiFont.Monogame
- FNA: BragiFont.FNA

# Usage
Approach 1: Draw Text Directly
```
var fontSize = 48;
// Bragi.Core.Initialize(GraphicsDevice); // Optional, if not called you'll need to add the GraphicsDevice to the first Bragi.Core.GetFont call that is made to set the GraphicsDevice.
var font = Bragi.Core.GetFont("pathToFontFile", fontSize, GraphicsDevice);
_spriteBatch.DrawString(font, "Hello World!", new Vector2(50, 50), Color.White);
```

Approach 2: Cache Text (quicker since we don't need to rebuild the text glyph list on subsequent calls)
```
var fontSize = 48;
// Bragi.Core.Initialize(GraphicsDevice); // Optional, if not called you'll need to add the GraphicsDevice to the first Bragi.Core.GetFont call that is made to set the GraphicsDevice.
var font = Bragi.Core.GetFont("pathToFontFile", fontSize, GraphicsDevice);
var text = font.MakeText("Hello World!");
_spriteBatch.DrawString(text, new Vector2(50, 50), Color.White);

```

# Future Plans
- Add support for text rotation
- Add support for making text italic, bold, etc.
