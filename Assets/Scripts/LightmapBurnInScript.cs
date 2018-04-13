using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightmapBurnInScript : MonoBehaviour {

    public Material highlightableMaterial;
    public List<MeshFilter> theMeshes = new List<MeshFilter>();

    // Use this for initialization
    void Awake () {
        SetRenderingOrder();
        FindMeshGOs();
    }
	
    /// <summary>
    /// Sort renderers by material/mesh
    /// </summary>
    private void SetRenderingOrder ()
    {
        Renderer[] renderers = FindObjectsOfType(typeof(Renderer)) as Renderer[];
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer rend = renderers[i];
            int id = 0;
            if (rend != null && rend.sharedMaterial != null)
            {
                int materialID = rend.sharedMaterial.GetInstanceID();
                id = 0xFF & materialID;
            }
            rend.sortingOrder = id;
        }
    }

    private void FindMeshGOs ()
    {
        
        foreach (var go in FindObjectsOfType<GameObject>())
        {
            MeshFilter meshComponent = go.GetComponent<MeshFilter>();
            if (meshComponent != null)
            {
                if (meshComponent.GetComponent<Renderer>().sharedMaterial == highlightableMaterial)
                {
                    theMeshes.Add(meshComponent);
                }
            }
        }
        //BURN!
        BurnInLightmapUV(theMeshes);
    }

    private void BurnInLightmapUV (List<MeshFilter> meshes)
    {
        for (int i = 0; i < meshes.Count; i++)
        {
            Renderer rend = meshes[i].GetComponent<Renderer>();
            if (rend != null)
            {
                Vector4 lightmapScaleOffset = rend.lightmapScaleOffset;
                print(lightmapScaleOffset.x);
                if (lightmapScaleOffset != Vector4.zero)
                {
                    Mesh mesh = meshes[i].sharedMesh;
                    Vector2[] uvs = mesh.uv2;
                    for (int j = 0; j < uvs.Length; j++)
                    {
                        Vector2 uv = uvs[j];
                        uv.x *= lightmapScaleOffset.x;
                        uv.y *= lightmapScaleOffset.y;
                        uv.x += lightmapScaleOffset.z;
                        uv.y += lightmapScaleOffset.w;
                        uvs[j] = uv;
                    }
                    mesh.uv2 = uvs;
                    meshes[i].sharedMesh = mesh;
                    rend.lightmapScaleOffset = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);
                }
            }
        }
    }
}
