using System;
using App.Scripts.Netcode.Helpers;

namespace App.Scripts.Netcode.Interfaces {
    public interface IAuthenticate {
        public void Authenticate(Action<ResultData> callback = null);
        public Action<ResultData> OnAuthenticateComplete { get; set; }
    }
}
