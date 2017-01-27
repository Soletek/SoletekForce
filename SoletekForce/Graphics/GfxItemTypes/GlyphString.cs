using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace SoletekForce.Graphics
{
	/// <summary>
	/// A text string based on a string of Glyphs from single texture.
	/// </summary>
	public class GlyphString : Sprite
	{
		readonly GlyphFont font;
		string text = "";
		int letterCount;
		Vector3 positionOffset = Vector3.Zero;
		Color4 color = Color4.White;
		bool centered = true;

		public GlyphString(Entity holder, GlyphFont font, string text = "") : base(holder)
		{
			this.font = font;
			SetTexture(font.GetTexture());
			Text = text;
		}

		public Vector3 PositionOffset
		{
			get { return positionOffset; }
			set
			{
				positionOffset = value;
				UpdateTextModel();
			}
		}

		public Color4 Color
		{
			get { return color; }
			set
			{
				color = value;
				SetColor(color);
			}
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				UpdateTextModel();
			}
		}

		void UpdateTextModel()
		{
			float offset = 0;

			var indexList = new List<ushort>();
			var vertexList = new List<Vertex>();

			if (letterCount <= text.Length)
			{
				if (indices != null) indexList.AddRange(indices);
				if (verticles != null) vertexList.AddRange(verticles);
			}
			else letterCount = 0;

			for (int i = letterCount; i < text.Length; i++)
			{
				GenerateMesh(indexList, vertexList, Vector3.Zero, color);
			}

			letterCount = text.Length;
			indices = indexList.ToArray();
			verticles = vertexList.ToArray();

			if (centered)
			{
				for (int i = 0; i < text.Length; i++)
				{
					GlyphData glyph = font.GetGlyph(text[i]);
					offset += glyph.LetterWidth;
				}

				offset = -offset / 2;
			}

			for (int i = 0; i < text.Length; i++)
			{
				GlyphData glyph = font.GetGlyph(text[i]);

				float w = glyph.LetterWidth;
				float h = glyph.LetterHeight;

				verticles[i * 4 + 0].position = new Vector3(offset, -h / 2, 0f) + positionOffset;
				verticles[i * 4 + 1].position = new Vector3(offset + w, -h / 2, 0f) + positionOffset;
				verticles[i * 4 + 2].position = new Vector3(offset + w, h / 2, 0f) + positionOffset;
				verticles[i * 4 + 3].position = new Vector3(offset, h / 2, 0f) + positionOffset;

				ClipTexture(glyph.X, glyph.Y, glyph.W, glyph.H, i * 4);
				offset += glyph.LetterWidth;
			}
		}
	}

	/// <summary>
	/// A collection of glyphs.
	/// </summary>
	public class GlyphFont
	{
		Dictionary<char, GlyphData> letters = new Dictionary<char, GlyphData>();
		Texture texture;
		readonly float dpi;

		public GlyphFont(Texture texture, float dpi, float spaceWidth = 10f)
		{
			this.texture = texture;
			this.dpi = dpi;

			SetGlyph(' ', new GlyphData(dpi, 0, 0, 0, 0, spaceWidth, 0));
		}

		public Texture GetTexture()
		{
			return texture;
		}

		public void SetGlyph(char character, GlyphData glyphData)
		{
			if (letters.ContainsKey(character))
			{
				letters.Remove(character);
			}
			letters.Add(character, glyphData);
		}

		public GlyphData GetGlyph(char character)
		{
			if (letters.ContainsKey(character))
			{
				return letters[character];
			}

			return new GlyphData();
		}

		/// <summary>
		/// Adds multiple monospaced horizontally aligned glyphs to the GlyphFont.
		/// </summary>
		public void AddMultiple(string coding, int x, int y, int w, int h, int offset)
		{
			foreach (var character in coding)
			{
				SetGlyph(character, new GlyphData(dpi, x, y, w, h));
				x += w + offset;
			}
		}
	}

	public struct GlyphData
	{
		public readonly float X;
		public readonly float Y;
		public readonly float W;
		public readonly float H;
		public readonly float LetterWidth;
		public readonly float LetterHeight;

		public GlyphData(float dpi, float x, float y, float w, float h)
			: this(dpi, x, y, w, h, w, h)
		{ }

		public GlyphData(float dpi, float x, float y, float w, float h, float letterWidth, float letterHeight)
		{
			X = x;
			Y = y;
			W = w;
			H = h;
			LetterWidth = letterWidth * dpi;
			LetterHeight = letterHeight * dpi;
		}
	}
}
