using UnityEngine;

namespace Wiggle.Global
{
    public static class HapticManager
    {
        public static void ShortVibrate(long milliseconds = 20, int amplitude = 120)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (PerformAndroidHapticFeedback()) return;

            VibrateAndroid(milliseconds, amplitude);
#elif UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static bool PerformAndroidHapticFeedback()
        {
            try
            {
                using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
                using AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView");
                using AndroidJavaClass hapticConstants = new AndroidJavaClass("android.view.HapticFeedbackConstants");

                int keyboardTap = hapticConstants.GetStatic<int>("KEYBOARD_TAP");
                return decorView.Call<bool>("performHapticFeedback", keyboardTap);
            }
            catch
            {
                return false;
            }
        }

        private static void VibrateAndroid(long milliseconds, int amplitude)
        {
            try
            {
                using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                using AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");
                string vibratorService = contextClass.GetStatic<string>("VIBRATOR_SERVICE");

                using AndroidJavaObject vibrator = context.Call<AndroidJavaObject>("getSystemService", vibratorService);

                if (vibrator == null) return;

                using AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt = buildVersion.GetStatic<int>("SDK_INT");

                if (sdkInt >= 26)
                {
                    using AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");

                    using AndroidJavaObject effect =
                        vibrationEffectClass.CallStatic<AndroidJavaObject>(
                            "createOneShot",
                            milliseconds,
                            Mathf.Clamp(amplitude, 1, 255)
                        );

                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    vibrator.Call("vibrate", milliseconds);
                }
            }
            catch
            {
                Handheld.Vibrate();
            }
        }
#endif
    }
}
