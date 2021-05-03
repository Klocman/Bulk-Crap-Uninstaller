/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace Klocman.Forms
{
    public sealed class CmbCheckboxSettings
    {
        public CmbCheckboxSettings(string text)
            : this(text, false)
        {
        }

        public CmbCheckboxSettings(string text, bool initialState)
        {
            Text = text;
            InitialState = initialState;
            Result = null;
        }

        public bool DisableLeftButton { get; set; }
        public bool DisableMiddleButton { get; set; }
        public bool DisableRightButton { get; set; }
        public bool InitialState { get; }

        /// <summary>
        ///     Resulting value of the checkbox. Null if not yet set.
        /// </summary>
        public bool? Result { get; internal set; }

        public string Text { get; }
    }
}