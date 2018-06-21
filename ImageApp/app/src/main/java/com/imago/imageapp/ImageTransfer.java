package com.imago.imageapp;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.os.Environment;
import android.support.v4.app.NotificationCompat;
import android.support.v4.app.NotificationManagerCompat;
import android.util.Log;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.util.HashMap;
import java.util.Map;

import javax.xml.transform.Result;

public class ImageTransfer {
    private NotificationManagerCompat notificationManager;
    private NotificationCompat.Builder builder;
    private Context context;

    ImageTransfer(Context context) {
        this.context = context;
    }

    void transferFiles() {
        OnPreExecute();
        try {
            Thread thread = new Thread(new Runnable() {
                Socket socket;
                @Override
                public void run() {
                    try {
                        //create a socket to make the connection with the server
                        InetAddress serverAddr = InetAddress.getByName("10.0.2.2");
                        socket = new Socket(serverAddr, 11000);
                        //sends the message to the server
                        OutputStream output = socket.getOutputStream();
                        Map<String, byte[]> convertedPictures = getPictures();
                        if (convertedPictures == null) return;
                        float inc = 100 / convertedPictures.size();
                        int i = 0;
                        OutputStreamWriter streamWriter = new OutputStreamWriter(output);
                        streamWriter.write("StorePicsCommand\n", 0, "StorePicsCommand\n".length());
                        streamWriter.write(Integer.toString(convertedPictures.keySet().size()).concat("\n"));
                        for (String imgName : convertedPictures.keySet()) {
                            streamWriter.write(imgName.concat("\n"), 0, imgName.concat("\n").length()); // Send name as bytes
                            output.write(convertedPictures.get(imgName).length); // number of bytes in pic
                            output.write(convertedPictures.get(imgName)); //Send bytes of pic
                            output.flush();
                            //Update bar
                            i += inc;
                            builder.setProgress(100, i, false);
                            notificationManager.notify(1, builder.build());
                        }
                    } catch (Exception e) {
                        Log.e("TCP", "ConnectToServer thread: Error", e);
                        System.out.println(e.getMessage());
                    } finally {
                        try {
                            socket.close();
                        } catch (Exception e) {
                            Log.e("TCP", "ConnectToServer thread: Error", e);
                            System.out.println(e.getMessage());
                        }
                    }
                }
            });
            thread.start();
            thread.join();
        } catch (Exception e) {
            Log.e("TCP", "ConnectToServer thread: Error", e);
            System.out.println(e.getMessage());
        }
    }


    private Map<String, byte[]> getPictures() {
        // Getting the Camera Folder
        File dcim =
                Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM);
        if (dcim == null) {
            return null;
        }
        File[] pics = dcim.listFiles();
        Map<String, byte[]> convertedPictures = new HashMap<String, byte[]>();
        String fileName;
        if (pics != null) {
            for (File pic : pics) {
                fileName = pic.getName();
                convertedPictures.put(fileName,convertPicture(pic));
            }
        }
        return convertedPictures;
    }

    byte[] convertPicture(File pic) {
        try {
            FileInputStream fis = new FileInputStream(pic);
            Bitmap bm = BitmapFactory.decodeStream(fis);
            return getBytesFromBitmap(bm);
        } catch(Exception e) {
            System.out.println(e.getMessage());
            return null;
        }

    }

    private byte[] getBytesFromBitmap(Bitmap bitmap) {
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.PNG, 70, stream);
            return stream.toByteArray();
    }

    protected void OnPreExecute() {
        notificationManager = NotificationManagerCompat.from(this.context);
        builder = new NotificationCompat.Builder(this.context, "default");
        builder.setContentTitle("Picture Transfer").setContentText("Transfer in progress")
                .setProgress(100,0,false).setPriority(NotificationCompat.PRIORITY_LOW);
    }

    protected void onPostExecute (Result result) {
        // At the End
        builder.setContentText("Download complete").setProgress(0, 0, false);
        notificationManager.notify(1, builder.build());
    }

}