using UnityEngine;

// Some functions that could help me debug!

class HelpfulFunctions
{
    public static void printStringArray(string[] array)
    {
        if (array.Length > 0)
        {
            string stringOfArray = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                stringOfArray += ", " + array[i];
            }
            Debug.Log(stringOfArray);
        }
    }


    public static string arrayToString(string[] array)
    {
        if (array.Length > 0)
        {
            string stringOfArray = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                stringOfArray += ", " + array[i];
            }
            return stringOfArray;
        }
        else
        {
            return "[]";
        }
    }
}