using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
//using Photon;

public class TankMove : UnityEngine.MonoBehaviour
{
    public float moveSpeed = 6.0f;      //玩家移动速度
    public float rotateSpeed = 5.0f;    //玩家转向速度
    public float carRotateSpeed = 2.0f; //坦克车体转向速度
    public float maingunRotateSpeed = 5.0f; //坦克炮塔转向速度
    public float jumpVelocity = 5.0f;   //玩家跳跃速度

    float minMouseRotateX = -5.0f;     //摄像机旋转角度的最小值
    float maxMouseRotateX = 10.0f;      //摄像机旋转角度的最大值
    float minMainGunRotateX = -5.0f;     //主炮旋转角度的最小值
    float maxMainGunRotateX = 45.0f;      //主炮旋转角度的最大值
    float mouseRotateX;                 //当前摄像机在X轴的旋转角度
    float mouseRotateY;                 //当前摄像机在Y轴的旋转角度
    float mainGunRotateX;               //当前主炮在X轴的旋转角度
    float mainGunRotateY;               //当前主炮在Y轴的旋转角度
    bool isGrounded;                    //玩家是否在地面

    GameObject mainGun;
    Camera myCamera;
    GameObject gunPivot;
    GameObject gunBarrelEnd;
    //Animator anim;
    Rigidbody rigid;
    BoxCollider boxCollider;
    //PlayerHealth playerHealth;

    float defaultAndroidDPI = 320.0f;

    public bool IsTurnOver {
        get {
            return Vector3.Angle(transform.up, Vector3.up) > 90;
        }
    }

    //初始化，获取组件，并计算相关值
    void Start()
    {
        myCamera = Camera.main;
        //gunPivot = GameObject.Find("GunPivot");
        //获取摄像机组件
        //mouseRotateX = cameraPivot.transform.localEulerAngles.x;           //将当前摄像机在X轴的旋转角度赋值给mouseRotateX
        //mouseRotateY = cameraPivot.transform.localEulerAngles.y;           //将当前摄像机在X轴的旋转角度赋值给mouseRotateY
        mouseRotateX = myCamera.transform.localEulerAngles.x;           //将当前摄像机在X轴的旋转角度赋值给mouseRotateX
        mouseRotateY = myCamera.transform.localEulerAngles.y;           //将当前摄像机在X轴的旋转角度赋值给mouseRotateY
        //anim = GetComponent<Animator>();                                //获取玩家动画控制器组件
        rigid = GetComponent<Rigidbody>();                              //获取玩家刚体组件
        boxCollider = GetComponent<BoxCollider>();              //获取玩家胶囊体碰撞体
        //playerHealth = GetComponent<PlayerHealth>();                   //获取玩家PlayerHealth脚本
		foreach (Transform t in GetComponentsInChildren<Transform>()) 
		{ 
			if(t.name=="MainGun"){mainGun = t.gameObject;} 
			if(t.name=="gunBarrelEnd"){gunBarrelEnd = t.gameObject;} 
		} 
        //mainGun = GameObject.Find("MainGun");              //获取坦克炮塔
        //gunBarrelEnd = GameObject.Find("gunBarrelEnd");     //获取坦克炮口
        Cursor.lockState = CursorLockMode.Locked;
#if (UNITY_ANDROID)
        rotateSpeed = rotateSpeed / Screen.dpi * defaultAndroidDPI;
#endif
    }

