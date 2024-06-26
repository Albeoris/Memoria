using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;

public class PreBoard : MonoBehaviour
{
	public Collection collection
	{
		get
		{
			return this._collection;
		}
		set
		{
			this._collection = value;
			this.UpdateCollection(-1);
		}
	}

	public QuadMistCard Preview
	{
		get
		{
			return this.list[this.select];
		}
	}

	public void InitResources()
	{
		this.cardPreview.InitResources();
		this.Execute();
	}

	[ContextMenu("Generate In Editor")]
	public void Execute()
	{
		base.transform.localPosition = PreBoard.POSITION;
		for (Int32 i = 0; i < 10; i++)
		{
			for (Int32 j = 0; j < 10; j++)
			{
				Int32 num = j * 10 + i;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.iconPrefab);
				this.cardIconUIs[num] = gameObject.GetComponent<CardIconUI>();
				this.cardIconUIs[num].Count = 0;
				this.cardIconUIs[num].ID = num;
				gameObject.name = num.ToString();
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3((Single)j * CardIconUI.SIZE_W, (Single)(-(Single)i) * CardIconUI.SIZE_H, 0f);
			}
		}
	}

	public void UpdateCollection(Int32 n = -1)
	{
		if (this.collection != null)
		{
			if (n == -1)
			{
				for (Int32 i = 0; i < CardPool.TOTAL_CARDS; i++)
				{
					this.cardIconUIs[i].ID = i;
					this.cardIconUIs[i].Count = this.collection.GetCardsWithID((TetraMasterCardId)i).Count;
				}
			}
			else
			{
				this.cardIconUIs[n].Count = this.collection.GetCardsWithID((TetraMasterCardId)n).Count;
			}
		}
	}

	public void NextCard()
	{
		if (this.list.Count == 0)
		{
			return;
		}
		this.select++;
		if (this.select == this.list.Count)
		{
			this.select = 0;
		}
		this.cardPreview.Preview = this.list[this.select];
		this.cardPreview.Count = this.list.Count;
		this.cardPreview.SetTextSelect(this.select, this.list.Count);
		this.cardPreview.Next();
	}

	public void PrevCard()
	{
		if (this.list.Count == 0)
		{
			return;
		}
		this.select--;
		if (this.select == -1)
		{
			this.select = this.list.Count - 1;
		}
		this.cardPreview.Preview = this.list[this.select];
		this.cardPreview.Count = this.list.Count;
		this.cardPreview.SetTextSelect(this.select, this.list.Count);
		this.cardPreview.Prev();
	}

	public void SetPreviewCardID(Int32 indx)
	{
		this.list = this.collection.GetCardsWithID((TetraMasterCardId)indx);
		this.listIndex = indx;
		this.UpdatePreview();
		this.UpdateCursor(indx);
	}

	public Boolean GetPreviewByWorldPoint(Vector3 worldPoint)
	{
		return this.cardPreview.gameObject.activeSelf && this.cardPreview.Contains(worldPoint);
	}

	public Int32 GetLRByWorldPoint(Vector3 worldPoint)
	{
		if (!this.cardPreview.gameObject.activeSelf)
		{
			return 0;
		}
		if (this.cardPreview.left.Contains(worldPoint))
		{
			return -1;
		}
		if (this.cardPreview.right.Contains(worldPoint))
		{
			return 1;
		}
		return 0;
	}

	public Int32 GetIndexByWorldPoint(Vector3 worldPoint)
	{
		for (Int32 i = 0; i < 100; i++)
		{
			if (this.cardIconUIs[i].Contains(worldPoint))
			{
				return i;
			}
		}
		return -1;
	}

	public void Add(QuadMistCard c)
	{
		this.collection.Add(c);
		this.UpdateCollection((Int32)c.id);
		this.UpdatePreview();
	}

	public void Remove(QuadMistCard c)
	{
		this.collection.Remove(c);
		this.UpdateCollection((Int32)c.id);
		this.UpdatePreview();
	}

	public QuadMistCard RemoveSelected()
	{
		QuadMistCard quadMistCard = this.list[this.select];
		this.Remove(quadMistCard);
		return quadMistCard;
	}

	public Int32 CountSelected()
	{
		return this.list.Count;
	}

	private void UpdatePreview()
	{
		if (this.list.Count > 0)
		{
			this.select = 0;
			this.cardPreview.gameObject.SetActive(true);
			this.cardPreview.Preview = this.list[this.select];
			this.cardPreview.Count = this.list.Count;
			this.cardPreview.SetTextID(this.listIndex);
			this.cardPreview.SetTextSelect(this.select, this.list.Count);
		}
		else
		{
			this.cardPreview.gameObject.SetActive(false);
		}
	}

	private void UpdateCursor(Int32 i)
	{
		this.cursor.transform.position = this.cardIconUIs[i].transform.position + new Vector3(0f, 0f, -1f);
	}

	private static Vector3 POSITION = new Vector3(0.18f, -0.45f, 1f);

	private static Int32 SIZE_X = 10;

	private static Int32 SIZE_Y = 10;

	public GameObject iconPrefab;

	public GameObject cursor;

	public CardIconUI[] cardIconUIs = new CardIconUI[100];

	public CardPreviewUI cardPreview;

	public Hand hand;

	private Int32 select;

	private Int32 listIndex;

	private List<QuadMistCard> list;

	private Collection _collection;
}
