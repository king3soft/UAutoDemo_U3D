using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    /// <summary>
    /// 低处射线发射起点
    /// </summary>
    public Transform lowRayPoint;
    /// <summary>
    /// 高处射线发射起点
    /// </summary>
    public Transform highRayPoint;

    /// <summary>
    /// 速度距离因子（危险距离随着速度增加多少）
    /// </summary>
    [Range(0,1)]
    public float speedMultiplier = 0.1f;
    /// <summary>
    /// 巡逻障碍物距离因子（巡逻障碍物的危险距离随着速度增加多少）
    /// </summary>
    [Range(1, 3)]
    public float patrollingMultiplier = 1;

    /// <summary>
    /// 基础危险距离
    /// </summary>
    protected float saveDistance = 3;


    /// <summary>
    /// 在上一次预测中得到的巡逻障碍物的实例
    /// </summary>
    protected PatrollingObstacle lastPatrollingObstacle;
    /// <summary>
    /// 上一次得到的巡逻障碍物的位置
    /// </summary>
    protected Vector3 lastPatrollingObstaclePostion;

    public static SimpleAI Instance;
    public bool enable = false;

    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     Time.timeScale = 2f;
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     Time.timeScale = 1;
        // }

        if (enable)
        {
          int lowSaveMask = FindSavePathMask(lowRayPoint, true);

          //Debug.Log($"lowSaveMask : {lowSaveMask}");

          int highSaveMask = FindSavePathMask(highRayPoint, false);
          int saveMask = lowSaveMask & highSaveMask;

          int consumableLena = FindConsumableLena();

          // 有安全的道路（切换道路）
          if (saveMask > 0)
          {
            //Debug.Log($"Have Save Lena : {saveMask}");
            int currentLena = TrackManager.instance.characterController.currentLane;
            int lenaMask = 1 << (2 - currentLena);

            // 如果安全道路跟可收集物重合，就重新设置安全道路为可收集物道路
            int consumableMask = 1 << (2 - consumableLena);
            /*if((saveMask & consumableMask) > 0)
            {
                if (Mathf.Abs(currentLena - consumableLena) != 2 || (saveMask & 2) != 0)
                {
                    saveMask &= consumableMask;
                }
            }*/
            int targetLena = -1;

            // 可收集物所在道路的安全是由道路是否安全
            // 玩家所在道路与可收集物道路之间的道路是否安全来决定
            bool consumableSave = (saveMask & consumableMask) > 0 &&
                                  (Mathf.Abs(currentLena - consumableLena) != 2 ||
                                   (saveMask & 2) != 0);

            if (consumableSave)
            {
              //Debug.Log("consumableSave");
              targetLena = consumableLena;
            }
            // 玩家不在安全道路上
            else if ((lenaMask & saveMask) == 0)
            {
              for (int i = 0; i <= 2; ++i)
              {
                lenaMask = 1 << (2 - i);
                if ((lenaMask & saveMask) > 0)
                {
                  targetLena = i;
                  break;
                }
              }
            }
            //Debug.Log($"saveMask : {saveMask}, targetLena : {targetLena}, currentLena: {currentLena}");
            /*if (lastSaveMask != saveMask)
            {

                lastSaveMask = saveMask;
            }*/

            if (targetLena != -1 && targetLena != currentLena)
            {
              // 如果当前道路与目标道路中间是危险道路，则使用跳跃跳过危险道路
              if (Mathf.Abs(currentLena - targetLena) == 2 && (saveMask & 2) == 0)
              {
                //TrackManager.instance.characterController.Slide();
                TrackManager.instance.characterController.Jump();
              }

              //Debug.Log($"Change Lena {saveLena}");
              TrackManager.instance.characterController.ChangeLane(targetLena - currentLena);
            }
          }
          else
          {
            // 有安全的下路（滑铲）
            if (lowSaveMask > 0)
            {
              TrackManager.instance.characterController.Slide();
            }
            // 有安全的上路（跳）
            else if (highSaveMask > 0)
            {
              //TrackManager.instance.characterController.Slide();
              TrackManager.instance.characterController.Jump();
            }

            if (consumableLena >= 0)
            {
              int currentLena = TrackManager.instance.characterController.currentLane;
              TrackManager.instance.characterController.ChangeLane(consumableLena - currentLena);
            }
          }
        }
    }

    public int FindConsumableLena()
    {
        int lena = -1;
        float minDistance = 5 + TrackManager.instance.speed * speedMultiplier;
        for(int i = -1; i <= 1; ++i)
        {
            RaycastHit hit;
            Vector3 origin = lowRayPoint.position + Vector3.right * TrackManager.instance.laneOffset * i;
            if(Physics.Raycast(origin, lowRayPoint.forward, out hit, 5 + TrackManager.instance.speed * speedMultiplier, 1 << LayerMask.NameToLayer("Coins")))
            {
                if(hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    lena = i + 1;
                }
            }
        }

        return lena;
    }


    public int FindSavePathMask(Transform origin, bool absoluteSave)
    {
        int mask = 0;

        float speedDistance = TrackManager.instance.speed * speedMultiplier;
        int layer = 1 << LayerMask.NameToLayer("Obstacle");

        // 判断三条道路是否安全
        for (int i = -1; i <= 1; ++i)
        {
            mask <<= 1;

            RaycastHit hit;
            Vector3 currentOrigin = origin.position + Vector3.right * TrackManager.instance.laneOffset * i;
            // 前方有障碍物
            if (Physics.Raycast(currentOrigin, origin.forward, out hit, 30 + speedDistance, layer))
            {

                // Debug.Log($"{hits[j].transform.gameObject.name} : {hits[j].distance}");
                // 障碍物在安全距离外，不需要避开
                if(hit.distance >= saveDistance + speedDistance)
                {
                    mask++;
                }
            }
            else
            {
                // 前方无障碍物，该道路安全
                mask++;
            }
        }

        // 对巡逻障碍物进行额外的判断
        if(absoluteSave)
        {
            // 在三条道路中间的两个间隔中判断是否安全
            int absoluteSaveMask = 1;
            for(int i = -1; i <= 1; i+=2)
            {
                RaycastHit hit;
                Vector3 currentOrigin = origin.position + Vector3.right * TrackManager.instance.laneOffset * i * 0.5f;
                if(Physics.Raycast(currentOrigin, origin.forward, out hit, 30+ speedDistance, layer))
                {
                    if (hit.distance < saveDistance + speedDistance)
                    {
                        mask &= absoluteSaveMask;

                    }
                }
                absoluteSaveMask <<= 2;
            }


            Debug.DrawLine(origin.position, origin.position + new Vector3(0, 0, saveDistance * patrollingMultiplier + speedDistance * patrollingMultiplier), Color.red);
            // 在中间对巡逻障碍物进行预测
            RaycastHit centerHit;
            if (Physics.Raycast(origin.position, origin.forward, out centerHit, 30 + speedDistance, layer))
            {
                PatrollingObstacle patrolling = centerHit.transform.GetComponent<PatrollingObstacle>();

                // 会移动的障碍物的安全区域更大，给角色更多时间处理危险
                if (patrolling && centerHit.distance < saveDistance * patrollingMultiplier + speedDistance * patrollingMultiplier)
                {
                    if(patrolling == lastPatrollingObstacle)
                    {
                        // 巡逻障碍物正在向右走
                        if(centerHit.transform.position.x > lastPatrollingObstaclePostion.x)
                        {
                            mask &= 4;
                        }
                        // 巡逻障碍物正在向左走
                        else
                        {
                            mask &= 1;
                        }
                    }
                    else
                    {
                        lastPatrollingObstacle = patrolling;
                        lastPatrollingObstaclePostion = centerHit.transform.position;
                    }
                }
            }
        }

        return mask;
    }
}
