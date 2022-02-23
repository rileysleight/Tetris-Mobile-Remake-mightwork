using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawn : MonoBehaviour
{

    [SerializeField]
    private GameObject[] tetrisBlocks;

    // Start is called before the first frame update
    void Start()
    {
        SpawnRandomBlock();
    }
    
    public void SpawnRandomBlock()
    {
        int num = Random.Range (0, tetrisBlocks.Length);
        Instantiate (tetrisBlocks[num], transform.position, Quaternion.identity);
    }

}
