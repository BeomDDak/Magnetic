using BackEnd.Tcp;
using UnityEngine;
using System.Collections.Generic;

namespace Protocol
{
    // �̺�Ʈ Ÿ��
    public enum Type : sbyte
    {
        GameStart,
        GameEnd,
        StonePlacement,
        PlayerSync,     // �÷��̾� ���� ����ȭ�� ���� Ÿ��

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
        public Dictionary<string, int> playerStones;  // �� �÷��̾��� ���� �� ��

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
