using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MouseLook : MonoBehaviour
{

    [HideInInspector] public static MouseLook instance;

    public TextMesh NickName;

    private PhotonView phV;
    CharacterController iam;
    private Animator myanim;
    public Camera cam;
    public List<GameObject> body_meshes = new List<GameObject>();

    //public GameObject hands;
    public List<GameObject> hands_meshes = new List<GameObject>();

    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;

    public bool smooth;
    public float smoothTime = 20f;
    public float smoothTimeRecoil = 1f;
    public bool lockCursor = true;

    static float time = 0;
    static float OffsetX = 0;
    static float OffsetY = 0;
    static float downX;
    static float downY;
    static bool recoilDown;

    static int i = 0;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;

    void Start()
    {
        time = 0;
        recoilDown = false;
        myanim = this.GetComponent<Animator>();
        iam = this.GetComponent<CharacterController>();
        phV = this.GetComponent<PhotonView>();
        cam = this.gameObject.GetComponentInChildren<Camera>();
        m_CharacterTargetRot = iam.transform.localRotation;
        m_CameraTargetRot = cam.transform.localRotation;
        if (!phV.IsMine)
        {
            Destroy(cam);
            foreach (GameObject c in hands_meshes)
            {
                c.SetActive(false);

            }

        }
        else
        {
            foreach (GameObject c in body_meshes)
                c.SetActive(false);
        }

        //PhotonView t = 
        NickName.text = GetComponent<PhotonView>().Owner.NickName;
    }
    void Update()
    {
        time += Time.deltaTime;
        if (!phV.IsMine)
        {
            return;
        }

        if (GameManager.instance.IsAlive && GameManager.instance.menuOn == false)
            MouseControl();
        //Debug.Log(time + " " + recoilDown);
        if (recoilDown && time > 0.7f)
        {
            //CameraWentDown();
        }
    }

    void CameraWentDown()
    {
        float yRot = Input.GetAxis("Mouse X") * XSensitivity + downX;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity + downY;
        downX = 0;
        downY = 0;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth)
        {
            iam.transform.localRotation = Quaternion.Slerp(iam.transform.localRotation, m_CharacterTargetRot,
                smoothTimeRecoil * Time.deltaTime);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, m_CameraTargetRot,
                smoothTimeRecoil * Time.deltaTime);
        }
        else
        {
            iam.transform.localRotation = m_CharacterTargetRot;
            cam.transform.localRotation = m_CameraTargetRot;
        }
        myanim.SetFloat("vertical_look", m_CameraTargetRot.x * 40);
        myanim.SetFloat("Horizontal_look", m_CharacterTargetRot.y);

        recoilDown = false;


    }

    void MouseControl()
    {

        float yRot = Input.GetAxis("Mouse X") * XSensitivity + OffsetX;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity + OffsetY;
        OffsetY = 0;
        OffsetX = 0;


        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth)
        {
            iam.transform.localRotation = Quaternion.Slerp(iam.transform.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            iam.transform.localRotation = m_CharacterTargetRot;
            cam.transform.localRotation = m_CameraTargetRot;
        }
        myanim.SetFloat("vertical_look", m_CameraTargetRot.x * 40);
        myanim.SetFloat("Horizontal_look", m_CharacterTargetRot.y);

    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    public void Recoil(float up, float side)
    {
        time = 0;
        OffsetX = side;
        OffsetY = up;
        downY = -up;
        downX = -side;
        recoilDown = true;

    }

    public static MouseLook GetInstanse()
    {
        if (instance == null)
        {
            instance = new MouseLook();
        }
        return instance;
    }

    public void change_wheapon(int x)
    {

        for (int i = 0; i < hands_meshes.Count; i++)
        {
            hands_meshes[i].SetActive(false);
        }
        //Debug.Log(x+"");
        if (phV.IsMine)
        {
            hands_meshes[x].SetActive(true);
        }
        for (int i = 1; i < body_meshes.Count; i++)
        {
            body_meshes[i].SetActive(false);
        }
        if (!phV.IsMine)
            body_meshes[x + 1].SetActive(true);
        gameObject.GetComponent<IKControl>().leftHandObj = body_meshes[x + 1].transform.Find("Rhandpoint").gameObject.transform;//перепутал руку сперва
    }

}
