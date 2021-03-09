using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Covid19
{
    [Service]
    public class BackgroundScanService : Service
    {
        static readonly string TAG = typeof(BackgroundScanService).FullName;

       
        bool isStarted;
        private bool _isReceiveredRegistered;
        private BluetoothDeviceReceiver _receiver;

       
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    Log.Info(TAG, "OnStartCommand: The service is already running.");
                }
                else
                {
                    Log.Info(TAG, "OnStartCommand: The service is starting.");
                    RegisterForegroundService();
                   _receiver = new BluetoothDeviceReceiver();
                    StartScanning();
                    RegisterBluetoothReceiver();
                    isStarted = true;
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                Log.Info(TAG, "OnStartCommand: The service is stopping.");
                CancelScanning();
                UnregisterBluetoothReceiver();
                StopForeground(true);
                StopSelf();
                isStarted = false;

            }
            else if (intent.Action.Equals(Constants.ACTION_RESTART_TIMER))
            {
                Log.Info(TAG, "OnStartCommand: Restarting the timer.");
               

            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            // We need to shut things down.
          //  Log.Debug(TAG, GetFormattedTimestamp() ?? "The TimeStamper has been disposed.");
            Log.Info(TAG, "OnDestroy: The started service is shutting down.");
            
            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(Constants.SERVICE_RUNNING_NOTIFICATION_ID);
            isStarted = false;
            base.OnDestroy();
            // Make sure we're not doing discovery anymore
            CancelScanning();

            // Unregister broadcast listeners
            UnregisterBluetoothReceiver();
        }
        void RegisterForegroundService()
        {
            try
            {

                String NOTIFICATION_CHANNEL_ID = "com.debweb.covidfight";
                String channelName = "covidfight app service";
                NotificationChannel chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationManager.ImportanceNone);
               // chan.LightColor = GetColor(Color.ParseColor("#000"));
                chan.LockscreenVisibility = NotificationVisibility.Private;
                NotificationManager manager = (NotificationManager)GetSystemService(Context.NotificationService);
                //Assert manager != null;
                manager.CreateNotificationChannel(chan);

                NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
                Notification notification = notificationBuilder.SetOngoing(true)
                        .SetSmallIcon(Resource.Drawable.ic_stat_name)
                        .SetContentTitle("App is running in background")
                        .SetPriority(1)
                        .SetCategory(Notification.CategoryService)
                        .Build();
               
                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            catch(Exception ex)
            {

            }
        }
        Notification.Action BuildStopServiceAction()
        {
            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            var builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause,
                                                          GetText(Resource.String.stop_service),
                                                          stopServicePendingIntent);
            return builder.Build();

        }
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(ScanDeviceActivity));
            notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }
        Notification.Action BuildRestartTimerAction()
        {
            var restartTimerIntent = new Intent(this, GetType());
            restartTimerIntent.SetAction(Constants.ACTION_RESTART_TIMER);
            var restartTimerPendingIntent = PendingIntent.GetService(this, 0, restartTimerIntent, 0);

            var builder = new Notification.Action.Builder(Resource.Drawable.ic_action_restart_timer,
                                             new Java.Lang.String("Restart Timer"),
                                              restartTimerPendingIntent);

            return builder.Build();
        }

        private void RegisterBluetoothReceiver()
        {
            if (_isReceiveredRegistered) return;

            RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
            _isReceiveredRegistered = true;
        }
        private static void StartScanning()
        {
            if (!BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.StartDiscovery();
        }

        private static void CancelScanning()
        {
            if (BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.CancelDiscovery();
        }
        private void UnregisterBluetoothReceiver()
        {
            if (!_isReceiveredRegistered) return;

            UnregisterReceiver(_receiver);
            _isReceiveredRegistered = false;
        }
    }
}