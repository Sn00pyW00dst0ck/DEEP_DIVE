using UnityEngine;

[System.Serializable]
public class MapTransforms
    {
        public Transform vrTarget;
        public Transform ikTarget;

        public Vector3 trackingPositionOffset;
        public Vector3 trackingRotationOffset;


        public void VRMapping()
        {            
            ikTarget.rotation = (vrTarget.rotation * Quaternion.Euler(trackingRotationOffset)); 
            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        }

        public void VRRotationMapping()
        {
            float z = ikTarget.transform.eulerAngles.z;
            float x = ikTarget.transform.eulerAngles.z;
            float y = ikTarget.transform.eulerAngles.z;


            Vector3 desiredRot = new Vector3(x, vrTarget.transform.eulerAngles.y, z);
            ikTarget.rotation = Quaternion.Euler(desiredRot);
    }
    }
public class AvatarController : MonoBehaviour
{
    [SerializeField]
    private MapTransforms head;

    [SerializeField]
    private MapTransforms leftHand;

    [SerializeField]
    private MapTransforms rightHand;

    [SerializeField]
    private MapTransforms[] body;

    [SerializeField]
    private float turnSmoothness;

    [SerializeField]
    Transform ikHead;

    [SerializeField]
    Vector3 headBodyOffset;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = ikHead.position + headBodyOffset;
        transform.forward = Vector3.Slerp(transform.forward, Vector3.ProjectOnPlane(ikHead.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

        head.VRMapping();
        foreach (MapTransforms bodyPart in body)
        {
            bodyPart.VRRotationMapping();
        }
        leftHand.VRMapping();
        rightHand.VRMapping();
        
    }


}
