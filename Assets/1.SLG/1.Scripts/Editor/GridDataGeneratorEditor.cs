using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridDataGenerator))]
public class GridDataGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기존 인스펙터 창
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("=== Grid Generate Tool ===", EditorStyles.boldLabel);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;
        buttonStyle.fixedHeight = 30;

        if(GUILayout.Button(" Generate Grid ", buttonStyle))
        {
            GridDataGenerator generator = (GridDataGenerator)target;

            generator.GenerateGrid();
            Debug.Log(" 그리드 생성 ");

            SceneView.RepaintAll();
        }
    }
}
