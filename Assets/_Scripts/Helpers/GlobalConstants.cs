using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConstants
{
    public static Vector3[] directions = new Vector3[]
    {
        new Vector3(1f, 1f, 1f), 
        new Vector3(1f, 1f, 0f), 
        new Vector3(1f, 1f, -1f), 
        new Vector3(1f, 0f, 1f), 
        new Vector3(1f, 0f, 0f), 
        new Vector3(1f, 0f, -1f), 
        new Vector3(1f, -1f, 1f), 
        new Vector3(1f, -1f, 0f), 
        new Vector3(1f, -1f, -1f), 
        new Vector3(0f, 1f, 1f), 
        new Vector3(0f, 1f, 0f), 
        new Vector3(0f, 1f, -1f), 
        new Vector3(0f, 0f, 1f),
        new Vector3(0f, 0f, -1f), 
        new Vector3(0f, -1f, 1f), 
        new Vector3(0f, -1f, 0f), 
        new Vector3(0f, -1f, -1f), 
        new Vector3(-1f, 1f, 1f), 
        new Vector3(-1f, 1f, 0f), 
        new Vector3(-1f, 1f, -1f), 
        new Vector3(-1f, 0f, 1f), 
        new Vector3(-1f, 0f, 0f), 
        new Vector3(-1f, 0f, -1f), 
        new Vector3(-1f, -1f, 1f), 
        new Vector3(-1f, -1f, 0f), 
        new Vector3(-1f, -1f, -1f), 
    };

    public static string InputError = "Input data is incorrect";
    public static string Generating = "Generating. Please wait";
    public static string Generated = "Claster is generated";
    public static string ClasterExist = "There is claster with such a name";
    public static string ClasterSaved = "Claster is successfully saved";
    public static string ClasterLoaded = "Claster is successfully loaded";
}
