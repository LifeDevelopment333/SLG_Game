#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class DocumentationViewer
{
    [MenuItem("SLG/Project Documentation")]
    public static void OpenDocs()
    {
        string path = Application.dataPath + "/../README.md";
        EditorUtility.OpenWithDefaultApp(path);
    }
}
#endif
