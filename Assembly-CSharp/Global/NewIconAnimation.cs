using System;
using UnityEngine;

public class NewIconAnimation : MonoBehaviour
{
    public Single Duration
    {
        get => duration;
        set => duration = value;
    }

    private void Awake()
    {
        sprite = gameObject.GetComponent<UISprite>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            ChangeColor();
            timer = 0f;
        }
    }

    private void ChangeColor()
    {
        isTransparent = !isTransparent;
        sprite.alpha = isTransparent ? 0.3f : 1f;
    }

    private Single timer;
    private Boolean isTransparent;
    private Single duration = 0.5f;

    [NonSerialized]
    private UISprite sprite;
}
