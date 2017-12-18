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
using static Android.Provider.MediaStore;
using Android.Provider;
using Java.IO;
using System.IO;

namespace FWUtils.Droid.Extensions
{
    public static class IOExtension
    {
        public static string Save(this Bitmap bitmap, string Android_OS_Environment, string fileName)
        {
            var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android_OS_Environment);

            //create a file to write bitmap data
            var stream = new FileStream(System.IO.Path.Combine(directory.Path, fileName), FileMode.Create);
            using (stream)
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                stream.Flush();
                stream.Close();
            }

            return System.IO.Path.Combine(directory.Path, fileName);
        }

        public static string SaveAndAddToGallery(this Bitmap bitmap, string Android_OS_Environment, string fileName, Context context)
        {
            var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android_OS_Environment);
            var path = System.IO.Path.Combine(directory.Path, fileName);

            //create a file to write bitmap data
            var stream = new FileStream(path, FileMode.OpenOrCreate);
            using (stream)
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                bitmap.AddImageToGallery(path, context);
                stream.Flush();
                stream.Close();
            }

            return path;
        }

        public static bool AddImageToGallery(this Bitmap bitmap, string filePath, Context context)
        {
            ContentValues values = new ContentValues();
            values.Put(MediaStore.MediaColumns.Data, filePath);
            context.ContentResolver.Insert(Images.Media.ExternalContentUri, values);
            return true;
        }
    }
}