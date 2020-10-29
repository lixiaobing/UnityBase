using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get;
            protected set;
        }

        private LuaManager lua = null;

        public LuaManager LuaMgr
        {
            get
            {
                return lua;
            }
        }

        private ResManager res = null;

        public ResManager ResMgr
        {
            get
            {
                return res;
            }
        }

        private NetManager net = null;
        public NetManager NetMgr
        {
            get
            {
                return net;
            }
        }


        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            res = this.gameObject.AddComponent<ResManager>();
            lua = this.gameObject.AddComponent<LuaManager>();
            net = this.gameObject.AddComponent<NetManager>();
        }

        IEnumerator Start()
        {
                 yield return StartGame();
        }

        IEnumerator StartGame()
        {
            lua.Init();
            yield return null;
        }
    }
}
