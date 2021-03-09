using System;
using System.Threading;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;
using Covid19.ListviewClass;
using Warkiz.Widgets;
using Android.Media;
using Com.Airbnb.Lottie;
using Android.Util;

namespace Covid19
{ 
    [Activity(MainLauncher = false,ScreenOrientation = ScreenOrientation.Portrait)]
    public class ScanDeviceActivity : AppCompatActivity
{
        static readonly string TAG = typeof(ScanDeviceActivity).FullName;
        // IndicatorSeekBar seekbar;
        TextView txtStatus;
        LottieAnimationView animation_view;
         Switch scan_switch;
        RelativeLayout layoutTop;
        private static ScanDeviceActivity _instance;
        private bool _isReceiveredRegistered;
        private BluetoothDeviceReceiver _receiver;
        private float physical_distance = 1;
        MediaPlayer _player;
        Intent startServiceIntent;
        Intent stopServiceIntent;
        bool isStarted = false;
        
        public static ScanDeviceActivity GetInstance()
        {
            return _instance;
        }
      
        protected override void OnCreate(Bundle savedInstanceState)
    {
        try
        {

            base.OnCreate(savedInstanceState);
            _instance = this;
            SetContentView(Resource.Layout.activity_scandevice);
                const int locationPermissionsRequestCode = 1000;

                var locationPermissions = new[]
                {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };
                // check if the app has permission to access coarse location
                var coarseLocationPermissionGranted =
                   ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);

                // check if the app has permission to access fine location
                var fineLocationPermissionGranted =
                    ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

                // if either is denied permission, request permission from the user
                if (coarseLocationPermissionGranted == Permission.Denied ||
                    fineLocationPermissionGranted == Permission.Denied)
                {
                    ActivityCompat.RequestPermissions(this, locationPermissions, locationPermissionsRequestCode);
                }

                scan_switch = FindViewById<Switch>(Resource.Id.scan_switch);
                layoutTop = FindViewById<RelativeLayout>(Resource.Id.layoutTop);
              //  seekbar =(IndicatorSeekBar)layoutTop.GetChildAt(2);
               // seekbar.StopTrackingTouch += Seekbar_StopTrackingTouch;
               // seekbar.TickMarksColor(Color.ParseColor("#ee6d66"));
               // string[] ticktexts = new string[3] { "1 meter", "1.5 meter", "2 meter" };
               // seekbar.CustomTickTexts(ticktexts);
                _receiver = new BluetoothDeviceReceiver();
                scan_switch.CheckedChange += Scan_switch_CheckedChange;
                _player = MediaPlayer.Create(this, Resource.Raw.smsalert);
                // RegisterBluetoothReceiver();
                //PopulateListView();
                animation_view = FindViewById<LottieAnimationView>(Resource.Id.animation_view);
                txtStatus= FindViewById<TextView>(Resource.Id.txtStatus);

                startServiceIntent = new Intent(this, typeof(BackgroundScanService));
                startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);

