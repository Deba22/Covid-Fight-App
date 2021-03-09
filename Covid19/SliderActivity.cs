using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Covid19
{
    [Activity(Label = "SliderActivity", Theme = "@style/MyTheme.Splash", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SliderActivity : AppIntro.AppIntro
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddSlide(SampleSlide.NewInstance(Resource.Layout.Intro1));
            AddSlide(SampleSlide.NewInstance(Resource.Layout.Intro2));
            AddSlide(SampleSlide.NewInstance(Resource.Layout.Intro3));
            AddSlide(SampleSlide.NewInstance(Resource.Layout.Intro4));
           
            SetFlowAnimation();
            SetColorSkipButton(Color.ParseColor("#1729D4"));
            SetColorDoneText(Color.ParseColor("#1729D4"));
            SetNextArrowColor(Color.ParseColor("#1729D4"));
            SetIndicatorColor(Color.ParseColor("#1729D4"), Color.ParseColor("#a39cf4"));        }

   

        public override void OnDonePressed()
        {
            
            StartActivity(new Intent(Application.Context, typeof(ScanDeviceActivity)));
            Finish();
        }
        public override void OnSkipPressed()
        {
            StartActivity(new Intent(Application.Context, typeof(ScanDeviceActivity)));
            Finish();
        }
        public override void OnSlideChanged()
        {

          
        }
    }
}