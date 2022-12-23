using System.Threading;

namespace BSN.Resa.DoctorApp.Data.ServiceCommunicators
{
    public enum ConnectionStatus
    {
        Started, Ended
    }

    public enum AuthenticationStatus
    {
        Unauthorized
    }

    public delegate void ConnectionStatusChanged(ConnectionStatus status);

    public delegate void AuthenticationStatusChanged(AuthenticationStatus status);

    public class ConnectionStatusManager
    {
        internal void StartConnection()
        {
            Interlocked.Increment(ref _connectionsCount);
            if (_connectionsCount == 1)
                ConnectionStatusChanged?.Invoke(ConnectionStatus.Started);
        }

        internal void EndConnection()
        {
            Interlocked.Decrement(ref _connectionsCount);
            if (_connectionsCount == 0)
                ConnectionStatusChanged?.Invoke(ConnectionStatus.Ended);
        }

        internal void SetAuthenticationStatus(AuthenticationStatus status)
        {
            OnAuthenticationStatusChanged?.Invoke(AuthenticationStatus.Unauthorized);
        }

        public event ConnectionStatusChanged ConnectionStatusChanged;

        public event AuthenticationStatusChanged OnAuthenticationStatusChanged;

        public bool HasAnyOpenConnection => !Interlocked.Equals(_connectionsCount, 0);

        private int _connectionsCount = 0;
    }
}
