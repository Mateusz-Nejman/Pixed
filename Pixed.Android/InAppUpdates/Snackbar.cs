using Android.App;
using Android.Widget;
using Google.Android.Material.Snackbar;
using System;

namespace Pixed.Android.InAppUpdates;

public static class Snackbar
{
    public static void Show(
        Activity activity,
        string text,
        string actionText,
        Action<global::Android.Views.View> clickHandler)
    {
        var message = $"{text} - {actionText}";

        try
        {
            var view = activity.Window?.DecorView;
            if (view is null)
            {
                FallbackToToastWithAction(activity, message, clickHandler, null);
                return;
            }

            var snackbar = Google.Android.Material.Snackbar.Snackbar.Make(view, text, BaseTransientBottomBar.LengthIndefinite);
            if (snackbar is null)
            {
                FallbackToToastWithAction(activity, message, clickHandler, view);
                return;
            }

            snackbar.SetAction(text: actionText, clickHandler: clickHandler);
            snackbar.Show();
        }
        catch (Exception ex) when (IsThemeRelated(ex))
        {
            HandleSnackbarFailure(activity, message, clickHandler, activity.Window?.DecorView);
        }
        catch (Exception)
        {
            HandleSnackbarFailure(activity, message, null, null);
        }
    }

    private static void FallbackToToastWithAction(
        Activity activity,
        string message,
        Action<global::Android.Views.View>? clickHandler,
        global::Android.Views.View? fallbackView)
    {
        Toast.MakeText(activity, message, ToastLength.Short)?.Show();

        if (clickHandler is not null && fallbackView is not null)
        {
            try
            {
                clickHandler(fallbackView);
            }
            catch (Exception)
            {
            }
        }
    }

    private static void HandleSnackbarFailure(
        Activity activity,
        string fallbackMessage,
        Action<global::Android.Views.View>? clickHandler,
        global::Android.Views.View? view)
    {
        Toast.MakeText(activity, fallbackMessage, ToastLength.Short)?.Show();

        if (clickHandler is not null && view is not null)
        {
            try
            {
                clickHandler(view);
            }
            catch (Exception)
            {
            }
        }
    }

    private static bool IsThemeRelated(Exception ex) =>
        ex is global::Android.Views.InflateException ||
        (ex is Java.Lang.UnsupportedOperationException &&
         ex.Message?.Contains("Failed to resolve attribute", StringComparison.Ordinal) == true);
}