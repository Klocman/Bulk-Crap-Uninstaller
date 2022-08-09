/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Properties;
using Klocman.Resources;

namespace BulkCrapUninstaller.Functions.Ratings
{
    public class RatingRenderer : BaseRenderer
    {
        private readonly Image[] _coloredImages =
        {
            Resources.rating1, Resources.rating3, Resources.rating4,
            Resources.rating5
        };
        private readonly Image _baseImage = Resources.star;

        private readonly int _maximumValue = 8;
        private readonly int _maxNumberImages = 4;
        private readonly int _minimumValue = -8;

        public override void Render(Graphics g, Rectangle r)
        {
            DrawBackground(g, r);
            r = ApplyCellPadding(r);

            // Convert our aspect to a numeric value
            var aspect = (RatingEntry)Aspect;

            if (aspect.Equals(RatingEntry.NotAvailable))
            {
                DrawText(g, r, Localisable.NotAvailable);
            }
            else if (aspect.AverageRating.HasValue || aspect.MyRating.HasValue)
            {
                var aspectValue = (float)(aspect.MyRating ?? aspect.AverageRating);

                // Calculate how many images we need to draw to represent our aspect value
                int numberOfImages;
                if (aspectValue <= _minimumValue) numberOfImages = 1;
                // ReSharper disable once PossibleLossOfFraction
                else if (aspectValue <= (_maximumValue + _minimumValue) / 2) numberOfImages = 2;
                else if (aspectValue < _maximumValue) numberOfImages = 3;
                else numberOfImages = _maxNumberImages;

                // If we need to shrink the image, what will its on-screen dimensions be?
                var imageScaledWidth = _baseImage.Width;
                var imageScaledHeight = _baseImage.Height;
                if (r.Height < _baseImage.Height)
                {
                    imageScaledWidth = (int)(_baseImage.Width * (float)r.Height / _baseImage.Height);
                    imageScaledHeight = r.Height;
                }
                // Calculate where the images should be drawn
                var imageBounds = r;
                imageBounds.Width = _maxNumberImages * (imageScaledWidth + Spacing) - Spacing;
                imageBounds.Height = imageScaledHeight;
                imageBounds = AlignRectangle(r, imageBounds);

                // Finally, draw the images
                var singleImageRect = new Rectangle(imageBounds.X, imageBounds.Y, imageScaledWidth, imageScaledHeight);
                var backgroundColor = GetBackgroundColor();
                for (var i = 0; i < numberOfImages; i++)
                {
                    if (ListItem.Enabled)
                    {
                        var displayedImage = aspect.MyRating.HasValue || Settings.Default.MiscColorblind ? _baseImage : _coloredImages[numberOfImages - 1];
                        g.DrawImage(displayedImage, singleImageRect);
                        //DrawImage(g, singleImageRect, image);
                    }
                    else
                        ControlPaint.DrawImageDisabled(g, _baseImage, singleImageRect.X, singleImageRect.Y,
                            backgroundColor);
                    singleImageRect.X += (imageScaledWidth + Spacing);
                }
            }
            else
            {
                DrawText(g, r, CommonStrings.Unknown);
            }
        }
    }
}