using System;
using UnityEngine;
using static SFXDataMesh.ModelSequence;

public class ResultText : MonoBehaviour
{
    public Int32 ID
    {
        set
        {
            this.content.ID = value;
            this.shadow.ID = value;
            this.content.transform.localPosition = this.map[value];
            this.shadow.transform.localPosition = this.map[value] + new Vector3(-0f, 0f, 0.1f);
        }
    }

    public Single Alpha // [DV] TODO => Doesn't work... didn't fade as intended. But Board.FadeInBoard works ?
    {
        set
        {
            this.content.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, value);
            this.shadow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, value);
        }
    }

    [SerializeField]
    private SpriteDisplay content;

    [SerializeField]
    private SpriteDisplay shadow;

    private Vector3[] map = new Vector3[]
    {
        new Vector3(1.08f, -0.88f, 0f),
        new Vector3(1.06f, -0.88f, 0f),
        new Vector3(0.92f, -0.88f, 0f),
        new Vector3(0.74f, -0.88f, 0f),
        new Vector3(1.08f, -0.88f, 0f),
        new Vector3(1.06f, -0.88f, 0f),
        new Vector3(0.92f, -0.88f, 0f),
        new Vector3(0.74f, -0.88f, 0f)
    };
}