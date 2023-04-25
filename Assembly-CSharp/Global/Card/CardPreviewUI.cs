using System;
using System.Collections;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;

public class CardPreviewUI : MonoBehaviour
{
	public QuadMistCard Preview
	{
		get
		{
			return stacks[0].Data;
		}
		set
		{
			stacks[0].Data = value;
			ID = (Int32)value.id;
		}
	}

	public Int32 Count
	{
		set
		{
			if (value > (Int32)stacks.Length)
			{
				value = (Int32)stacks.Length;
			}
			if (value < 0)
			{
				value = 0;
			}
			for (Int32 i = 0; i < (Int32)stacks.Length; i++)
			{
				if (i < value)
				{
					stacks[i].gameObject.SetActive(true);
				}
				else
				{
					stacks[i].gameObject.SetActive(false);
				}
			}
		}
	}

	private Int32 ID
	{
		set
		{
			for (Int32 i = 1; i < (Int32)stacks.Length; i++)
			{
				stacks[i].Data = CardPool.GetMaxStatCard(value);
			}
		}
	}

	public void InitResources()
	{
		CreateStack();
	}

	private void CreateStack()
	{
		for (Int32 i = 0; i < 5; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(cardPrefab);
			stacks[i] = gameObject.GetComponent<QuadMistCardUI>();
			stacks[i].ResetEffect();
			gameObject.name = "Preview0" + i;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = default(Vector3);
			gameObject.SetActive(false);
		}
		for (Int32 j = 0; j < (Int32)stacks.Length; j++)
		{
			stacks[j].Small = false;
			stacks[j].transform.localPosition = new Vector3((Single)j * 0.04f, -((Single)j * 0.01f - 0.17f), 1f * (Single)(j + 1));
		}
	}

	private IEnumerator AnimateNext()
	{
		Single time = 0f;
		Single tick = time * 60f;
		for (Int32 i = 4; i > 0; i--)
		{
			Single offx = 0.01f * (Single)(i - 4);
			for (Int32 j = 1; j < (Int32)stacks.Length; j++)
			{
				stacks[j].transform.localPosition = new Vector3((Single)j * 0.04f + offx, -((Single)j * 0.01f - 0.17f), 1f * (Single)(j + 1));
			}
			yield return new WaitForSeconds(tick);
		}
		for (Int32 k = 1; k < (Int32)stacks.Length; k++)
		{
			stacks[k].transform.localPosition = new Vector3((Single)k * 0.04f, -((Single)k * 0.01f - 0.17f), 1f * (Single)(k + 1));
		}
		yield break;
	}

	private IEnumerator AnimatePrev()
	{
		Single time = 0f;
		Single tick = time * 60f;
		for (Int32 i = 4; i > 0; i--)
		{
			Single offx = 0.01f * (Single)(-(Single)i + 1);
			for (Int32 j = 1; j < (Int32)stacks.Length; j++)
			{
				stacks[j].transform.localPosition = new Vector3((Single)j * 0.04f + offx, -((Single)j * 0.01f - 0.17f), 1f * (Single)(j + 1));
			}
			yield return new WaitForSeconds(tick);
		}
		for (Int32 k = 1; k < (Int32)stacks.Length; k++)
		{
			stacks[k].transform.localPosition = new Vector3((Single)k * 0.04f, -((Single)k * 0.01f - 0.17f), 1f * (Single)(k + 1));
		}
		yield break;
	}

	public void Next()
	{
		base.StartCoroutine(AnimateNext());
	}

	public void Prev()
	{
		base.StartCoroutine(AnimatePrev());
	}

	public Boolean Contains(Vector3 worldPoint)
	{
		return stacks[0].Contains(worldPoint);
	}

	public void SetTextID(Int32 id)
	{
		no.text = "NO" + (id + 1);
		cardName.text = FF9TextTool.CardName(id);
	}

	public void SetTextSelect(Int32 cur, Int32 max)
	{
		select.text = cur + 1 + "/" + max;
	}

	public QuadMistCardUI[] stacks;

	public SpriteClickable left;

	public SpriteClickable right;

	public TextMesh no;

	public TextMesh select;

	public TextMesh cardName;

	public GameObject cardPrefab;
}
