package com.imago.imageapp;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Environment;
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

public class ImageTransfer {

    void transferFiles() {
        try {
            //here you must put your computer's IP address.
            InetAddress serverAddr = InetAddress.getByName("10.0.0.2");
            //create a socket to make the connection with the server
            Socket socket = new Socket(serverAddr, 1234);
            try {
                //sends the message to the server
                OutputStream output = socket.getOutputStream();
                Map<String,byte[]> convertedPictures = getPictures();
                if(convertedPictures == null) return;
                OutputStreamWriter streamWriter = new OutputStreamWriter(output);
                streamWriter.write("StorePicsCommand\n",0, "StorePicsCommand\n".length());
                streamWriter.write(Integer.toString(convertedPictures.keySet().size()).concat("\n"));
                for (String imgName: convertedPictures.keySet()) {
                    streamWriter.write(imgName.concat("\n"),0,imgName.concat("\n").length()); // Send name as bytes
                    output.write(convertedPictures.get(imgName).length); // number of bytes in pic
                    output.write(convertedPictures.get(imgName)); //Send bytes of pic
                    output.flush();
                }

            } catch (Exception e) {
                Log.e("TCP", "S: Error", e);
            } finally {
                socket.close();
            }
        } catch (Exception e) {
            Log.e("TCP", "C: Error", e);
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
}