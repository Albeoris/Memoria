using System;
using UnityEngine;
using Memoria.Prime;
using System.IO;
using Memoria;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDisplay : MonoBehaviour
{
	public Int32 ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
			if (sheet != null)
			{
				if ((name == "Frame" && _id == 7) && Configuration.Mod.TranceSeek)
                {
					String externalPath = "TranceSeek/FF9_Data/embeddedasset/quadmist/atlas/goldenbluecardframe";
					if (!File.Exists(externalPath))
					{
						Log.Message("[SpriteDisplay] ERREUR :" + externalPath + " n'existe pas !");
						_id = 5;
						QuadMistResourceManager instance = QuadMistResourceManager.Instance;
						GetComponent<SpriteRenderer>().sprite = instance.GetResource(sheet.name, _id).Sprite;
					}
					else
					{
                        var rawData = File.ReadAllBytes(externalPath);
						Texture2D texture = new Texture2D(2, 2);
						texture.LoadImage(rawData);
						GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, 204, 247), new Vector2(0, 1f), 481);
                    }

				}
				else if ((name == "Frame" && _id == 8) && Configuration.Mod.TranceSeek)
                {
					String externalPath = "TranceSeek/FF9_Data/embeddedasset/quadmist/atlas/goldenredcardframe";
					if (!File.Exists(externalPath))
					{
						Log.Message("[SpriteDisplay] ERREUR :" + externalPath + " n'existe pas !");
						_id = 6;
						QuadMistResourceManager instance = QuadMistResourceManager.Instance;
						GetComponent<SpriteRenderer>().sprite = instance.GetResource(sheet.name, _id).Sprite;
					}
					else
					{
						var rawData = File.ReadAllBytes(externalPath);
						Texture2D texture = new Texture2D(2, 2);
						texture.LoadImage(rawData);
                        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, 204, 247), new Vector2(0, 1f), 481);
					}
	
				}
				else
				{
					QuadMistResourceManager instance = QuadMistResourceManager.Instance;
					GetComponent<SpriteRenderer>().sprite = instance.GetResource(sheet.name, _id).Sprite;
				}

			}
			else
			{
				_id = 0;
			}
		}
	}

	private void Start()
	{
		ID = _id;
	}

	[SerializeField]
	public SpriteSheet sheet;

	[SerializeField]
	public Int32 _id;
}
