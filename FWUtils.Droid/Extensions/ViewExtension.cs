using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace FWUtils.Droid.Extensions
{
    public static class ViewExtension
    {
        /// <summary>
        /// Converts a view to Bitmap
        /// </summary>
        /// <param name="view"></param>
        /// <param name="width">Width of the bitmap</param>
        /// <param name="height">Height of the bitmap</param>
        /// <returns>Bitmap image</returns>
        /// <remarks>Source: https://stackoverflow.com/questions/5536066/convert-view-to-bitmap-on-android </remarks>
        public static Bitmap ToImage(this View view, int width = -1, int height = -1)
        {
            if (width == -1)
                width = view.Width;
            if (height == -1)
                height = view.Height;

            //Define a bitmap with the same size as the view
            Bitmap returnedBitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            //Bind a canvas to it
            Canvas canvas = new Canvas(returnedBitmap);
            //Get the view's background
            Drawable bgDrawable = view.Background;
            if (bgDrawable != null)
                //has background drawable, then draw it on the canvas
                bgDrawable.Draw(canvas);
            else
                //does not have background drawable, then draw white background on the canvas
                canvas.DrawColor(Color.White);

            // draw the view on the canvas
            view.Draw(canvas);

            //return the bitmap
            return returnedBitmap;
        }


    }
}