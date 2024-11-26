using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] private Material visionConeMaterial;
    [SerializeField] private float visionRange;
    [SerializeField] private float visionAngle;
    [SerializeField] private LayerMask visionObstructingLayer;//layer with objects that obstruct the view.
    [SerializeField] private int visionConeResolution = 120;//the vision cone will be made up of triangles the higher the resolution the more round it'll look.
    [SerializeField] private bool visualizeCone;
    private Mesh visionConeMesh;
    private MeshFilter meshFilter;
    void Start()
    {
        transform.AddComponent<MeshRenderer>().material = visionConeMaterial;
        meshFilter = transform.AddComponent<MeshFilter>();
        visionConeMesh = new Mesh();
        visionAngle *= Mathf.Deg2Rad;
    }
    void Update()
    {
        DrawVisionCone();//calling the vision cone function
    }
    void DrawVisionCone()//this method creates the vision cone mesh
    {
        int[] triangles = new int[(visionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[visionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -visionAngle / 2;
        float angleIcrement = visionAngle / (visionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < visionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, visionRange, visionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * visionRange;
            }

            Currentangle += angleIcrement;
        }
        
        if (visualizeCone)
        {
            for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
            {
                triangles[i] = 0;
                triangles[i + 1] = j + 1;
                triangles[i + 2] = j + 2;
            }
            visionConeMesh.Clear();
            visionConeMesh.vertices = Vertices;
            visionConeMesh.triangles = triangles;
            meshFilter.mesh = visionConeMesh;
        }
        else if(!visualizeCone && visionConeMesh != null)
        {
            visionConeMesh.Clear();
        }
    }
}