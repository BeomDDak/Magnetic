using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneManager
{
    /*
    private GameObject[] stonePrefabs;
    private List<Stone> activeStones = new List<Stone>();
    private Magnet magnetManager;

    public StoneManager(GameObject[] prefabs, Magnet magnetManager)
    {
        stonePrefabs = prefabs;
        this.magnetManager = magnetManager;
    }

    public GameObject CreateStone(int stoneType, Vector3 position)
    {
        GameObject stoneObject = Object.Instantiate(stonePrefabs[stoneType], position, Quaternion.identity);
        Stone stone = stoneObject.GetComponent<Stone>();
        activeStones.Add(stone);
        return stoneObject;
    }

    public void TriggerMagneticEffect(Vector3 center)
    {
        magnetManager.ApplyMagneticEffect(center, activeStones);
    }

    public void RemoveStone(Stone stone)
    {
        activeStones.Remove(stone);
    }

    public List<Stone> GetActiveStones()
    {
        return activeStones;
    }
    */
}
