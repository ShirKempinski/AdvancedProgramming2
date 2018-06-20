package com.imago.imageapp;

import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;
import android.os.IBinder;
import android.support.annotation.Nullable;
import android.widget.Toast;

public class ImageServiceService extends Service {

    private BroadcastReceiver receiver;

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        super.onCreate();
    }


    public int onStartCommand(Intent intent, int
            flag, int startId)
    {
        Toast.makeText(this,"Service starting...",
                Toast.LENGTH_SHORT).show();
        startBroadcastReceiver();
        return START_STICKY;
    }


    public void onDestroy() {
        Toast.makeText(this,"Service ending...",
                Toast.LENGTH_SHORT).show();
    }

    public void startBroadcastReceiver() {
        final IntentFilter theFilter = new IntentFilter();
        theFilter.addAction("android.net.wifi.supplicant.CONNECTION_CHANGE");
        theFilter.addAction("android.net.wifi.STATE_CHANGE");
        this.receiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                WifiManager wifiManager = (WifiManager) context
                        .getSystemService(Context.WIFI_SERVICE);
                NetworkInfo networkInfo = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
                if (networkInfo != null) {
                    if (networkInfo.getType() == ConnectivityManager.TYPE_WIFI) {
            //get the different network states
                                    if (networkInfo.getState() == NetworkInfo.State.CONNECTED) {
                                        startTransfer();
            // Starting the Transfer
                                    }
                                }
                            }
                        }
                    };
            // Registers the receiver so that your service will listen for
            // broadcasts
        this.registerReceiver(this.receiver, theFilter);
    }

    private void startTransfer() {
        ImageTransfer transfer = new ImageTransfer();
        transfer.transferFiles();
    }
}
