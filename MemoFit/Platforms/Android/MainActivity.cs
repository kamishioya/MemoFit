using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;
using MemoFit.Services;

namespace MemoFit
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // エッジ・ツー・エッジ有効化
            WindowCompat.SetDecorFitsSystemWindows(Window!, false);

            // ステータスバーの高さを取得して SafeAreaService に設定
            ViewCompat.SetOnApplyWindowInsetsListener(Window!.DecorView, new WindowInsetsCallback());
        }

        private class WindowInsetsCallback : Java.Lang.Object, IOnApplyWindowInsetsListener
        {
            public WindowInsetsCompat? OnApplyWindowInsets(Android.Views.View? v, WindowInsetsCompat? insets)
            {
                if (v == null || insets == null) return insets;

                var statusBarInsets = insets.GetInsets(WindowInsetsCompat.Type.StatusBars());
                float density = v.Resources?.DisplayMetrics?.Density ?? 1f;
                // 物理 px → CSS px (= dp)
                float statusBarHeightCssPx = (statusBarInsets?.Top ?? 0) / density;

                var service = IPlatformApplication.Current?.Services.GetService<SafeAreaService>();
                if (service != null)
                    service.StatusBarHeight = statusBarHeightCssPx;

                return ViewCompat.OnApplyWindowInsets(v, insets);
            }
        }
    }
}

