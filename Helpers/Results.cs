namespace App.Scripts.Netcode.Helpers {
    public enum Results {
        Success,
        Failure,
        InProgress,
        Canceled,
        Unknown,
        NotSupported,
        NotInitialized,
        NotAvailable,
        NotAuthorized,
        NotReady,
        NotConnected,
        NotInstalled,
    }
    
    public struct ResultData {
        public Results result;
        public string message;
    }
}