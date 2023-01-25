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

    public int id;
    bool hasAllImages;

    public static string fileType = ".jpg";

    public static ImageData CreateImageData(int imageID)
    {
        ImageData data = new ImageData();
        data.id = imageID;
        data.hasAllImages = true;
        return data;
    }
    public static ImageData TryGetImageData(int imageID)
    {
        byte[] largeImage = TryGetImage(SaveManager.imageDirectories[0], imageID);
        byte[] smallImage = TryGetImage(SaveManager.imageDirectories[1], imageID);
        byte[] croppedImage = TryGetImage(SaveManager.imageDirectories[2], imageID);


        //If data not found
        if (largeImage == null && smallImage == null && croppedImage == null)
            return null;

        ImageData instance = CreateImageData(imageID);
        instance.SetImages(largeImage, smallImage, croppedImage);

        if (largeImage == null || smallImage == null || croppedImage == null)
            instance.hasAllImages = false;

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

    public void SetImage(ApiCall.ImageTypes imageType, byte[] imageData)
    {
        switch (imageType)
        {
            case ApiCall.ImageTypes.LARGE:
                imageLarge = imageData;
                break;
            case ApiCall.ImageTypes.SMALL:
                imageSmall = imageData;
                break;
            case ApiCall.ImageTypes.CROPPED:
                imageCropped = imageData;
                break;
            default:
                break;
        }
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

    public bool HasImages()
    {
        return hasAllImages;
    }

    public void SaveImages()
    {
        imageLarge.SaveImage(SaveManager.imageDirectories[0], id.ToString(), fileType);
        imageSmall.SaveImage(SaveManager.imageDirectories[1], id.ToString(), fileType);
        imageCropped.SaveImage(SaveManager.imageDirectories[2], id.ToString(), fileType);
    }



}
