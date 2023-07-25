using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Combat playerCombat;
    public Transform cam;
    public float speed;

    public float turnSmoothTime;

    private float turnSmoothVelocity;

    float dodgeCooldown;

    [SerializeField]
    Transform Chara;

    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(!playerCombat.isAttCD && !playerCombat.isShieldUp && !playerCombat.isDash)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
            //Debug.Log(dir);
            if (dir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 movedir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(movedir.normalized * speed * Time.deltaTime);
            }
        }
        

        //
        

    }

    

}
