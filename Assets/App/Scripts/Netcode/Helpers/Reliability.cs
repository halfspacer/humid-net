/// <summary>
/// Types of packet reliability.
/// 
/// Ordered packets will only be ordered relative to other ordered packets.
/// </summary>
public enum PacketReliability : int
{
    /// <summary>
    /// Packets will only be sent once and may be received out of order
    /// </summary>
    UnreliableUnordered = 0,
    /// <summary>
    /// Packets may be sent multiple times and may be received out of order
    /// </summary>
    ReliableUnordered = 1,
    /// <summary>
    /// Packets may be sent multiple times and will be received in order
    /// </summary>
    ReliableOrdered = 2
}
