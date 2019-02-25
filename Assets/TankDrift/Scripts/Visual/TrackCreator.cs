using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreator : MonoBehaviour {

    public float pDiatance;
    public float segLength;
    public float maxTrackLength;
    public float depth;
    public Material meshCapMat;
    public Material meshMat;
    public Texture2D terrainTex;
    public Vector2 limit;

    RenderTexture rt;

    [SerializeField]
    GameObject meshObj;
    [SerializeField]
    Mesh mesh;
    MeshFilter mf;
    MeshRenderer mr;

    [SerializeField]
    GameObject meshObjCap;
    [SerializeField]
    Mesh meshCap;
    MeshFilter mfCap;
    MeshRenderer mrCap;
    Vector3 lastP1 = Vector3.zero;
    Vector3 lastP2 = Vector3.zero;
    float tracklength = 0;
    Status status = Status.Backward;

    private void Start()
    {
        //CreateNewTrack();
        //terrainTex = meshMat.GetTexture("_TerrainMap") as Texture2D;
        if (terrainTex)
        {
            rt = new RenderTexture(terrainTex.width, terrainTex.height, 0);
            rt.wrapMode = TextureWrapMode.Repeat;
            rt.useMipMap = true;
            rt.Create();
            Graphics.Blit(terrainTex, rt);
            meshMat.SetTexture("_TerrainMap", rt);
            var terrainWidth = FindObjectsOfType<Terrain>()[0].terrainData.size.x;
            meshMat.SetTextureScale("_TerrainMap", new Vector2(1 / terrainWidth, 1 / terrainWidth));
            //Debug.Log(terrainWidth);
        }
    }

    private void CreateNewTrack()
    {
        if (mesh && mesh.triangles.Length == 0)
        {
            Destroy(meshObj);
            Destroy(meshObjCap);
        }

        var P1 = getP1();
        var P1s = getP1s();
        var P2s = getP2s();
        var P2 = getP2();
        var P1H = getH(P1);
        var P2H = getH(P2);


        meshObjCap = new GameObject("TrackCap");
        meshObjCap.transform.position = transform.position;
        meshObjCap.transform.parent = RelativeSystemControl.system;
        meshObjCap.transform.localEulerAngles = Vector3.zero;
        meshObjCap.layer = 9;
        mfCap = meshObjCap.AddComponent<MeshFilter>();
        mrCap = meshObjCap.AddComponent<MeshRenderer>();
        meshCap = new Mesh();
        Vector3[] vertCap = new Vector3[] { Loc(P1 - transform.up * P1H), Loc(P2 - transform.up * P2H) };
        meshCap.vertices = vertCap;
        mfCap.sharedMesh = meshCap;
        mrCap.sharedMaterial = meshCapMat;
        mrCap.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        meshObj = new GameObject("TrackU");
        meshObj.transform.position = transform.position;
        meshObj.transform.parent = RelativeSystemControl.system;
        meshObj.transform.localEulerAngles = Vector3.zero;
        mf = meshObj.AddComponent<MeshFilter>();
        mr = meshObj.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        Vector3[] vert = new Vector3[] { Loc(P1), Loc(P1), Loc(P2), Loc(P2) };
        mesh.vertices = vert;
        mf.sharedMesh = mesh;
        mr.sharedMaterial = meshMat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private void CreateNewSeg()
    {
        var P1 = getP1();
        var P1s = getP1s();
        var P2s = getP2s();
        var P2 = getP2();
        var P1H = getH(P1);
        var P2H = getH(P2);
        int i1, i2, i3, i4;

        Vector3[] vertCap = new Vector3[meshCap.vertices.Length + 2];
        meshCap.vertices.CopyTo(vertCap, 0);
        new Vector3[] { Loc(P1 - ToRelativeWorldRotate(transform.up) * P1H), Loc(P2 - ToRelativeWorldRotate(transform.up) * P2H) }.CopyTo(vertCap, meshCap.vertices.Length);
        i1 = meshCap.vertices.Length;
        i2 = i1 + 1;
        /*  i2-i1
             |\|
            i2-i1   */
        int[] triCap = new int[meshCap.triangles.Length + 2 * 3];
        meshCap.triangles.CopyTo(triCap, 0);
        new int[] { i1, i1 - 2, i2, i2, i1 - 2, i2 - 2 }.CopyTo(triCap, meshCap.triangles.Length);

        meshCap.vertices = vertCap;
        meshCap.triangles = triCap;
        meshCap.RecalculateBounds();
        meshCap.RecalculateNormals();
        meshCap.UploadMeshData(false);

        Vector3[] vert = new Vector3[mesh.vertices.Length + 4];
        mesh.vertices.CopyTo(vert, 0);
        new Vector3[] { Loc(P1), Loc(P1s), Loc(P2s), Loc(P2) }.CopyTo(vert, mesh.vertices.Length);

        i1 = mesh.vertices.Length;
        i2 = i1 + 1;
        i3 = i1 + 2;
        i4 = i1 + 3;
        /*  i4--i3--i2--i1
             | \ | \ | \ |
            i4--i3--i2--i1   */
        int[] tri = new int[mesh.triangles.Length + 6 * 3];
        mesh.triangles.CopyTo(tri, 0);
        new int[] {
            i1, i2, i1 - 4, i2, i2 - 4, i1 - 4,
            i2, i3, i2 - 4, i3, i3 - 4, i2 - 4,
            i3, i4, i3 - 4, i4, i4 - 4, i3 - 4,
        }.CopyTo(tri, mesh.triangles.Length);

        mesh.vertices = vert;
        mesh.triangles = tri;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.UploadMeshData(false);
    }

    private void Update()
    {
        if (lastP1 == Vector3.zero && lastP2 == Vector3.zero)
        {
            lastP1 = getP1();
            lastP2 = getP2();
        }
        Vector3 P1 = getP1();
        Vector3 P2 = getP2();
        Vector3 Pc = (P1 + P2) / 2;
        float cH = getH(Pc);
        transform.localPosition = transform.localPosition.SetY(Mathf.Clamp(transform.localPosition.y - cH, limit.x, limit.y));
        if ((P1 - lastP1).magnitude > segLength || (P2 - lastP2).magnitude > segLength)
        {
            P1 = getP1();
            P2 = getP2();
            //var dir = ((P1 + P2) - (lastP1 + lastP2)) / 2;
            var dir1 = P1 - lastP1;
            var dir2 = P2 - lastP2;
            float angle = Mathf.Max(/*Vector3.Angle(transform.forward, dir), */Vector3.Angle(ToRelativeWorldRotate(transform.forward), dir1), Vector3.Angle(ToRelativeWorldRotate(transform.forward), dir2));
            switch (status)
            {
                case Status.Backward:
                    if(angle < 80 && cH < 1)
                    {
                        status = Status.Forward;
                        CreateNewTrack();
                    }
                    break;
                case Status.Forward:
                    if (angle > 85 || cH >= 1)
                    {
                        status = Status.Backward;
                    }
                    else
                    {
                        if(tracklength > maxTrackLength)
                        {
                            CreateNewSeg();
                            CreateNewTrack();
                            tracklength = 0;
                        }
                        CreateNewSeg();
                        tracklength++;
                    }
                    break;
            }
            lastP1 = P1;
            lastP2 = P2;
        }
    }

    Vector3 Loc(Vector3 wld)
    {
        return wld - meshObjCap.transform.localPosition;
    }

    Vector3 Wld(Vector3 loc)
    {
        return loc + meshObjCap.transform.localPosition;
    }

    Vector3 ToRelativeWorld(Vector3 absoluteWorld)
    {
        return (RelativeSystemControl.system.worldToLocalMatrix * absoluteWorld.V4(1)).V3();
    }
    Vector3 ToRelativeWorldRotate(Vector3 absoluteWorld)
    {
        return Quaternion.Inverse(RelativeSystemControl.system.rotation) * absoluteWorld;
    }

    Vector3 getP1()
    {
        return ToRelativeWorld(transform.position + transform.right * pDiatance / 2);
    }
    Vector3 getP2()
    {
        return ToRelativeWorld(transform.position - transform.right * pDiatance / 2);
    }
    Vector3 getP1s()
    {
        return ToRelativeWorld(transform.position + transform.right * pDiatance / 3 - transform.up * depth);
    }
    Vector3 getP2s()
    {
        return ToRelativeWorld(transform.position - transform.right * pDiatance / 3 - transform.up * depth);
    }
    float getH(Vector3 p)
    {
        p = (RelativeSystemControl.system.localToWorldMatrix * p.V4(1)).V3();
        float distance = 1;
        Ray ray = new Ray(p + transform.up, -transform.up);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 2, 1 << 10))
        {
            distance = hit.distance - 1;
        }
        return distance;
    }

    enum Status
    {
        Forward,
        Backward,
    }
}
