using System;
using App.Scripts.Netcode.Helpers;

namespace App.Scripts.Netcode.Interfaces {
    public interface IUninitialize {
        public void Uninitialize(Action<ResultData> onComplete = null);
    }
}
