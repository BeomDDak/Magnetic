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
        PlayerSync,     // 플레이어 상태 동기화를 위한 타입

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
