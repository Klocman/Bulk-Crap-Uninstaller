/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace Klocman.Subsystems
{
    public sealed class FontGrabber
    {
        private readonly Dictionary<string, FontFamily> _validFontFamilies = new();

        public FontGrabber()
        {
            UpdateFontFamilies();
        }

        public IEnumerable<FontFamily> ValidFontFamilies => _validFontFamilies.Values;
        public IEnumerable<string> ValidFontFamilyNames => _validFontFamilies.Keys;

        public Font GetFont(string familyName, float size, FontStyle style)
        {
            if (size <= 0)
                throw new ArgumentException("Font size must be higher than 0");

            return _validFontFamilies.ContainsKey(familyName)
                ? new Font(_validFontFamilies[familyName], size, style)
                : null;
        }

        public FontFamily GetFontFamily(string familyName)
        {
            return _validFontFamilies.ContainsKey(familyName) ? _validFontFamilies[familyName] : null;
            //throw new ArgumentException("Invalid font family name");
        }

        private void UpdateFontFamilies()
        {
            using (var installedFonts = new InstalledFontCollection())
            {
                foreach (var t in installedFonts.Families.Where(t => t.IsStyleAvailable(FontStyle.Regular)))
                {
                    _validFontFamilies.Add(t.Name, t);
                }
            }
        }
    }
}