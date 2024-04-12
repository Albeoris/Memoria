using Memoria.Prime;
using Memoria.Prime.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Memoria.Assets.TexturePacker
{
	public class TPSpriteSheetLoader
	{
		private const Int64 KnownFormat = 40000;
		private static Int64 _counter = 0;

		private readonly String _texturePath;
		private readonly String _tpsheetPath;

		private Texture2D _texture;
		private StreamReader _sr;
		private Int64 _format;
		private String _textureName;
		private String _normalMapName;
		private Int32 _textureWidth;
		private Int32 _textureHeight;
		private LinkedList<Sprite> _sprites;
		private LinkedList<SpriteSheet.Info> _spriteInfos;
		private Boolean _appendTexture;

		public TPSpriteSheetLoader(String tpsheetPath)
		{
			_tpsheetPath = Path.ChangeExtension(tpsheetPath, ".tpsheet");
			_texturePath = Path.ChangeExtension(tpsheetPath, ".png");

			Exceptions.CheckFileNotFoundException(_tpsheetPath);
		}

		public SpriteSheet Load(Texture2D texture = null)
		{
			try
			{
				if (texture == null)
				{
					Exceptions.CheckFileNotFoundException(_texturePath);
					_texture = StreamingResources.LoadTexture2D(_texturePath);
				}
				else
				{
					_texture = texture;
				}
				_sprites = new LinkedList<Sprite>();
				_spriteInfos = new LinkedList<SpriteSheet.Info>();
				using (_sr = new StreamReader(_tpsheetPath))
				{
					while (!_sr.EndOfStream)
						ReadNex();
				}

				// x86/FF9_Data/output.log.txt says:
				// "SpriteSheet must be instantiated using the ScriptableObject.CreateInstance method instead of new SpriteSheet."
				return new SpriteSheet
				{
					name = "TPSheet_" + _textureName + Interlocked.Increment(ref _counter),
					sheet = _sprites.ToArray(),
					info = _spriteInfos.ToArray(),
					appendTexture = _appendTexture
				};
			}
			finally
			{
				_format = 0;
				_textureName = null;
				_normalMapName = null;
				_textureWidth = 0;
				_textureHeight = 0;
				_texture = null;
				_sprites = null;
				_appendTexture = false;
			}
		}

		private void ReadNex()
		{
			switch ((Char)_sr.Peek())
			{
				case '#':
				case '\r':
				case '\n':
					_sr.ReadLine();
					return;
				case ':':
					ReadConfig();
					break;
				default:
					ReadSprite();
					break;
			}
		}

		private void ReadConfig()
		{
			String line = _sr.ReadLine();
			if (line == null)
				return;

			try
			{
				String[] pair = line.Split('=');
				switch (pair[0])
				{
					case ":format":
						ParseFormatValue(pair);
						break;
					case ":texture":
						ParseTextureValue(pair);
						break;
					case ":size":
						ParseSizeValue(pair);
						break;
					case ":normalmap":
						ParseNormalMapValue(pair);
						break;
					case ":append":
						ParseAppendValue(pair);
						break;
					default:
						Log.Warning("An unknown tpsheet config occurred: " + line);
						break;
				}
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "An unexpected error occurred while parsing a tpsheet config: " + line);
			}
		}

		private void ParseFormatValue(String[] pair)
		{
			_format = Int64.Parse(pair[1], CultureInfo.InvariantCulture);
			if (_format != KnownFormat)
				Log.Warning("An unexpected format value occurred: {0}, expected: {1}", _format, KnownFormat);
		}

		private void ParseTextureValue(String[] pair)
		{
			_textureName = pair[1];
		}

		private void ParseSizeValue(String[] pair)
		{
			String[] data = pair[1].Split('x');
			_textureWidth = Int32.Parse(data[0], CultureInfo.InvariantCulture);
			_textureHeight = Int32.Parse(data[1], CultureInfo.InvariantCulture);
		}

		private void ParseNormalMapValue(String[] pair)
		{
			_normalMapName = pair[1];
		}

		private void ParseAppendValue(String[] pair)
		{
			_appendTexture = Boolean.Parse(pair[1]);
		}

		private void ReadSprite()
		{
			String line = _sr.ReadLine();
			if (String.IsNullOrEmpty(line))
				return;

			String[] general = line.Split(new[] { "; " }, StringSplitOptions.None);
			if (general.Length < 1)
				return;

			String[] parts = general[0].Split(';');
			if (parts.Length < 7)
				return;

			SpriteSheet.Info info = new SpriteSheet.Info();
			String name = TPSheetSpriteNameFormatter.UnescapeSpecialChars(parts[0]);
			Int32 frameX = Int32.Parse(parts[1], CultureInfo.InvariantCulture);
			Int32 frameY = Int32.Parse(parts[2], CultureInfo.InvariantCulture);
			Int32 frameW = Int32.Parse(parts[3], CultureInfo.InvariantCulture);
			Int32 frameH = Int32.Parse(parts[4], CultureInfo.InvariantCulture);
			Single pivotX = Single.Parse(parts[5], CultureInfo.InvariantCulture);
			Single pivotY = Single.Parse(parts[6], CultureInfo.InvariantCulture);
			if (parts.Length > 10)
				info.padding = new Vector4(Single.Parse(parts[7], CultureInfo.InvariantCulture),
					Single.Parse(parts[8], CultureInfo.InvariantCulture),
					Single.Parse(parts[9], CultureInfo.InvariantCulture),
					Single.Parse(parts[10], CultureInfo.InvariantCulture));
			else
				info.padding = new Vector4();
			if (parts.Length > 14)
				info.border = new Vector4(Single.Parse(parts[11], CultureInfo.InvariantCulture),
					Single.Parse(parts[12], CultureInfo.InvariantCulture),
					Single.Parse(parts[13], CultureInfo.InvariantCulture),
					Single.Parse(parts[14], CultureInfo.InvariantCulture));
			else
				info.border = new Vector4();

			Sprite sprite = Sprite.Create(_texture, new Rect(frameX, frameY, frameW, frameH), new Vector2(pivotX, pivotY));
			sprite.name = name;

			Vector2[] vertices = null;
			if (general.Length > 1)
			{
				parts = general[1].Split(';');
				vertices = new Vector2[Int32.Parse(parts[0], CultureInfo.InvariantCulture)];
				for (Int32 i = 0; i < vertices.Length; i++)
				{
					Single x = Single.Parse(parts[1 + 2 * i], CultureInfo.InvariantCulture);
					Single y = Single.Parse(parts[2 + 2 * i], CultureInfo.InvariantCulture);
					vertices[i] = new Vector2(x, y);
				}
			}

			UInt16[] triangleIndices = null;
			if (general.Length > 2)
			{
				parts = general[2].Split(';');
				triangleIndices = new UInt16[3 * Int32.Parse(parts[0], CultureInfo.InvariantCulture)];
				for (Int32 i = 0; i < triangleIndices.Length; i++)
					triangleIndices[i] = UInt16.Parse(parts[1 + i], CultureInfo.InvariantCulture);
			}

			if (vertices != null || triangleIndices != null)
				sprite.OverrideGeometry(vertices ?? sprite.vertices, triangleIndices ?? sprite.triangles);

			_sprites.AddLast(sprite);
			_spriteInfos.AddLast(info);
		}
	}
}
