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
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileFilter;
import java.io.FileInputStream;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
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
        try {
            Thread thread = new Thread(new Runnable() {
                Socket socket;

                @Override
                public void run() {
                    try {
                        //create a socket to make the connection with the server
                        InetAddress serverAddr = InetAddress.getByName("10.0.2.2");
                        socket = new Socket(serverAddr, 12000);
                        OutputStream output = socket.getOutputStream();
                        Map<String, byte[]> convertedPictures = getPictures();
                        if (convertedPictures.isEmpty()) return;
                        OnPreExecute();
                        float inc = 100 / convertedPictures.size();
                        int i = 0;
                        DataOutputStream stream = new DataOutputStream(output);

                        stream.writeInt(Integer.reverseBytes(convertedPictures.keySet().size())); // send number of pics
                        stream.flush();
                        for (String imgName : convertedPictures.keySet()) {
                            stream.writeInt(Integer.reverseBytes(imgName.getBytes().length)); // send number of bytes in name
                            stream.flush();
                            stream.write(imgName.getBytes()); // send name ' crashes
                            stream.flush();
                            stream.writeInt(Integer.reverseBytes(convertedPictures.get(imgName).length)); // number of bytes in pic
                            stream.flush();
                            stream.write(convertedPictures.get(imgName)); //Send bytes of pic
                            stream.flush();
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
        File dcim = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM);
        if (dcim == null) return null;

        List<File> pics = searchPictures(dcim.getAbsolutePath());
        if (pics.isEmpty()) return null;
        Map<String, byte[]> convertedPictures = new HashMap<String, byte[]>();
        String fileName;
        for (File pic : pics) {
                fileName = pic.getName();
                convertedPictures.put(fileName,convertPicture(pic));
        }
        return convertedPictures;
    }

    /**
     * searches recursively for images in the given directory path (directoryName )
     * @param directoryName directory path
     */
    public List<File> searchPictures(String directoryName) {
        File directory = new File(directoryName);
        // Get all the files from a directory.
        File[] files = directory.listFiles(new FileFilter() {
            @Override
            public boolean accept(File file) {
                return true;
            }
        });
        ArrayList<File> fileList = new ArrayList<File>();
        for (File file : files) {
            if (file.isFile() && isValidImage(file.getName())) {
                fileList.add(file);
            } else if (file.isDirectory()) {
                fileList.addAll(searchPictures(file.getAbsolutePath()));
            }
        }
        return fileList;
    }

    /**
     * checks if a given file is an image.
     * @param img file's name
     * @return true if the file is an image, false o.w
     */
    private boolean isValidImage(String img) {
        List<String> suffix = new ArrayList<String>();
        suffix.add(".jpg");
        suffix.add(".png");
        suffix.add(".gif");
        suffix.add(".bmp");
        suffix.add(".jpeg");
        for(String suf : suffix) {
            if (img.endsWith(suf))
                return true;
        }
        return false;
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
        builder.setSmallIcon(R.drawable.ic_launcher_background);
    }

    protected void onPostExecute (Result result) {
        // At the End
        builder.setContentText("Download complete").setProgress(0, 0, false);
        notificationManager.notify(1, builder.build());
    }

}