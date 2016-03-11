using UnityEngine;
using UnityEditor;
using System.Collections;

public class ScriptEditor: EditorWindow{

    [MenuItem("Window/ScriptEditor")]
    static void init()
    {
        ScriptEditorDebughelpers.openScriptEditor();
    }
    void OnGUI()
    {
        int max = 10;
        for (int i = 0; i < max; ++i)
        {

            float c = Mathf.Sin(((i) / (float)max) * 3.14159f * 2);
            float d = Mathf.Sin(((i + 1) / (float)max) * 3.14159f * 2);

            float e = Mathf.Cos(((i) / (float)max) * 3.14159f * 2);
            float f = Mathf.Cos(((i + 1) / (float)max) * 3.14159f * 2);

            Vector2 va = new Vector2(300 + e * 200, c * 200 + 300), vb = new Vector2(300 + f * 200, d * 200 + 300);
            Vector2 vat = new Vector2(300 + e * 150, c * 150 + 300), vbt = new Vector2(300 + f * 150, d * 150 + 300);
            Drawing.bezierLine(va + new Vector2(3, 3), vat + new Vector2(3, 3), vb + new Vector2(3, 3), vbt + new Vector2(3, 3), new Color(0.4f, 0.4f, 0.5f), 3, true, 40);
        }
        for (int i = 0; i < max; ++i)
        {

            float c = Mathf.Sin(((i) / (float)max) * 3.14159f * 2);
            float d = Mathf.Sin(((i+1) / (float)max) * 3.14159f * 2);

            float e = Mathf.Cos(((i) / (float)max) * 3.14159f * 2);
            float f = Mathf.Cos(((i + 1) / (float)max) * 3.14159f * 2);

            Vector2 va = new Vector2(300 + e * 200, c * 200 + 300), vb = new Vector2(300 + f * 200, d * 200 + 300);
            Vector2 vat = new Vector2(300 + e * 150, c * 150 + 300), vbt = new Vector2(300 + f * 150, d * 150 + 300);
            Drawing.bezierLine(va, vat, vb, vbt, Color.black, 2, true, 40);
        }
    }
}
