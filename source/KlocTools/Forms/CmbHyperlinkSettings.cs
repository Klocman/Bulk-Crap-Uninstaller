/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace Klocman.Forms
{
    public sealed class CmbHyperlinkSettings
    {
        public CmbHyperlinkSettings(string text, Action clickAction)
        {
            Text = text;
            ClickAction = clickAction;
        }

        public Action ClickAction { get; private set; }
        public string Text { get; private set; }
    }
}