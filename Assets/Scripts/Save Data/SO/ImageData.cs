using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Build.Content;
using UnityEngine;

public class ImageData
{
    byte[] imageLarge;    
    byte[] imageSmall;    
    byte[] imageCropped;  
    
    static string fileType = ".jpg";

    public static ImageData CreateImageData(int imageID, byte[] largeImage, byte[] smallImage, byte[] croppedImage)
    {
        ImageData data = new ImageData();

        data.SetImages(largeImage, smallImage, croppedImage);
        data.SaveImages(imageID.ToString());


        return data;
    }
    public static ImageData TryGetImageData(int imageID)
    {
        ImageData instance = new ImageData();

        byte[] largeImage = TryGetImage(SaveManager.imageDirectories[0], imageID);
        byte[] smallImage = TryGetImage(SaveManager.imageDirectories[1], imageID);
        byte[] croppedImage = TryGetImage(SaveManager.imageDirectories[2], imageID);


        //If data not found
        if (largeImage == null || smallImage == null || croppedImage == null)
            return null;

        instance.SetImages(largeImage, smallImage, croppedImage);

        return instance;
    }

    private static byte[] TryGetImage(string directory, int imageID)
    {
        string path = directory + imageID + fileType;

        if (!File.Exists(path))
            return null;

        byte[] image = File.ReadAllBytes(path);
        return image;

    }


    public void SetImages(byte[] large, byte[] small, byte[] cropped)
    {
        imageLarge = large;
        imageSmall = small;
        imageCropped = cropped;
    }

    public byte[] GetLargeImage()
    {
        return imageLarge;
    }

    public byte[] GetSmallImage()
    {
        return imageSmall;
    }

    public byte[] GetCroppedImage()
    {
        return imageCropped;
    }

    public void SaveImages(string fileName)
    {
        imageLarge.SaveImage(SaveManager.imageDirectories[0], fileName, fileType);
        imageSmall.SaveImage(SaveManager.imageDirectories[1], fileName, fileType);
        imageCropped.SaveImage(SaveManager.imageDirectories[2], fileName, fileType);
    }



}
