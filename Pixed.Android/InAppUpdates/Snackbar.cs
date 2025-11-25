using Google.Android.Material.Snackbar;

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
            clickHandler.Invoke(fallbackView);
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
            clickHandler.Invoke(view);
        }
    }

    private static bool IsThemeRelated(Exception ex) =>
        ex is global::Android.Views.InflateException ||
        (ex is Java.Lang.UnsupportedOperationException &&
         ex.Message.Contains("Failed to resolve attribute", StringComparison.Ordinal));
}