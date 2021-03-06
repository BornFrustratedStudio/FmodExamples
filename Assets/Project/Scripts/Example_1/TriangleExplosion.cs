﻿using System.Collections;
using UnityEngine;


public class TriangleExplosion : MonoBehaviour
{
    [SerializeField]
    private float m_minForce = 150;
    [SerializeField]
    private float m_maxForce = 300;

    public IEnumerator SplitMesh(bool destroy)
    {
        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
            //GetComponent<Collider>().isTrigger = true;
        }

        Mesh M = new Mesh();
        if (GetComponent<MeshFilter>())
        {
            M = GetComponent<MeshFilter>().mesh;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if (GetComponent<MeshRenderer>())
        {
            materials = GetComponent<MeshRenderer>().materials;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;

        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            int[] indices = M.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];
                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                GameObject GO = new GameObject("Triangle " + (i / 3));
                GO.layer = LayerMask.NameToLayer("Default");
                ///Set Position
                GO.transform.position = transform.position;
                ///Set Scale
                GO.transform.localScale = new Vector3(transform.localScale.x/10, transform.localScale.y/10, transform.localScale.z / 10);
                ///Set Rotation
                GO.transform.rotation = transform.rotation;
                ///Give render material
                GO.AddComponent<MeshRenderer>().material = materials[submesh];
                ///Add mesh filter
                GO.AddComponent<MeshFilter>().mesh = mesh;
                ///Calculate emission position
                Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
                ///Add rigidbody and apply explosive force
                GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(m_minForce, m_maxForce), explosionPos, 5);

                GO.AddComponent<BoxCollider>();

                ///Destroy this pices after time
                Destroy(GO, 5 + Random.Range(0.0f, 5.0f));

            }
        }
        GetComponent<Renderer>().enabled = false;

        yield return new WaitForSeconds(0.3f);
        if (destroy == true)
        {
            Destroy(gameObject);
        }

    }


}