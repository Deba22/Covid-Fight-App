using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Covid19
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
       public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.activity_splash);
                // animationView = FindViewById<LottieAnimationView>(Resource.Id.splashanimview);
                // if (animationView != null)
                // {
                //    animationView.PlayAnimation();
                // }

            }
            catch (Exception ex)
            {
            }
        }
        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (animationView == null)
                //{
                //    animationView = FindViewById<LottieAnimationView>(Resource.Id.splashanimview);
                //    animationView.PlayAnimation();
                //}


                Task startupWork = new Task(() => { SimulateStartup(); });
                startupWork.Start();
            }
            catch (Exception ex)
            {

            }

        }
        async void SimulateStartup()
        {
            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
            await Task.Delay(2000); // Simulate a bit of startup work.
            Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
            //  StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            StartActivity(new Intent(Application.Context, typeof(SliderActivity)));
        }

    }
}