    //每隔固定时间执行一次，用于物理模拟
    void FixedUpdate()
    {
        /*if (!playerHealth.isAlive)  //如果玩家已死亡，结束函数执行
            return;
        CheckGround();              //判断玩家是否位于地面
        if (isGrounded == false)
            anim.SetBool("isJump", false);*/

        if (IsTurnOver) {
            Debug.Log("turn over");
        }
        
        MoveHandler();
    }
    /*
    //判断玩家是否位于地面，更新PlayerMove类的isGrounded字段
    void CheckGround()
    {
        RaycastHit hitInfo;
        float shellOffset = 0.01f;
        float groundCheckDistance = 0.01f;
        Vector3 currentPos = transform.position;
        currentPos.y += boxCollider.height / 2f;
        if (Physics.SphereCast(currentPos, boxCollider.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
            ((boxCollider.height / 2f) - boxCollider.radius) + groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
    }*/
    /*
    //跳跃函数
    void Jump(bool isGround)
    {
        //当玩家按下跳跃键，并且玩家在地面上时执行跳跃相关函数
        if (CrossPlatformInputManager.GetButtonDown("Jump") && isGround)
        {
            //给玩家刚体组件添加向上的作用力，以改变玩家的运动速度，改变值为jumpVelocity
            rigid.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
        }
    }
    */
    void MoveHandler()
    {
        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("fire2");
        }
        float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");	//获取玩家水平轴上的输入
        float v = CrossPlatformInputManager.GetAxisRaw("Vertical");	//获取玩家垂直轴上的输入
        Move(v);		//根据玩家的输入控制角色移动
        float rv = CrossPlatformInputManager.GetAxisRaw("Mouse X");	//获取玩家鼠标垂直轴上的移动
        float rh = CrossPlatformInputManager.GetAxisRaw("Mouse Y");    //获取玩家鼠标水平轴上的移动
        Rotate(rh, rv, h);	//根据玩家的鼠标输入控制角色转向
        //Jump(isGrounded);	//玩家的跳跃函数
    }
    //角色移动函数
    void Move(float v)
    {
        //玩家以moveSpeed的速度进行平移
        //transform.Translate((Vector3.forward * v + Vector3.right * h) * moveSpeed * Time.deltaTime);
        transform.Translate((Vector3.forward * v) * moveSpeed * Time.deltaTime);
    }
    //角色转向函数
    void Rotate(float rh, float rv, float h)
    {
        rh = Mathf.Abs(rh) < 0.1f ? 0.0f : rh;
        rv = Mathf.Abs(rv) < 0.1f ? 0.0f : rv;
        h = Mathf.Abs(h) < 0.1f ? 0.0f : h;
        transform.Rotate(0, h * carRotateSpeed, 0);   //鼠标水平轴上的移动控制角色左右转向
        mouseRotateX -= rh * rotateSpeed;           //计算当前摄像机的旋转角度
        mouseRotateY += rv * rotateSpeed;
        mouseRotateY -= h * carRotateSpeed;
        mouseRotateX = Mathf.Clamp(mouseRotateX, minMouseRotateX, maxMouseRotateX); //将旋转角度限制在miniMouseRotateX与MaxiMouseRotateY之间
        //cameraPivot.transform.localEulerAngles = new Vector3(mouseRotateX, mouseRotateY, 0.0f);
        myCamera.transform.localEulerAngles = new Vector3(mouseRotateX, mouseRotateY, 0.0f);    //设置摄像机的旋转角度
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 hitPosition;
        
        if (Physics.Raycast(ray, out hit, 100))
        {
            hitPosition = hit.point;
            //Debug.DrawLine(ray.origin, hit.point, Color.yellow);
            //Debug.DrawLine(mainGun.transform.position, hit.point, Color.yellow);
        }
        else
        {
            hitPosition = ray.origin + ray.direction * 100;
            //Debug.DrawLine(ray.origin, hitPosition, Color.yellow);
           // Debug.DrawLine(mainGun.transform.position, hitPosition, Color.yellow);

        }
        Debug.DrawLine(ray.origin, hitPosition, Color.red);
        //Debug.DrawLine(mainGun.transform.position, hitPosition, Color.yellow);

        mainGunRotate(mainGun,hitPosition);
        /*mainGun.rotation = Quaternion.slerp(
          mainGun.rotation,
          Quaternion.LookatRotation(hit.point.position - mainGun.position),
          0.3f
        );*/
        //(Physics.Raycast(transform.position, fwd, 10)
        //mouseRotateY -= rh * rotateSpeed;           //计算当前摄像机的旋转角度

    }

    void mainGunRotate(GameObject mainGun,Vector3 targetPosition)
    {
        //Vector3 hitPointFzy = target + mainGun.transform.position;
        /*gunPivot.transform.LookAt(target);
        float rotate_angle = Quaternion.Angle(mainGun.transform.rotation, gunPivot.transform.rotation);
        Debug.Log("Angle:" + rotate_angle.ToString());
        transform.Rotate(normalize, Time.deltaTime);*/
        Quaternion rotation = Quaternion.LookRotation(targetPosition - mainGun.transform.position);
        mainGun.transform.rotation = Quaternion.Slerp(mainGun.transform.rotation, rotation, 0.05f);
        //mainGun.transform.rotation = Quaternion.Lerp(mainGun, gunPivot, 0.3f);
        //Vector3.Lerp(mainGun.transform, gunPivot.transform, 0.3f);
        //return;
        //Todo 坦克在斜面上的时候 forward或者X的角度限制改变
        /*Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, target);
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y,0.0f);*/
        //mainGun.transform.rotation = targetRotation;
        //Debug.Log(targetRotation.eulerAngles.z);
        /*float targetX = targetRotation.eulerAngles.x;
        while (targetX > 180) targetX -= 360;
        targetX = Mathf.Clamp(targetX, minMainGunRotateX, maxMainGunRotateX);
        float targetX = targetRotation.eulerAngles.x;
        Vector3 targetEulerAngles = new Vector3(
            targetX,
            targetRotation.eulerAngles.y,
            0.0f);*/

        //mainGun.transform.eulerAngles = targetEulerAngles;
        /* mainGun.transform.rotation = Quaternion.Lerp(mainGun.transform.rotation, targetRotation,//仍有问题 高度过高无法显示
             0.3f);*/
        /*if (mainGun.transform.rotation.eulerAngles.x > 180)
        {
            float x = mainGun.transform.rotation.eulerAngles.x - 180;
            float y = mainGun.transform.rotation.eulerAngles.y;
            mainGun.transform.eulerAngles = new Vector3(x, y, 0.0f);
        }
            
        Debug.Log("mainGunX" + mainGun.transform.rotation.eulerAngles.x);*/

        return;

    }
}






















