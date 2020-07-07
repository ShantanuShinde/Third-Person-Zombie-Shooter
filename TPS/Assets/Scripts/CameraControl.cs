using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    
    public float CameraHeight = 3, CameraLeftShift=1;
    float CameraMaxTilt = 90;
    [Range(0, 4)]
    public float CameraSpeed = 2;
    float currentPam, currentTilt = 3, currentDistance = 5;

    //float defDist = 5;

    public GameObject player;
    public Transform tilt;
    Camera mainCam;

    public bool collisionDebug;
    public float collisionCushion = 0.35f;
    public float clipCushion = 1.75f;
    public int rayGridx = 9, rayGridY = 5;

    float adjustedDistance;
    public LayerMask collisionMask;
    Ray camRay;
    RaycastHit camHit;


    

    Vector3[] camClip, clipDirection, playerClip, rayColOrigin, rayColPoint;
    bool[] rayColHit;

    
    private void Start()
    {
        mainCam = Camera.main;

        transform.position = player.transform.position + Vector3.up * CameraHeight + player.transform.right*CameraLeftShift;
        transform.rotation = player.transform.rotation;

        tilt.eulerAngles = new Vector3(currentTilt, transform.eulerAngles.y, transform.eulerAngles.z);
        tilt.position = tilt.forward * currentDistance;
        mainCam.transform.position = transform.position + tilt.forward * -currentDistance;



        CameraClipInfo();
    }

    private void Update()
    {
        bool closeDist = Mathf.Abs(currentDistance - adjustedDistance) < 0.001;
        
        
        if (GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Aim && !Input.GetKey(KeyCode.Mouse1))
        {
            currentTilt = 4.5f;
            GameManager.Instance.playerStates.SetBack();
        }
        if (Input.GetKey(KeyCode.Mouse1) && player.GetComponent<PlayerContorller>().weapInd != 3)
        {
            if(GameManager.Instance.playerStates.PlayerState != PlayerStates.States.Aim)
            {
                currentTilt = 4.5f;
            }

            GameManager.Instance.playerStates.SetAim();
        }
        else if(closeDist && GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Back)
        {
            adjustedDistance = currentDistance;
            GameManager.Instance.playerStates.SetStand();
        }
        if(rayGridx*rayGridY != rayColHit.Length)
        {
            CameraClipInfo();
        }
        
        CameraTransforms();
        if (GameManager.Instance.playerStates.PlayerState != PlayerStates.States.Aim && GameManager.Instance.playerStates.PlayerState != PlayerStates.States.Back)
            CameraCollision();
    }
    void CameraCollision()
    {
        
        float camDistance = adjustedDistance + collisionCushion;

        for(int i = 0; i < camClip.Length; i++)
        {
            Vector3 clipPoint = mainCam.transform.up * camClip[i].y + mainCam.transform.right * camClip[i].x;
            clipPoint *= clipCushion;
            clipPoint += mainCam.transform.forward * camClip[i].z;
            clipPoint += transform.position - (tilt.forward * camDistance);

            
            Vector3 playerPoint = mainCam.transform.up * camClip[i].y + mainCam.transform.right * camClip[i].x;
            playerPoint += transform.position;

            clipDirection[i] = (clipPoint - playerPoint).normalized;
            playerClip[i] = playerPoint;
        }

        int currentRay = 0;
        bool isColliding = false;

        float rayX = rayGridx - 1;
        float rayY = rayGridY - 1;

        for(int x = 0; x < rayGridx; x++)
        {
            Vector3 CU_Point = Vector3.Lerp(clipDirection[1],clipDirection[2],x/rayX);
            Vector3 CL_Point = Vector3.Lerp(clipDirection[0], clipDirection[3], x / rayX);

            Vector3 PU_Point = Vector3.Lerp(playerClip[1], playerClip[2], x / rayX);
            Vector3 PL_Point = Vector3.Lerp(playerClip[0], playerClip[3], x / rayX);

            for (int y = 0; y < rayGridY; y++)
            {
                camRay.origin = Vector3.Lerp(PU_Point, PL_Point, y / rayY);
                camRay.direction = Vector3.Lerp(CU_Point, CL_Point, y / rayY);

                rayColOrigin[currentRay] = camRay.origin;
                if (Physics.Raycast(camRay, out camHit, camDistance, collisionMask))
                {
                    isColliding = true;
                    rayColHit[currentRay] = true;
                    rayColPoint[currentRay] = camHit.point;

                }
                
                currentRay++;
            }
        }
        if (isColliding)
        {
            float minRayDistance = float.MaxValue;
            currentRay = 0;

            for(int i = 0; i < rayColHit.Length; i++)
            {
                if (rayColHit[i])
                {
                    float colDistance = Vector3.Distance(rayColOrigin[i], rayColPoint[i]);
                    if(colDistance < minRayDistance)
                    {
                        minRayDistance = colDistance;
                        currentRay = i;
                    }
                }
            }
            Vector3 clipCenter = transform.position - (tilt.forward * currentDistance);
            adjustedDistance = Vector3.Distance(-mainCam.transform.forward, clipCenter - rayColPoint[currentRay]);
            adjustedDistance = currentDistance - (adjustedDistance + collisionCushion);
        }
        else 
        {
            adjustedDistance = currentDistance;
        }

       
    }
    
   void CameraClipInfo()
    {
        camClip = new Vector3[4];
        clipDirection = new Vector3[4];
        playerClip = new Vector3[4];

        mainCam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, camClip);

        int rays = rayGridx * rayGridY;

        rayColOrigin = new Vector3[rays];
        rayColPoint = new Vector3[rays];
        rayColHit = new bool[rays];

    
    }
    void CameraTransforms()
    {
        if (!player.GetComponent<PlayerContorller>().animPlaying)
        {
            switch (GameManager.Instance.playerStates.PlayerState)
            {
                case PlayerStates.States.Forward:
                    currentDistance = 5;
                    if (player.GetComponent<Animator>().GetFloat("Speed") >= 0.5f)
                    {

                        currentPam = player.transform.eulerAngles.y;
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentPam, transform.eulerAngles.z);
                        transform.position = player.transform.position + Vector3.up * CameraHeight + player.transform.right * CameraLeftShift;
                    }
                    //adjustedDistance = currentDistance;

                    currentTilt = 4.5f;
                    break;
                case PlayerStates.States.Backward:
                    if (player.GetComponent<Animator>().GetFloat("Speed") >= 0.5f)
                    {
                        transform.position = player.transform.position + Vector3.up * CameraHeight - player.transform.right * CameraLeftShift;
                        currentPam = Input.GetAxis("Mouse X") * 10;
                        transform.RotateAround(player.transform.position, Vector3.up, currentPam);
                    }
                    currentTilt = 4.5f;
                    //adjustedDistance = currentDistance;
                    break;
                case PlayerStates.States.Left:
                    if (player.GetComponent<Animator>().GetFloat("Speed") >= 0.5f)
                    {
                        transform.position = player.transform.position + Vector3.up * CameraHeight - player.transform.forward * CameraLeftShift;
                        currentPam = Input.GetAxis("Mouse X") * 10;
                        transform.RotateAround(player.transform.position, Vector3.up, currentPam);
                    }
                    currentTilt = 4.5f;
                    //adjustedDistance = currentDistance;
                    break;
                case PlayerStates.States.Right:
                    if (player.GetComponent<Animator>().GetFloat("Speed") >= 0.5f)
                    {
                        currentPam = Input.GetAxis("Mouse X") * 10;
                        transform.position = player.transform.position + Vector3.up * CameraHeight + player.transform.forward * CameraLeftShift;
                        currentPam = Input.GetAxis("Mouse X") * 10;
                        transform.RotateAround(player.transform.position, Vector3.up, currentPam);
                    }
                    currentTilt = 4.5f;
                    //adjustedDistance = currentDistance;
                    break;
                case PlayerStates.States.Stand:
                    currentDistance = 5;
                    currentPam = Input.GetAxis("Mouse X") * CameraSpeed;
                    transform.RotateAround(player.transform.position, Vector3.up, currentPam);
                    currentTilt -= Input.GetAxis("Mouse Y") * CameraSpeed;
                    currentTilt = Mathf.Clamp(currentTilt, -50, +CameraMaxTilt);
                    //adjustedDistance = currentDistance;
                    break;
                case PlayerStates.States.Aim:
                    transform.position = player.transform.position + Vector3.up * CameraHeight + player.transform.right * CameraLeftShift;
                    currentPam = Input.GetAxis("Mouse X") * 10;
                    transform.RotateAround(player.transform.position, Vector3.up, currentPam);

                    float ang = player.GetComponent<Animator>().GetFloat("LookAngle");
                    currentTilt = ang;
                    if (ang < 0)
                    {
                        adjustedDistance = Mathf.Lerp(adjustedDistance, currentDistance - (3.5f + 1.5f * (ang / 45)), 10 * Time.deltaTime);
                    }
                    else
                    {
                        adjustedDistance = Mathf.Lerp(adjustedDistance, currentDistance - 3.5f, 10 * Time.deltaTime);
                    }

                    break;
                case PlayerStates.States.Back:

                    adjustedDistance = Mathf.Lerp(adjustedDistance, currentDistance, 20 * Time.deltaTime);
                    break;
            }
        }
        
        
        tilt.eulerAngles = new Vector3(currentTilt, transform.eulerAngles.y, transform.eulerAngles.z);
        
        mainCam.transform.position = transform.position + tilt.forward * -adjustedDistance;
    }
}

