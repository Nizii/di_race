using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class trackCreator : MonoBehaviour
{

    private Vector3 inputVector = new Vector3(0,0,0);
    private float rollInput = 0f;

    Mesh mesh;
    List<Vector3> centerUpVectors = new List<Vector3>();
    List<Vector3> centerPositionVectors = new List<Vector3>();
    List<Vector3> verticesList = new List<Vector3>();
    List<int> trianglesList = new List<int>();

    Queue<GameObject> trackBorders = new Queue<GameObject>();

    [SerializeField]
    private float trackWidth = 2f;

    [SerializeField]
    private float speed = 2f;


    [SerializeField]
    private float rollSpeed = 1f;


    [SerializeField]
    private float pitchSpeed = .04f;


    [SerializeField]
    private float yawSpeed = .1f;

    [SerializeField]
    private GameObject trackHead;


    [SerializeField]
    private Material glowLineTopMaterial;

    [SerializeField]
    private Material glowLineSideMaterial;


    private int physicUpdateCount = 0;

    private bool gameOver = false;


    private Mesh glowLineTL; // Top Left
    private Mesh glowLineTR; // Top Right
    private Mesh glowLineSL; // Side Left
    private Mesh glowLineSR; // Sider Right

    private List<Vector3> glowLineTLVerticesList = new List<Vector3>();
    private List<int> glowLineTLTrianglesList = new List<int>();

    private List<Vector3> glowLineTRVerticesList = new List<Vector3>();
    private List<int> glowLineTRTrianglesList = new List<int>();

    private List<Vector3> glowLineSLVerticesList = new List<Vector3>();
    private List<int> glowLineSLTrianglesList = new List<int>();

    private List<Vector3> glowLineSRVerticesList = new List<Vector3>();
    private List<int> glowLineSRTrianglesList = new List<int>();


    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        this.AddVertices();

        glowLineTL = new Mesh();
        glowLineTR = new Mesh();
        glowLineSL = new Mesh();
        glowLineSR = new Mesh();

        GameObject glowLineTLGO = new GameObject("GlowLineTopLeft");
        glowLineTLGO.transform.parent = this.transform;
        glowLineTLGO.transform.position = new Vector3(0, 0.01f, 0);
        glowLineTLGO.AddComponent<MeshFilter>().mesh = glowLineTL;
        glowLineTLGO.AddComponent<MeshRenderer>().material = glowLineTopMaterial;
        GameObject glowLineTRGO = new GameObject("GlowLineTopRight");
        glowLineTRGO.transform.parent = this.transform;
        glowLineTRGO.AddComponent<MeshFilter>().mesh = glowLineTR;
        glowLineTRGO.AddComponent<MeshRenderer>().material = glowLineTopMaterial;
        glowLineTRGO.transform.position = new Vector3(0, 0.01f, 0);
        GameObject glowLineSLGO = new GameObject("GlowLineSideLeft");
        glowLineSLGO.transform.parent = this.transform;
        glowLineSLGO.AddComponent<MeshFilter>().mesh = glowLineSL;
        glowLineSLGO.AddComponent<MeshRenderer>().material = glowLineSideMaterial;
        glowLineSLGO.transform.position = new Vector3(0, 0.01f, 0);
        GameObject glowLineSRGO = new GameObject("GlowLineSideRight");
        glowLineSRGO.transform.parent = this.transform;
        glowLineSRGO.AddComponent<MeshFilter>().mesh = glowLineSR;
        glowLineSRGO.AddComponent<MeshRenderer>().material = glowLineSideMaterial;
        glowLineSRGO.transform.position = new Vector3(0, 0.01f, 0);

    }


    public void OnMoveTracker(InputAction.CallbackContext value)
    {
        inputVector = new Vector3(value.ReadValue<Vector2>().x * yawSpeed * speed, -value.ReadValue<Vector2>().y * pitchSpeed * speed, 0f);
        rollInput = -value.ReadValue<Vector2>().x;
    }


    // Update is called once per frame
    void Update()
    {
        

    }    

    private void FixedUpdate()
    {
        if (!gameOver)
        {
            if (physicUpdateCount % 2 == 0)
            {
                Collider trackHeadCollider = trackHead.GetComponent<Collider>();
                foreach (Vector3 vertex in verticesList)
                {
                    if (trackHeadCollider.bounds.Contains(vertex))
                    {
                        if(ColliderContainsPoint(trackHeadCollider, vertex)){
                            this.gameOver = true;
                        }
                    }
                }
                BuildTrack();
            }
            physicUpdateCount++;
        }
    }


    private void BuildTrack()
    {
        //Controller Inputs
        //Vector3 inputVector = new Vector3(move.ReadValue<Vector2>().x * yawSpeed * speed, -move.ReadValue<Vector2>().y * pitchSpeed * speed, 0f);
        Vector3 inputVectorY = new Vector3(0f, inputVector.y, 0f);
        Vector3 newForward = trackHead.transform.rotation * inputVector + trackHead.transform.forward;

        Vector3 newUP = (trackHead.transform.rotation * inputVectorY + trackHead.transform.up).normalized;
        trackHead.transform.rotation = Quaternion.LookRotation(newForward, newUP);
        trackHead.transform.Rotate(0f, 0f, rollInput * rollSpeed * speed, Space.Self);
        trackHead.transform.position += trackHead.transform.forward * speed;

        centerUpVectors.Add(newUP);
        centerPositionVectors.Add(trackHead.transform.position);

        ////Create Mesh Parts////
        this.AddVertices();
        this.AddTriangles();
        this.AddColliderBoxes();
        UpdateMesh();

        //Update Camera
        //Camera.main.transform.position = trackHead.transform.position + trackHead.transform.up * 3f + -trackHead.transform.forward*20f;
        //Camera.main.transform.rotation = Quaternion.LookRotation(trackHead.transform.forward);
    }

    private void AddVertices()
    {
        //Build Vertices
        Vector3 vertex0 = trackHead.transform.position - trackHead.transform.right * trackWidth * .5f;
        Vector3 vertex1 = trackHead.transform.position + trackHead.transform.right * trackWidth * .5f;
        Vector3 vertex2 = vertex0 - trackHead.transform.right + trackHead.transform.up * 2;
        Vector3 vertex3 = vertex1 + trackHead.transform.right + trackHead.transform.up * 2;
        Vector3 vertex4 = vertex2 - trackHead.transform.right * .5f;
        Vector3 vertex5 = vertex3 + trackHead.transform.right * .5f;
        Vector3 vertex6 = vertex2 - trackHead.transform.up * 3.2f;
        Vector3 vertex7 = vertex3 - trackHead.transform.up * 3.2f;

        verticesList.Add(vertex0);
        verticesList.Add(vertex1);
        verticesList.Add(vertex2);
        verticesList.Add(vertex3);
        verticesList.Add(vertex4);
        verticesList.Add(vertex5);
        verticesList.Add(vertex6);
        verticesList.Add(vertex7);

        //TL Line
        glowLineTLVerticesList.Add(vertex2 - trackHead.transform.right * .3f);
        glowLineTLVerticesList.Add(vertex4);

        //TR Line
        glowLineTRVerticesList.Add(vertex5);
        glowLineTRVerticesList.Add(vertex3 + trackHead.transform.right * .3f);

        //SL Line
        glowLineSLVerticesList.Add(vertex0);
        glowLineSLVerticesList.Add(vertex0 - trackHead.transform.right * 0.25f + trackHead.transform.up * 0.5f);

        //SR Line
        glowLineSRVerticesList.Add(vertex1 + trackHead.transform.right * 0.25f + trackHead.transform.up * 0.5f);
        glowLineSRVerticesList.Add(vertex1);
    }

    private void AddTriangles()
    {
        int vertexCount = verticesList.Count;
        trianglesList.Add(vertexCount - 16); //0
        trianglesList.Add(vertexCount - 8); //8
        trianglesList.Add(vertexCount - 15); //1

        trianglesList.Add(vertexCount - 15); //1
        trianglesList.Add(vertexCount - 8); //8
        trianglesList.Add(vertexCount - 7); //9

        trianglesList.Add(vertexCount - 15); //1
        trianglesList.Add(vertexCount - 7); //9
        trianglesList.Add(vertexCount - 13); //3

        trianglesList.Add(vertexCount - 13); //3
        trianglesList.Add(vertexCount - 7); //9
        trianglesList.Add(vertexCount - 5); //11

        trianglesList.Add(vertexCount - 14); //2
        trianglesList.Add(vertexCount - 6); //10
        trianglesList.Add(vertexCount - 16); //0

        trianglesList.Add(vertexCount - 16); //0
        trianglesList.Add(vertexCount - 6); //10
        trianglesList.Add(vertexCount - 8); //8

        trianglesList.Add(vertexCount - 12); //4
        trianglesList.Add(vertexCount - 4); //12
        trianglesList.Add(vertexCount - 14); //2

        trianglesList.Add(vertexCount - 14); //2
        trianglesList.Add(vertexCount - 4); //12
        trianglesList.Add(vertexCount - 6); //10

        trianglesList.Add(vertexCount - 13); //3
        trianglesList.Add(vertexCount - 5); //11
        trianglesList.Add(vertexCount - 11); //5

        trianglesList.Add(vertexCount - 11); //5
        trianglesList.Add(vertexCount - 5); //11
        trianglesList.Add(vertexCount - 3); //13

        trianglesList.Add(vertexCount - 10); //6
        trianglesList.Add(vertexCount - 9); //7
        trianglesList.Add(vertexCount - 2); //14

        trianglesList.Add(vertexCount - 9); //7
        trianglesList.Add(vertexCount - 1); //15
        trianglesList.Add(vertexCount - 2); //14

        trianglesList.Add(vertexCount - 10); //6
        trianglesList.Add(vertexCount - 2); //14
        trianglesList.Add(vertexCount - 12); //4

        trianglesList.Add(vertexCount - 2); //14
        trianglesList.Add(vertexCount - 4); //12
        trianglesList.Add(vertexCount - 12); //4

        trianglesList.Add(vertexCount - 9); //7
        trianglesList.Add(vertexCount - 11); //5
        trianglesList.Add(vertexCount - 1); //15

        trianglesList.Add(vertexCount - 1); //15
        trianglesList.Add(vertexCount - 11); //5
        trianglesList.Add(vertexCount - 3); //13

        int vertexCountTL = glowLineTLVerticesList.Count;
        glowLineTLTrianglesList.Add(vertexCountTL - 4);
        glowLineTLTrianglesList.Add(vertexCountTL - 3);
        glowLineTLTrianglesList.Add(vertexCountTL - 1);

        glowLineTLTrianglesList.Add(vertexCountTL - 4);
        glowLineTLTrianglesList.Add(vertexCountTL - 1);
        glowLineTLTrianglesList.Add(vertexCountTL - 2);

        int vertexCountTR = glowLineTRVerticesList.Count;
        glowLineTRTrianglesList.Add(vertexCountTR - 4);
        glowLineTRTrianglesList.Add(vertexCountTR - 3);
        glowLineTRTrianglesList.Add(vertexCountTR - 1);

        glowLineTRTrianglesList.Add(vertexCountTR - 4);
        glowLineTRTrianglesList.Add(vertexCountTR - 1);
        glowLineTRTrianglesList.Add(vertexCountTR - 2);

        int vertexCountSL = glowLineSRVerticesList.Count;
        glowLineSLTrianglesList.Add(vertexCountSL - 4);
        glowLineSLTrianglesList.Add(vertexCountSL - 3);
        glowLineSLTrianglesList.Add(vertexCountSL - 1);

        glowLineSLTrianglesList.Add(vertexCountSL - 4);
        glowLineSLTrianglesList.Add(vertexCountSL - 1);
        glowLineSLTrianglesList.Add(vertexCountSL - 2);

        int vertexCountSR = glowLineSLVerticesList.Count;
        glowLineSRTrianglesList.Add(vertexCountSR - 4);
        glowLineSRTrianglesList.Add(vertexCountSR - 3);
        glowLineSRTrianglesList.Add(vertexCountSR - 1);

        glowLineSRTrianglesList.Add(vertexCountSR - 4);
        glowLineSRTrianglesList.Add(vertexCountSR - 1);
        glowLineSRTrianglesList.Add(vertexCountSR - 2);
    }

    private void AddColliderBoxes()
    {
        int vertexCount = verticesList.Count;
        GameObject leftColliderGO, rightColliderGO;
        BoxCollider leftCollider, rightCollider;
        if (trackBorders.Count > 400)
        {
            leftColliderGO = trackBorders.Dequeue();
            rightColliderGO = trackBorders.Dequeue();
            leftCollider = leftColliderGO.GetComponent<BoxCollider>();
            rightCollider = rightColliderGO.GetComponent<BoxCollider>();

        } else
        {
            leftColliderGO = new GameObject("leftBorder");
            leftColliderGO.tag = "Track";
            rightColliderGO = new GameObject("rightBorder");
            rightColliderGO.tag = "Track";
            leftCollider = leftColliderGO.AddComponent<BoxCollider>();
            rightCollider = rightColliderGO.AddComponent<BoxCollider>();
        }
        //Box Left
        leftColliderGO.transform.parent = transform;
        Vector3 leftColliderGOForward = verticesList[vertexCount - 8] - verticesList[vertexCount - 16];
        //Vector3 leftColliderGOUP = verticesList[vertexCount - 16] - verticesList[vertexCount - 14];
        Vector3 leftColliderGOUP = verticesList[vertexCount - 10] - verticesList[vertexCount - 14];
        leftColliderGO.transform.rotation = Quaternion.LookRotation(leftColliderGOForward, leftColliderGOUP);
        leftColliderGO.transform.position = verticesList[vertexCount - 6] + (verticesList[vertexCount - 16] - verticesList[vertexCount - 6]) * 0.5f;
        leftCollider.size = new Vector3(.1f, leftColliderGOUP.magnitude, leftColliderGOForward.magnitude);
        leftCollider.isTrigger = true;
        trackBorders.Enqueue(leftColliderGO);

        //Box Right
        rightColliderGO.transform.parent = transform;
        Vector3 rightColliderGOForward = verticesList[vertexCount - 7] - verticesList[vertexCount - 15]; 
        //Vector3 rightColliderGOUP = verticesList[vertexCount - 15] - verticesList[vertexCount - 13];
        Vector3 rightColliderGOUP = verticesList[vertexCount - 9] - verticesList[vertexCount - 13];
        rightColliderGO.transform.rotation = Quaternion.LookRotation(rightColliderGOForward, rightColliderGOUP);
        rightColliderGO.transform.position = verticesList[vertexCount - 5]+(verticesList[vertexCount - 15] - verticesList[vertexCount - 5]) * 0.5f; 
        rightCollider.size = new Vector3(.1f, rightColliderGOUP.magnitude, rightColliderGOForward.magnitude);
        rightCollider.isTrigger = true;
        trackBorders.Enqueue(rightColliderGO);
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if(other.tag != "Track" && other.tag != "Player")
        {
            Destroy(other.gameObject);
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();

        mesh.RecalculateNormals();

        glowLineTL.Clear();
        glowLineTL.vertices = glowLineTLVerticesList.ToArray();
        glowLineTL.triangles = glowLineTLTrianglesList.ToArray();
        glowLineTL.RecalculateNormals();

        glowLineTR.Clear();
        glowLineTR.vertices = glowLineTRVerticesList.ToArray();
        glowLineTR.triangles = glowLineTRTrianglesList.ToArray();
        glowLineTR.RecalculateNormals();

        glowLineSL.Clear();
        glowLineSL.vertices = glowLineSLVerticesList.ToArray();
        glowLineSL.triangles = glowLineSLTrianglesList.ToArray();
        glowLineSL.RecalculateNormals();

        glowLineSR.Clear();
        glowLineSR.vertices = glowLineSRVerticesList.ToArray();
        glowLineSR.triangles = glowLineSRTrianglesList.ToArray();
        glowLineSR.RecalculateNormals();
    }

    bool ColliderContainsPoint(Collider colliderToCheck, Vector3 posToCheck)
    {
        Vector3 offset = colliderToCheck.bounds.center - posToCheck;
        Ray inputRay = new Ray(posToCheck, offset.normalized);
        RaycastHit rHit;

        if (!colliderToCheck.Raycast(inputRay, out rHit, offset.magnitude * 1.1f))
            return true;
        else
            return false;
    }
    public float getSpeed()
    {
        return this.speed;
    }
    public Vector3[] getCurrentCarUp()
    {
        if ((centerUpVectors.Count - 10) > 0)
            return new[] { centerUpVectors[centerUpVectors.Count - 10], centerPositionVectors[centerPositionVectors.Count - 10] };
        else
            return new[] { centerUpVectors[0], centerPositionVectors[0] };
    }
    public Vector3[] getCurrentCarUp(Vector3 currentCarPos)
    {
        float lastDistance = float.MinValue;
        float shortestDistance = float.MaxValue;
        int shortestDistanceIndex = centerPositionVectors.Count - 1;
        for (int i = centerPositionVectors.Count - 1; i >= 0; i--)
        {
            Vector3 centerPositionVector = centerPositionVectors[i];
            float currentDistance = (currentCarPos - centerPositionVector).magnitude;

            if (currentDistance < shortestDistance)
            {
                shortestDistance = currentDistance;
                shortestDistanceIndex = i;
            }
            if (currentDistance > lastDistance)
            {
                //break;
            }
            lastDistance = currentDistance;
            
        }
        return new[]{centerUpVectors[shortestDistanceIndex], centerPositionVectors[shortestDistanceIndex] };

    }
}
