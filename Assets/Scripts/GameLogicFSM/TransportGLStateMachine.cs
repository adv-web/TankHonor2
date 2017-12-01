﻿using UnityEngine;
using System.Collections;

public class TransportGLStateMachine : BaseGLStateMachine
{
    const double transportAddDuration = 120.0;//推车状态持续时间
    private bool endFlag = false;//结束标记
    private bool timeoutFlag = false;//倒计时结束标记
    private string winTeam;//胜利队伍

    private CarController carController;//宝物车控制器
    private GameObject car;//宝物车实例

    private float remainDistance;//推车的剩余距离
    private float totalDistance;//总距离

    public TransportGLStateMachine() { }

    /* 学生作业：
     * 完成宝物运输状态机TransportGLStateMachine  Enter、Exit、update函数填充
     * 本代码实例中仅对 Enter、Exit、update3个函数提供了部分实现，请将这些函数填充完整，使运输状态可以正确执行
     */
    //进入状态机
    public override void Enter()
    {
		/* 学生作业：
         * 首先，启用怪物生成器（包括僵尸和骷髅）
         * 其次，更新UI面板，启用宝物车剩余距离UI
         * 接着，启用宝物车控制器
         * 然后，初始化推车的剩余距离remainDistance
         * 最后，根据玩家所属队伍，更新任务提示UI信息
         */
		GMInstance.zombieGenerator.generatorStartWorking();
		GMInstance.skeletonGenerator.generatorStartWorking ();
		GMInstance.UIController.enableRemainDistance (true);
		GMInstance.UIController.setMaxRemainDistance (totalDistance);
		carController.StartWorking ();
        this.startCountDown(transportAddDuration + GameManager.gm.remainTime + PhotonNetwork.time);
		remainDistance = totalDistance;
		if (GMInstance.tankHealth.team== "AttackerTeam")
			GMInstance.UIController.updateProcessText("将宝物车运送到目的地!");
		else
			GMInstance.UIController.updateProcessText("阻止宝物车的运送!");
    }
    //状态机初始化
    public override void Init()
    {
     
        car = GameObject.Find("Car");
        carController = car.GetComponent<CarController>();
        totalDistance = carController.trailReference.spline.Length() - 0.1f;
    }
    //退出状态机
    public override void Exit()
    {
        /* 学生作业：
         * 停止怪物生成器的工作（包括僵尸和骷髅）
         * 停止宝物车控制器
         * 游戏结束时，为了保证宝物车的剩余距离为0，强制更新剩余距离UI为0米
         */
		GMInstance.zombieGenerator.generatorStopWorking();
		GMInstance.skeletonGenerator.generatorStopWorking ();
		carController.enabled = false;
        GMInstance.gameStateQueue.Enqueue("end");//将结算状态机添加到队列中
		if(GMInstance.winTeam == "AttackerTeam")
			GMInstance.UIController.updateRemainDistanceUI(0.00f);
    }
    //每帧更新
    public override void Update()
    {
        base.Update();
        //更新倒计时时间的显示
        GMInstance.UIController.updateTimeLabelForProcess((int)m_countDown);
        GMInstance.UIController.updatePlayerHPLabel(GMInstance.tankHealth.currentHP);

		/* 学生作业：
		 * 计算宝物车剩余距离
		 * 更新剩余距离UI
		 */
		GMInstance.UIController.updateRemainDistanceUI(remainDistance);
#if (!UNITY_ANDROID)
        GMInstance.UIController.scorePanelEnable(Input.GetKey(KeyCode.Tab));
        if (GMInstance.lockCursor)
            GMInstance.UIController.InternalLockUpdate();
#endif
		calRemainDistance();
        if (PhotonNetwork.isMasterClient)
        {
			/* 学生作业：
			 * 倒计时结束，宝物车没有到达终点，保卫者胜利
			 * 规定时间内，车到达了终点，盗墓者胜利
			 */
			if (timeoutFlag) {
				winTeam = "DefenderTeam";
				endFlag = true;
			} else if (carController.carReachedDestination ()) 
			{
				winTeam = "AttackerTeam";
				endFlag = true;
			}
            if (winTeam != null)
                GMInstance.photonView.RPC("notifyGameResult", PhotonTargets.All, winTeam);//通知游戏结果
        }
        
    }
    //重写倒计时结束函数
    public override void onCountDownFinish()
    {
        timeoutFlag = true;
    }
    //返回状态机结束标记
    public override bool checkEndCondition()
    {
        return endFlag;
    }
    //计算剩余距离
    public void calRemainDistance()
    {
        remainDistance = totalDistance - carController.distance;
    }

}
