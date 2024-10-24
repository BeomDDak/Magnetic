using BackEnd.Tcp;
using UnityEngine;
using System.Collections.Generic;

namespace Protocol
{
    // 이벤트 타입
    public enum Type : sbyte
    {
        GameStart,
        GameEnd,
        StonePlacement,
        StoneSync,      // 돌의 상태 동기화
        PlayerSync,     // 플레이어 상태 동기화

    }

    public class Message
    {
        public Type type;

        public Message(Type type)
        {
            this.type = type;
        }
    }

    public class GameStartMessage : Message
    {
        public GameStartMessage() : base(Type.GameStart)
        {
            
        }
    }

    public class StonePlacementMessage : Message
    {
        public float posX;
        public float posY;
        public float posZ;
        public int stoneIndex;
        public string player;

        public StonePlacementMessage(Vector3 pos, int index, string currentPlayer) : base(Type.StonePlacement)
        {
            this.posX = pos.x;
            this.posY = pos.y;
            this.posZ = pos.z;
            this.stoneIndex = index;
            this.player = currentPlayer;
        }
    }

    public class StoneSyncMessage : Message
    {
        public string stoneId;
        public float posX, posY, posZ;           // 위치
        public float rotX, rotY, rotZ, rotW;     // 회전
        public float velX, velY, velZ;           // 속도
        public float angVelX, angVelY, angVelZ;  // 각속도
        public List<string> attachedStoneIds;    // 붙어있는 돌들의 ID

        public StoneSyncMessage(
            string id,
            Vector3 position,
            Quaternion rotation,
            Vector3 velocity,
            Vector3 angularVelocity,
            List<string> attachedIds) : base(Type.StoneSync)
        {
            stoneId = id;
            posX = position.x; posY = position.y; posZ = position.z;
            rotX = rotation.x; rotY = rotation.y; rotZ = rotation.z; rotW = rotation.w;
            velX = velocity.x; velY = velocity.y; velZ = velocity.z;
            angVelX = angularVelocity.x; angVelY = angularVelocity.y; angVelZ = angularVelocity.z;
            attachedStoneIds = attachedIds;
        }
    }

    public class PlayerSyncMessage : Message
    {
        public Dictionary<string, int> playerStones;  // 각 플레이어의 남은 돌 수

        public PlayerSyncMessage(Dictionary<Define.Player, int> stoneCount) : base(Type.PlayerSync)
        {
            playerStones = new Dictionary<string, int>();
            foreach (var pair in stoneCount)
            {
                playerStones[pair.Key.ToString()] = pair.Value;
            }
        }
    }
}
