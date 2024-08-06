using UnityEngine;

public static class Util
{
    public static string GetStringAfterLastSlash(string str)
    {
        int index = str.LastIndexOf('/');
        if (index >= 0)
        {
            return str[(index + 1)..];
        }
        else
        {
            return str;
        }
    }
}
