using UnityEngine;

public class RopePhysics : MonoBehaviour
{
    #region Rope Settings
    public int segmentCount = 20;
    public float segmentLength = 0.25f;
    public GameObject segmentPrefab;

    public Transform startPoint;
    public Transform endPoint;
    #endregion

    #region Runtime
    private GameObject[] segments;
    #endregion

    void Start()
    {
        #region Create Rope
        GenerateRope();
        #endregion
    }

    void GenerateRope()
    {
        #region Setup Array
        segments = new GameObject[segmentCount];
        #endregion

        #region Create First Segment
        Vector3 pos = startPoint.position;

        GameObject first = Instantiate(segmentPrefab, pos, Quaternion.identity);
        segments[0] = first;

        Rigidbody prevRb = first.GetComponent<Rigidbody>();
        prevRb.isKinematic = true; // anchor start
        #endregion

        #region Create Chain
        for (int i = 1; i < segmentCount; i++)
        {
            pos.y -= segmentLength;

            GameObject seg = Instantiate(segmentPrefab, pos, Quaternion.identity);
            segments[i] = seg;

            Rigidbody rb = seg.GetComponent<Rigidbody>();

            CharacterJoint joint = seg.AddComponent<CharacterJoint>();
            joint.connectedBody = prevRb;

            joint.swingAxis = Vector3.right;

            SoftJointLimit limit = joint.lowTwistLimit;
            limit.limit = -30;
            joint.lowTwistLimit = limit;

            SoftJointLimit limit2 = joint.highTwistLimit;
            limit2.limit = 30;
            joint.highTwistLimit = limit2;

            SoftJointLimit swing = joint.swing1Limit;
            swing.limit = 40;
            joint.swing1Limit = swing;

            prevRb = rb;
        }
        #endregion

        #region Attach End Point
        Rigidbody lastRb = segments[segmentCount - 1].GetComponent<Rigidbody>();

        if (endPoint != null)
        {
            CharacterJoint endJoint = endPoint.gameObject.AddComponent<CharacterJoint>();
            endJoint.connectedBody = lastRb;
        }
        #endregion
    }
}