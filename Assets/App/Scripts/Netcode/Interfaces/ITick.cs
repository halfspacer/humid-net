namespace App.Scripts.Netcode.Interfaces {
    public interface ITick {
        
        /// <summary>
        /// Tick is called regularly to poll for new data.
        /// Use this to update the state of any remote resources.
        /// </summary>
        public void Tick();
    }
}
