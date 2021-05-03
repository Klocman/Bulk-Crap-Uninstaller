/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Media;

namespace Klocman.Extensions
{
    public static class IconExtensions
    {
        /// <summary>
        ///     Useful only with icons from the SystemIcons class. Plays sounds from the SystemSounds class.
        /// </summary>
        /// <param name="iconSet"></param>
        public static void PlayCorrespondingSystemSound(this Icon iconSet)
        {
            if (iconSet.Equals(SystemIcons.Error) || iconSet.Equals(SystemIcons.Hand))
            {
                SystemSounds.Hand.Play();
            }
            else if (iconSet.Equals(SystemIcons.Warning) || iconSet.Equals(SystemIcons.Exclamation))
            {
                SystemSounds.Exclamation.Play();
            }
            else if (iconSet.Equals(SystemIcons.Asterisk) || iconSet.Equals(SystemIcons.Application))
            {
                SystemSounds.Asterisk.Play();
            }
            else if (iconSet.Equals(SystemIcons.Question))
            {
                SystemSounds.Question.Play();
            }
            else if (iconSet.Equals(SystemIcons.Shield))
            {
                SystemSounds.Beep.Play();
            }
        }
    }
}