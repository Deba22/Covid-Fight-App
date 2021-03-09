using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using Covid19.ListviewClass;

namespace Covid19
{
    class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public static BluetoothAdapter Adapter => BluetoothAdapter.DefaultAdapter;

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            // Found a device
            switch (action)
            {
                case BluetoothDevice.ActionFound:
                    // Get the device
                    var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    List<string> scanneddevices = new List<string>();
                    scanneddevices.Add(device.Name);
                  
                    int rssi = intent.GetShortExtra(BluetoothDevice.ExtraRssi, short.MinValue);
                    //Toast.MakeText(context, "  RSSI: " + rssi + "dBm", ToastLength.Long).Show();
                    // Only update the adapter with items which are not bonded
                   // if (device.BondState != Bond.Bonded)
                   // {
                         ScanDeviceActivity.GetInstance().UpdateAdapter(new DataItem(device.Name, device.Address, rssi));
                        //ScanDeviceActivity.GetInstance().UpdateAdapterStatus("Scanning...");
                    //}

                    break;
                case BluetoothAdapter.ActionDiscoveryStarted:
                    ScanDeviceActivity.GetInstance().UpdateAdapterStatus("Scanning...");
                    break;
                case BluetoothAdapter.ActionDiscoveryFinished:
                    Adapter.StartDiscovery();
                   // ScanDeviceActivity.GetInstance().UpdateAdapterStatus("Discovery Finished.");
                    break;
                default:
                    break;
            }
        }
    }
}