using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    private bool isAlarm;//控制警报是否开始
    private float AnimSpeed = 10;//警报切换速度
    private float lowIntensityt = 0;//最小值
    private float hightIntensity = 10f;//最大值
    private float TargetIntensity;//用来变化的值
    private Light alarmLight;
    public GameObject Cube;

    // Start is called before the first frame update
    void Start()
    {
        TargetIntensity = hightIntensity;//默认最亮
       // isAlarm = true;
        alarmLight = GetComponent<Light>();//脚本赋值
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isAlarm)
        {
            //duration += Time.deltaTime;
            //if (duration > 1) 
            //{
            //    isAlarm = false;
            //}
            //报警状态
            alarmLight.intensity = Mathf.Lerp(alarmLight.intensity, TargetIntensity, AnimSpeed * Time.deltaTime);
            //随着每帧的变化而变化
            if (Mathf.Abs(alarmLight.intensity - TargetIntensity) < 0.01f)
            {
                //近似相等
                if (hightIntensity == TargetIntensity)
                {
                    TargetIntensity = lowIntensityt;
                }
                else if (TargetIntensity == lowIntensityt)
                {
                    TargetIntensity = hightIntensity;
                }
            }
        }
        if (!isAlarm) 
        {
            alarmLight.intensity = 0;

        }

    }
    //float duration = 0;
    public void SetAlarm() 
    {
        isAlarm = true;
        //duration = 0;
    }
    public void CloseAlarm()
    {
        isAlarm = false;
        //duration = 0;
    }

}
