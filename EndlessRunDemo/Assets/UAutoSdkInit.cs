using System;
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

                    runer.m_Handlers.addMsgHandler("startAutoRun", startAutoRunHandler);
                    runer.m_Handlers.addMsgHandler("stopAutoRun", stopAutoRunHandler);

                    runer.Run();
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[ERROR] UAutoSDk Reg MsgHandler Error. {0}", e.ToString());
            }
        }

        private object startAutoRunHandler(string[] args)
        {
          try
          {
            SimpleAI.Instance.enable = true;
            return "success";
          }
          catch (Exception e)
          {
            return e.ToString();
          }
        }

        private object stopAutoRunHandler(string[] args)
        {
          try
          {
            SimpleAI.Instance.enable = false;
            return "success";
          }
          catch (Exception e)
          {
            return e.ToString();
          }
        }
  }
}
