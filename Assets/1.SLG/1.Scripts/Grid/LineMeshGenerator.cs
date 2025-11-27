using UnityEngine;

public static class LineMeshGenerator
{
    public static void AddLine(Vector3 start, Vector3 end, float width,
                               ref int index, Vector3[] verts, int[] tris)
    {
        Vector3 dir = (end - start).normalized;
        Vector3 normal = Vector3.up;
        Vector3 side = Vector3.Cross(normal, dir) * (width * 0.5f);

        int v = index * 4;
        int t = index * 6;

        verts[v + 0] = start - side;
        verts[v + 1] = start + side;
        verts[v + 2] = end - side;
        verts[v + 3] = end + side;

        tris[t + 0] = v + 0;
        tris[t + 1] = v + 1;
        tris[t + 2] = v + 2;

        tris[t + 3] = v + 2;
        tris[t + 4] = v + 1;
        tris[t + 5] = v + 3;

        index++;
    }
}
