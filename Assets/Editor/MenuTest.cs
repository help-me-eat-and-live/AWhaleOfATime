using UnityEditor;
using UnityEngine;

public class MenuTest
{
    [MenuItem("Tools/Debug Menu Test")]
    public static void RunTest()
    {
        EditorUtility.DisplayDialog("Menu Test", "✅ Unity menu is working!", "OK");
    }
}
