using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    private bool isMobile;
    private PhotonView myView;

    [Header("VR")]
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    [Header("Mobile")]
    public Rigidbody rb;
    private FixedJoystick joystick;

    public float _movespeed;

    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private float cameraSensitivity;

    private int rightId;

    private float halfScreenWidth;

    private Vector2 lookInput;
    private float cameraPitch;

    void Start()
    {
        isMobile = GameManager.Instance.IsMobile;
        myView = GetComponent<PhotonView>();

        if (isMobile) // Mobile
        {
            joystick = FindObjectOfType<FixedJoystick>();
            rightId = -1;

            halfScreenWidth = Screen.width * 0.5f;

            if (photonView.IsMine)
            {
                myView.RPC("DisableVR", RpcTarget.AllBuffered);
                FindObjectOfType<XROrigin>().gameObject.SetActive(false);
            }
            else
            {
                cameraTransform.gameObject.SetActive(false);
            }

        }
        else // VR
        {
            myView = GetComponent<PhotonView>();

            XROrigin rig = FindObjectOfType<XROrigin>();
            headRig = rig.transform.Find("Camera Offset/Main Camera");
            leftHandRig = rig.transform.Find("Camera Offset/Left Controller");
            rightHandRig = rig.transform.Find("Camera Offset/Right Controller");

            if (photonView.IsMine)
            {
                foreach (var item in GetComponentsInChildren<Renderer>())
                {
                    item.enabled = false;
                }

                myView.RPC("DisableMobile", RpcTarget.AllBuffered);
            }
        }
    }

    void Update()
    {
        if (isMobile) // Mobile
        {
            if (photonView.IsMine)
            {
                Move();
                GetTouchInput();

                if (rightId != -1)
                {
                    LookAround();
                }
            }
        }
        else // VR
        {
            if (photonView.IsMine)
            {
                MapPosition(head, headRig);
                MapPosition(leftHand, leftHandRig);
                MapPosition(rightHand, rightHandRig);

                UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
                UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
            }
        }
    }

    #region Mobile
    private void Move()
    {
        Vector3 v = new Vector3(joystick.Horizontal * _movespeed, rb.velocity.y, joystick.Vertical * _movespeed);
        rb.velocity = transform.TransformDirection(v);
    }

    private void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    {
                        if (t.position.x > halfScreenWidth && rightId == -1)
                            rightId = t.fingerId;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        if (t.fingerId == rightId)
                            rightId = -1;
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        if (t.fingerId == rightId)
                        {
                            lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                        }
                    }
                    break;

                case TouchPhase.Stationary:
                    {
                        if (t.fingerId == rightId)
                        {
                            lookInput = Vector2.zero;
                        }
                    }
                    break;
            }
        }
    }

    private void LookAround()
    {
        if (photonView.IsMine)
        {
            cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        }

        transform.Rotate(transform.up, lookInput.x);
    }

    [PunRPC]
    public void DisableVR()
    {
        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);
    }
    #endregion

    #region VR
    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    void MapPosition(Transform target, Transform targetRig)
    {
        target.position = targetRig.position;
        target.rotation = targetRig.rotation;
    }

    [PunRPC]
    public void DisableMobile()
    {
        cameraTransform.gameObject.SetActive(false);
        Destroy(GetComponent<PhotonTransformView>());
        Destroy(GetComponent<PhotonRigidbodyView>());
        Destroy(rb);
    }
    #endregion
}