                stopServiceIntent = new Intent(this, typeof(BackgroundScanService));
                stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            }
        catch (Exception ex)
        {
        }
    }

        private void Scan_switch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            
            if(e.IsChecked)
            {
                if (BluetoothDeviceReceiver.Adapter.IsEnabled)
                {
                    StartService(startServiceIntent);
                    Log.Info(TAG, "User requested that the service be started.");

                    isStarted = true;
                    animation_view.Visibility = Android.Views.ViewStates.Visible;
                    txtStatus.Visibility = Android.Views.ViewStates.Visible;
                }
                else
                {
                    Toast.MakeText(this, "Please turn on bluetooth to use this app", ToastLength.Long).Show();
                    scan_switch.Checked = false;              }
            }
            else
            {
               
                Log.Info(TAG, "User requested that the service be stopped.");
                StopService(stopServiceIntent);
                isStarted = false;
                animation_view.Visibility = Android.Views.ViewStates.Gone;
                txtStatus.Visibility = Android.Views.ViewStates.Gone;
            }
        }

        private void Seekbar_StopTrackingTouch(object sender, StopTrackingTouchEventArgs e)
        {
            physical_distance = e.SeekBar.ProgressFloat;
            Toast.MakeText(this, physical_distance.ToString(), ToastLength.Long).Show();
        }

       

      
            private void RegisterBluetoothReceiver()
        {
            if (_isReceiveredRegistered) return;

            RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
            _isReceiveredRegistered = true;
        }
        //protected override void OnDestroy()
        //{
        //    base.OnDestroy();

        //    // Make sure we're not doing discovery anymore
        //    CancelScanning();

        //    // Unregister broadcast listeners
        //    UnregisterBluetoothReceiver();
        //}

        //protected override void OnPause()
        //{
        //    base.OnPause();

        //    // Make sure we're not doing discovery anymore
        //    CancelScanning();

        //    // Unregister broadcast listeners
        //    UnregisterBluetoothReceiver();
        //}

        //protected override void OnResume()
        //{
        //    base.OnResume();

        //    StartScanning();

        //    // Register broadcast listeners
        //    RegisterBluetoothReceiver();
        //}

        public void UpdateAdapter(DataItem dataItem)
        {
            var rssiDistance = dataItem.Rssi;
            if (rssiDistance >= -68)
            {
               // isViolated = true;
                Toast.MakeText(this, "Alert! There is a deive with name: " + dataItem.Text + " and address: " + dataItem.SubTitle + " close to you. Please maintain physical distance of atleast 6 feet.", ToastLength.Long).Show();
                _player.Start();
                Vibrator vibrator = (Vibrator)this.GetSystemService(Context.VibratorService);
                vibrator.Vibrate(1000);
            }
            //var rssiDistance = calculateDistance(dataItem.Rssi);

            //if(rssiDistance<= physical_distance)
            //{
            //    Toast.MakeText(this, "Alert! There is a deive with name: "+dataItem.Text+"and address: "+dataItem.SubTitle+"at a distance of "+ rssiDistance +"m. Please maintain physical distance of "+ physical_distance +"m", ToastLength.Long).Show();
            //    _player.Start();
            //}
            //else
            //{

            //}


            //var lst = FindViewById<Android.Widget.ListView>(Resource.Id.lstview);
            // var adapter = lst.Adapter as ListViewAdapter;

            //var items = adapter?.Items.Where(m => m.GetListItemType() == ListItemType.DataItem).ToList();

            //if (items != null && !items.Any(x =>
            //        ((DataItem)x).Text == dataItem.Text && ((DataItem)x).SubTitle == dataItem.SubTitle))
            //{
            //    adapter.Items.Add(dataItem);
            //}

            //lst.Adapter = new ListViewAdapter(this, adapter?.Items);
        }
        private double calculateDistance(float rssi)
        {

            var txPower = -59; //hard coded power value. Usually ranges between -59 to -65


  if (rssi == 0)
            {
                return -1.0;
            }

            var ratio = rssi * 1.0 / txPower;
            if (ratio < 1.0)
            {
                return Math.Pow(ratio, 10);
            }
            else
            {
                var distance = (0.89976) * Math.Pow(ratio, 7.7095) + 0.111;
                return distance;
            }
        }
        //private static void StartScanning()
        //{
        //    if (!BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.StartDiscovery();
        //}

        //private static void CancelScanning()
        //{
        //    if (BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.CancelDiscovery();
        //}
        //private void UnregisterBluetoothReceiver()
        //{
        //    if (!_isReceiveredRegistered) return;

        //    UnregisterReceiver(_receiver);
        //    _isReceiveredRegistered = false;
        //}

        //private void PopulateListView()
        //{
        //    var item = new List<IListItem>
        //    {
        //        new HeaderListItem("PREVIOUSLY PAIRED")
        //    };

        //    item.AddRange(
        //        BluetoothDeviceReceiver.Adapter.BondedDevices.Select(
        //            bluetoothDevice => new DataItem(
        //                bluetoothDevice.Name,
        //                bluetoothDevice.Address
        //            )
        //        )
        //    );

        //    StartScanning();

        //    item.Add(new StatusHeaderListItem("Scanning started..."));

        //    var lst = FindViewById<Android.Widget.ListView>(Resource.Id.lstview);
        //    lst.Adapter = new ListViewAdapter(this, item);
        //}

        public void UpdateAdapterStatus(string discoveryStatus)
        {
            var txtStatus = FindViewById<TextView>(Resource.Id.txtStatus);
            txtStatus.Text = discoveryStatus;
            //var adapter = lst.Adapter as ListViewAdapter;

            //var hasStatusItem = adapter?.Items?.Any(m => m.GetListItemType() == ListItemType.Status);

            //if (hasStatusItem.HasValue && hasStatusItem.Value)
            //{
            //    var statusItem = adapter.Items.Single(m => m.GetListItemType() == ListItemType.Status);
            //    statusItem.Text = discoveryStatus;
            //}

            //lst.Adapter = new ListViewAdapter(this, adapter?.Items);
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(Constants.SERVICE_STARTED_KEY, isStarted);
            base.OnSaveInstanceState(outState);
        }
       
    }
}


//<RelativeLayout
//            android:id="@+id/layoutTop"
//		android:orientation="horizontal"
//		android:layout_width="match_parent"
//		android:layout_height="wrap_content"
//		android:layout_marginTop="15dp"
//		>
		
//		<TextView
//                android:id="@+id/txtFirst"
//			android:text="Set your physical "
//			android:textSize="20dp"
//            android:layout_width="wrap_content"
//			android:layout_height="wrap_content"
//			android:layout_marginLeft="10dp"

//		/>
//		<TextView
//            android:id="@+id/txtSecond"
//			android:layout_below="@id/txtFirst"
//			android:text=" distance limit"
//			android:textSize="20dp"
//            android:layout_width="wrap_content"
//			android:layout_height="wrap_content"
//				android:layout_marginLeft="10dp"
//		/>
//		<com.warkiz.widget.IndicatorSeekBar
//    android:layout_width="match_parent"
//    android:layout_height="wrap_content"
//	android:layout_toRightOf="@id/txtFirst"
//	android:layout_marginTop="20dp"
//    app:isb_max="2"
//    app:isb_min="1"
//    app:isb_progress_value_float="true"
//    app:isb_seek_smoothly="false"
//    app:isb_ticks_count="3"
//    app:isb_show_tick_marks_type="oval"
//    app:isb_tick_marks_size="13dp"
//    app:isb_show_tick_texts="true"
//    app:isb_tick_texts_size="15sp"
//    app:isb_thumb_size="20dp"
//    app:isb_indicator_text_size="15sp"
// app:isb_track_background_size="21dp"
// app:isb_track_progress_size="14dp"
//    app:isb_only_thumb_draggable="false"	
//	  app:isb_tick_texts_color="#FB3258"
//			app:isb_thumb_color="#FB3258"
//			app:isb_indicator_color="#FB3258"
//			 app:isb_track_progress_color="#FB3258"
//		/>
//			<Switch
//android:text="Start Scanning"  
//			android:textSize="20dp"
//			android:layout_below="@id/txtSecond"
//				android:layout_margin="10dp"
//android:layout_width="match_parent"  
//android:layout_height="wrap_content"  
//android:checked="true"  
//android:textOn="YES"  
//android:textOff="NO"  
//android:id="@+id/scan_switch" />  

//	</RelativeLayout>