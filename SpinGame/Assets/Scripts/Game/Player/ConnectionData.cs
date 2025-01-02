using Unity.Collections;
using Unity.Netcode;
using System;

[Serializable]
public struct ConnectionData : INetworkSerializable, IEquatable<ConnectionData>
{
    public ulong clientID;
    public FixedString32Bytes username;
    public int skinId;
    public int score;
    public bool isHost;

    public ConnectionData(ulong id, FixedString32Bytes name, int skinid)
    {
        clientID = id;
        username = name;
        skinId = skinid;
        score = 0;
        isHost = false;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref skinId);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref isHost);
    }
    public bool Equals(ConnectionData other)
    {
        return clientID == other.clientID;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(clientID, username, skinId,score, isHost);
    }
    public override bool Equals(object obj)
    {
        return obj is ConnectionData other && Equals(other);
    }
}