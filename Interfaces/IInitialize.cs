using System;
using App.Scripts.Netcode.Helpers;

namespace App.Scripts.Netcode.Interfaces {
    public interface IInitialize {
        public void Initialize(Action<ResultData> onComplete = null);
    }
}
