using System;
using System.Collections.Generic;
namespace Bcp
{
    public static class Bcp
    {
        public const uint MaxOfflinePack = 200;
        public const uint MaxConnectionsPerSession = 3;
        public const uint HeartBeatDelayMilliseconds = 3000;
        public const uint ReadingTimeoutMilliseconds = 6000;
        public const uint WritingTimeoutMilliseconds = 1000;
        public const int NumBytesSessionId = 16;
        public const uint MaxDataSize = 10000;

        public struct ConnectionHead
        {
            public readonly byte[] SessionId;
            public readonly uint ConnectionId;
            public ConnectionHead(byte[] sessionId, uint connectionId)
            {
                SessionId = sessionId;
                ConnectionId = connectionId;
            }
        }

        public interface IPacket { }
        public interface IServerToClient : IPacket { }
        public interface IClientToServer : IPacket { }
        public interface IAcknowledgeRequired : IPacket { }
        public interface IRetransmission : IPacket
        {
            uint ConnectionId { get; }
            uint PackId { get; }
        }

        public struct Data : IClientToServer, IServerToClient, IAcknowledgeRequired
        {
            public const byte HeadByte = 0;

            public readonly IList<ArraySegment<byte>> Buffers;

            public Data(IList<ArraySegment<byte>> buffers)
            {
                Buffers = buffers;
            }
        }

        public struct Acknowledge : IClientToServer, IServerToClient
        {
            public const byte HeadByte = 1;
        }

        public struct RetransmissionData : IClientToServer, IServerToClient, IAcknowledgeRequired, IRetransmission
        {
            public const byte HeadByte = 2;

            public readonly uint ConnectionId;
            public readonly uint PackId;
            public readonly IList<ArraySegment<Byte>> Buffers;

            public RetransmissionData(uint connectionId, uint packId, IList<ArraySegment<Byte>> buffers)
            {
                ConnectionId = connectionId;
                PackId = packId;
                Buffers = buffers;
            }

            uint IRetransmission.ConnectionId { get { return ConnectionId; } }
            uint IRetransmission.PackId { get { return PackId; } }
        }

        public struct Renew : IClientToServer
        {
            public const byte HeadByte = 3;
        }

        public struct Finish : IClientToServer
        {
            public const byte HeadByte = 5;
        }

        public struct RetransmissionFinish : IClientToServer, IServerToClient, IAcknowledgeRequired, IRetransmission
        {
            public const byte HeadByte = 6;

            public readonly uint ConnectionId;
            public readonly uint PackId;

            public RetransmissionFinish(uint connectionId, uint packId)
            {
                ConnectionId = connectionId;
                PackId = packId;
            }

            uint IRetransmission.ConnectionId { get { return ConnectionId; } }
            uint IRetransmission.PackId { get { return PackId; } }
        }

        public struct ShutDown : IClientToServer, IServerToClient
        {
            public const byte HeadByte = 7;
        }

        public struct HeartBeat : IClientToServer, IServerToClient
        {
            public const byte HeadByte = 8;
        }
    }
}

