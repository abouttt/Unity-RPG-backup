using UnityEngine;

public static class Extension
{
    public static string GetStringAfterLastSlash(this string str)
    {
        return Util.GetStringAfterLastSlash(str);
    }
}
