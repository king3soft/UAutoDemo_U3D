using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
namespace UAutoSDK
{
    public class UAutoSdkInit : MonoBehaviour
    {
        void Awake()
        {
            UAutoRuner runer = gameObject.AddComponent<UAutoRuner>();
            try
            {
                if (runer != null)
                {
                    runer.Init();
                    runer.Run();
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[ERROR] UAutoSDk Reg MsgHandler Error. {0}", e.ToString());
            }
        }

    private void Update()
    {
      if(Input.GetMouseButtonDown(0))
      {
        Profiler.SetAreaEnabled(0, true);
        Profiler.SetAreaEnabled((ProfilerArea)2, true);
        Profiler.SetAreaEnabled((ProfilerArea)1, true);
        Profiler.SetAreaEnabled((ProfilerArea)3, true);
        Profiler.SetAreaEnabled((ProfilerArea)6, true);
        Profiler.SetAreaEnabled((ProfilerArea)10, true);
        Profiler.SetAreaEnabled((ProfilerArea)11, true);
        Profiler.SetAreaEnabled((ProfilerArea)12, true);
        Profiler.logFile = Application.persistentDataPath + "/"+ "test";
        Profiler.enableBinaryLog = true;
        Profiler.enabled = true;
        Profiler.maxUsedMemory = 1024 * 1024 * 1024;
      }
      if(Input.GetMouseButtonUp(0))
      {
        Profiler.enabled = false;
        Profiler.logFile = "";
        Profiler.enableBinaryLog = false;
      }
    }
  }
}
