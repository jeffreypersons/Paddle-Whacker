using System;


public static class CollectionUtils
{
    public static string[] PrependToArray(string value, string[] source)
    {
        string[] newArray = new string[source.Length + 1];
        newArray[0] = value;
        Array.Copy(source, 0, newArray, 1, source.Length);
        return newArray;
    }
}
