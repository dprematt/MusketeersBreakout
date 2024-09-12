using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using PlayFab;
using UnityEngine.UI;
using static System.Random;
using System.Linq;

struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }