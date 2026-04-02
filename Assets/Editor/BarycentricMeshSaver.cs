using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BarycentricMeshEditorUtility
{
    [MenuItem("Tools/Generate and Save Barycentric Mesh for Selected Object")]
    public static void GenerateAndSaveBarycentricMesh()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("Выберите объект с MeshFilter.");
            return;
        }

        MeshFilter mf = Selection.activeGameObject.GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogError("На выбранном объекте нет MeshFilter.");
            return;
        }

        Mesh originalMesh = mf.sharedMesh;
        if (originalMesh == null)
        {
            Debug.LogError("MeshFilter не содержит меш.");
            return;
        }

        int[] tris = originalMesh.triangles;
        Vector3[] verts = originalMesh.vertices;

        Vector3[] barycentric = new Vector3[verts.Length];

        for (int i = 0; i < tris.Length; i += 3)
        {
            barycentric[tris[i]] = new Vector3(1, 0, 0);
            barycentric[tris[i + 1]] = new Vector3(0, 1, 0);
            barycentric[tris[i + 2]] = new Vector3(0, 0, 1);
        }

        Mesh newMesh = Object.Instantiate(originalMesh);

        List<Vector3> baryList = new List<Vector3>(barycentric);
        newMesh.SetUVs(2, baryList);

        string path = "Assets/BarycentricMeshes/";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets", "BarycentricMeshes");
        }

        string assetPath = path + Selection.activeGameObject.name + "_barycentric.asset";

        if (AssetDatabase.LoadAssetAtPath<Mesh>(assetPath) != null)
        {
            AssetDatabase.DeleteAsset(assetPath);
        }

        AssetDatabase.CreateAsset(newMesh, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        mf.sharedMesh = newMesh;

        Debug.Log("Barycentric mesh сгенерирован и сохранён: " + assetPath);
    }
}
