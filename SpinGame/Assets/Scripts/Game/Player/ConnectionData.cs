using Unity.Collections;
using Unity.Netcode;
using System;

[Serializable]
public struct ConnectionData : INetworkSerializable, IEquatable<ConnectionData>
{
    public ulong clientID;
    public FixedString32Bytes username;
    public int skinId;

    public ConnectionData(ulong id, FixedString32Bytes name, int skinid)
    {
        clientID = id;
        username = name;
        skinId = skinid;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref skinId);
    }

    // Método requerido por IEquatable<T>
    public bool Equals(ConnectionData other)
    {
        return clientID == other.clientID;
    }

    // Sobrescribir GetHashCode para consistencia
    public override int GetHashCode()
    {
        return HashCode.Combine(clientID, username, skinId);
    }

    // Sobrescribir Equals para compatibilidad con object
    public override bool Equals(object obj)
    {
        return obj is ConnectionData other && Equals(other);
    }
}