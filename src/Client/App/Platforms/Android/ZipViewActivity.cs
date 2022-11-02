using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Functionland.FxFiles.Client.Shared.Models;
using android = Android;
using Uri = Android.Net.Uri;

namespace Functionland.FxFiles.Client.App.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
    DataHost = "*",
    DataSchemes = new[] { "file", "content" },
    Categories = new[] { Intent.ActionView, Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataMimeTypes = new[] { "application/zip", "application/x-rar-compressed" })]
public class ZipViewActivity : MainActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var uri = Uri.Parse(Intent.DataString);
        var path = GetActualPathFromFile(uri);
        var intentHolder = MauiApplication.Current.Services.GetRequiredService<IntentHolder>();
        intentHolder.FileUrl = path;
    }
    private string? GetActualPathFromFile(Uri uri)
    {
        bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

        if (isKitKat && DocumentsContract.IsDocumentUri(this, uri))
        {
            // ExternalStorageProvider
            if (IsExternalStorageDocument(uri))
            {
                string? docId = DocumentsContract.GetDocumentId(uri);

                char[] chars = { ':' };
                string[]? split = docId?.Split(chars);
                string? type = split?[0];

                if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    return android.OS.Environment.ExternalStorageDirectory + "/" + split?[1];
                }
            }
            // DownloadsProvider
            else if (IsDownloadsDocument(uri))
            {
                string? id = DocumentsContract.GetDocumentId(uri);

                if (id == null) return null;

                Uri? contentUri = ContentUris.WithAppendedId(
                               Uri.Parse("content://downloads/public_downloads")!, long.Parse(id));

                //System.Diagnostics.Debug.WriteLine(contentUri.ToString());

                return GetDataColumn(this, contentUri, null, null);
            }
            // MediaProvider
            else if (IsMediaDocument(uri))
            {
                string? docId = DocumentsContract.GetDocumentId(uri);

                if (docId == null) return null;

                char[] chars = { ':' };
                string[] split = docId.Split(chars);

                String type = split[0];

                Uri? contentUri = null;
                if ("image".Equals(type))
                {
                    contentUri = MediaStore.Images.Media.ExternalContentUri;
                }
                else if ("video".Equals(type))
                {
                    contentUri = MediaStore.Video.Media.ExternalContentUri;
                }
                else if ("audio".Equals(type))
                {
                    contentUri = MediaStore.Audio.Media.ExternalContentUri;
                }

                if (contentUri == null) return null;

                string selection = "_id=?";
                string[] selectionArgs = new string[]
                {
                    split[1]
                };

                return GetDataColumn(this, contentUri, selection, selectionArgs);
            }
        }
        // MediaStore (and general)
        else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
        {

            // Return the remote address
            if (IsGooglePhotosUri(uri))
                return uri.LastPathSegment;

            return GetDataColumn(this, uri, null, null);
        }
        // File
        else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return uri.Path;
        }

        return null;
    }

    public static string? GetDataColumn(Context context, Uri uri, string selection, string[] selectionArgs)
    {
        ICursor? cursor = null;
        string column = "_data";
        string[] projection = { column };

        try
        {
            cursor = context.ContentResolver?.Query(uri, projection, selection, selectionArgs, null);
            if (cursor != null && cursor.MoveToFirst())
            {
                int index = cursor.GetColumnIndexOrThrow(column);
                return cursor.GetString(index);
            }
        }
        finally
        {
            if (cursor != null)
                cursor.Close();
        }
        return null;
    }

    //Whether the Uri authority is ExternalStorageProvider.
    public static bool IsExternalStorageDocument(Uri uri) => "com.android.externalstorage.documents".Equals(uri.Authority);

    //Whether the Uri authority is DownloadsProvider.
    public static bool IsDownloadsDocument(Uri uri) => "com.android.providers.downloads.documents".Equals(uri.Authority);

    //Whether the Uri authority is MediaProvider.
    public static bool IsMediaDocument(Uri uri) => "com.android.providers.media.documents".Equals(uri.Authority);

    //Whether the Uri authority is Google Photos.
    public static bool IsGooglePhotosUri(Uri uri) => "com.google.android.apps.photos.content".Equals(uri.Authority);
}