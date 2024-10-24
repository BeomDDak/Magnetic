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
        StoneSync,      // ���� ���� ����ȭ
        PlayerSync,     // �÷��̾� ���� ����ȭ

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
        public float posX, posY, posZ;           // ��ġ
        public float rotX, rotY, rotZ, rotW;     // ȸ��
        public float velX, velY, velZ;           // �ӵ�
        public float angVelX, angVelY, angVelZ;  // ���ӵ�
        public List<string> attachedStoneIds;    // �پ��ִ� ������ ID

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
