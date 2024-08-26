/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 23:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Data;
using System.Reflection;

namespace QuickZip.MiniHtml2
{


    /// <summary>
    /// Description of Updater.
    /// </summary>
    public class HtmlUpdater
    {
        TextBlock textBlock;
        CurrentStateType currentState = new CurrentStateType();

        private void UpdateStyle(HtmlTag aTag)
        {
            currentState.UpdateStyle(aTag);
        }

        private Inline UpdateElement(HtmlTag aTag)
        {
            Inline retVal = null;

            switch (aTag.Name)
            {
                case "binding":
                case "text":
                    if (aTag.Name == "binding")
                    {
                        retVal = new Bold(new Run("{Binding}"));
                        if (aTag.Contains("path") && (textBlock.DataContext != null))
                        {
                            object obj = textBlock.DataContext;
                            PropertyInfo pi = obj.GetType().GetProperty(aTag["path"]);
                            if (pi != null && pi.CanRead)
                                retVal = new Run(pi.GetValue(obj, null).ToString());
                        }                        
                    }
                    else
                        retVal = new Run(aTag["value"]);

                    if (currentState.SubScript) retVal.SetValue(Typography.VariantsProperty, FontVariants.Subscript);
                    if (currentState.SuperScript) retVal.SetValue(Typography.VariantsProperty, FontVariants.Superscript);
                    if (currentState.Bold) retVal = new Bold(retVal);
                    if (currentState.Italic) retVal = new Italic(retVal);
                    if (currentState.Underline) retVal = new Underline(retVal);

                    if (currentState.Foreground.HasValue)
                        retVal.Foreground = new SolidColorBrush(currentState.Foreground.Value);

                    if (currentState.Font != null)
                        try { retVal.FontFamily = new FontFamily(currentState.Font); }
                        catch { } //Font name not found...

                    if (currentState.FontSize.HasValue)
                        retVal.FontSize = currentState.FontSize.Value;

                    break;
                case "br":
                    retVal = new LineBreak();
                    break;
                default:
                    Debug.WriteLine("UpdateElement - " + aTag.Name + " not handled.");
                    retVal = new Run();
                    break;
            }


            if (currentState.HyperLink != null && currentState.HyperLink != "")
            {
                Hyperlink link = new Hyperlink(retVal);
                try
                {
                    link.NavigateUri = new Uri(currentState.HyperLink);
                }
                catch
                {
                    link.NavigateUri = null;
                }
                retVal = link;
            }

            return retVal;
        }

        public HtmlUpdater(TextBlock aBlock)
        {
            textBlock = aBlock;
        }

        public void Update(HtmlTagTree tagTree)
        {
            List<HtmlTag> tagList = tagTree.ToHtmlTagList();

            foreach (HtmlTag tag in tagList)
            {
                switch (Defines.BuiltinTags[tag.ID].flags)
                {
                    case HTMLFlag.TextFormat: UpdateStyle(tag); break;
                    case HTMLFlag.Element: textBlock.Inlines.Add(UpdateElement(tag)); break;
                }

            }
        }
    }
}
