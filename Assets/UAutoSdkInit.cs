using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TcpServer;
using UnityEngine;
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
    }
}