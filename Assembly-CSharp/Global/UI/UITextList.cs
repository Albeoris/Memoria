using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
    protected BetterList<UITextList.Paragraph> paragraphs
    {
        get
        {
            if (this.mParagraphs == null && !UITextList.mHistory.TryGetValue(base.name, out this.mParagraphs))
            {
                this.mParagraphs = new BetterList<UITextList.Paragraph>();
                UITextList.mHistory.Add(base.name, this.mParagraphs);
            }
            return this.mParagraphs;
        }
    }

    public Boolean isValid
    {
        get
        {
            return this.textLabel != (UnityEngine.Object)null && this.textLabel.ambigiousFont != (UnityEngine.Object)null;
        }
    }

    public Single scrollValue
    {
        get
        {
            return this.mScroll;
        }
        set
        {
            value = Mathf.Clamp01(value);
            if (this.isValid && this.mScroll != value)
            {
                if (this.scrollBar != (UnityEngine.Object)null)
                {
                    this.scrollBar.value = value;
                }
                else
                {
                    this.mScroll = value;
                    this.UpdateVisibleText();
                }
            }
        }
    }

    protected Single lineHeight
    {
        get
        {
            return (!(this.textLabel != (UnityEngine.Object)null)) ? 20f : ((Single)this.textLabel.fontSize + this.textLabel.effectiveSpacingY);
        }
    }

    protected Int32 scrollHeight
    {
        get
        {
            if (!this.isValid)
            {
                return 0;
            }
            Int32 num = Mathf.FloorToInt((Single)this.textLabel.height / this.lineHeight);
            return Mathf.Max(0, this.mTotalLines - num);
        }
    }

    public void Clear()
    {
        this.paragraphs.Clear();
        this.UpdateVisibleText();
    }

    private void Start()
    {
        if (this.textLabel == (UnityEngine.Object)null)
        {
            this.textLabel = base.GetComponentInChildren<UILabel>();
        }
        if (this.scrollBar != (UnityEngine.Object)null)
        {
            EventDelegate.Add(this.scrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
        }
        this.textLabel.overflowMethod = UILabel.Overflow.ClampContent;
        if (this.style == UITextList.Style.Chat)
        {
            this.textLabel.pivot = UIWidget.Pivot.BottomLeft;
            this.scrollValue = 1f;
        }
        else
        {
            this.textLabel.pivot = UIWidget.Pivot.TopLeft;
            this.scrollValue = 0f;
        }
    }

    private void Update()
    {
        if (this.isValid && (this.textLabel.width != this.mLastWidth || this.textLabel.height != this.mLastHeight))
        {
            this.Rebuild();
        }
    }

    public void OnScroll(Single val)
    {
        Int32 scrollHeight = this.scrollHeight;
        if (scrollHeight != 0)
        {
            val *= this.lineHeight;
            this.scrollValue = this.mScroll - val / (Single)scrollHeight;
        }
    }

    public void OnDrag(Vector2 delta)
    {
        Int32 scrollHeight = this.scrollHeight;
        if (scrollHeight != 0)
        {
            Single num = delta.y / this.lineHeight;
            this.scrollValue = this.mScroll + num / (Single)scrollHeight;
        }
    }

    private void OnScrollBar()
    {
        this.mScroll = UIProgressBar.current.value;
        this.UpdateVisibleText();
    }

    public void Add(String text)
    {
        this.Add(text, true);
    }

    protected void Add(String text, Boolean updateVisible)
    {
        UITextList.Paragraph paragraph;
        if (this.paragraphs.size < this.paragraphHistory)
        {
            paragraph = new UITextList.Paragraph();
        }
        else
        {
            paragraph = this.mParagraphs[0];
            this.mParagraphs.RemoveAt(0);
        }
        paragraph.text = text;
        this.mParagraphs.Add(paragraph);
        this.Rebuild();
    }

    protected void Rebuild()
    {
        if (this.isValid)
        {
            this.mLastWidth = this.textLabel.width;
            this.mLastHeight = this.textLabel.height;
            this.textLabel.UpdateNGUIText();
            NGUIText.rectHeight = 1000000;
            NGUIText.regionHeight = 1000000;
            this.mTotalLines = 0;
            for (Int32 i = 0; i < this.paragraphs.size; i++)
            {
                UITextList.Paragraph paragraph = this.mParagraphs.buffer[i];
                String text;
                NGUIText.WrapText(paragraph.text, out text, false, true);
                paragraph.lines = text.Split(new Char[]
                {
                    '\n'
                });
                this.mTotalLines += (Int32)paragraph.lines.Length;
            }
            this.mTotalLines = 0;
            Int32 j = 0;
            Int32 size = this.mParagraphs.size;
            while (j < size)
            {
                this.mTotalLines += (Int32)this.mParagraphs.buffer[j].lines.Length;
                j++;
            }
            if (this.scrollBar != (UnityEngine.Object)null)
            {
                UIScrollBar uiscrollBar = this.scrollBar as UIScrollBar;
                if (uiscrollBar != (UnityEngine.Object)null)
                {
                    uiscrollBar.barSize = ((this.mTotalLines != 0) ? (1f - (Single)this.scrollHeight / (Single)this.mTotalLines) : 1f);
                }
            }
            this.UpdateVisibleText();
        }
    }

    protected void UpdateVisibleText()
    {
        if (this.isValid)
        {
            if (this.mTotalLines == 0)
            {
                this.textLabel.text = String.Empty;
                return;
            }
            Int32 num = Mathf.FloorToInt((Single)this.textLabel.height / this.lineHeight);
            Int32 num2 = Mathf.Max(0, this.mTotalLines - num);
            Int32 num3 = Mathf.RoundToInt(this.mScroll * (Single)num2);
            if (num3 < 0)
            {
                num3 = 0;
            }
            StringBuilder stringBuilder = new StringBuilder();
            Int32 num4 = 0;
            Int32 size = this.paragraphs.size;
            while (num > 0 && num4 < size)
            {
                UITextList.Paragraph paragraph = this.mParagraphs.buffer[num4];
                Int32 num5 = 0;
                Int32 num6 = (Int32)paragraph.lines.Length;
                while (num > 0 && num5 < num6)
                {
                    String value = paragraph.lines[num5];
                    if (num3 > 0)
                    {
                        num3--;
                    }
                    else
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append("\n");
                        }
                        stringBuilder.Append(value);
                        num--;
                    }
                    num5++;
                }
                num4++;
            }
            this.textLabel.text = stringBuilder.ToString();
        }
    }

    public UILabel textLabel;

    public UIProgressBar scrollBar;

    public UITextList.Style style;

    public Int32 paragraphHistory = 100;

    protected Char[] mSeparator = new Char[]
    {
        '\n'
    };

    protected Single mScroll;

    protected Int32 mTotalLines;

    protected Int32 mLastWidth;

    protected Int32 mLastHeight;

    private BetterList<UITextList.Paragraph> mParagraphs;

    private static Dictionary<String, BetterList<UITextList.Paragraph>> mHistory = new Dictionary<String, BetterList<UITextList.Paragraph>>();

    public enum Style
    {
        Text,
        Chat
    }

    protected class Paragraph
    {
        public String text;

        public String[] lines;
    }
}
