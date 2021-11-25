using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private Block block;
    [SerializeField] private int w = 10;
    [SerializeField] private int h = 20;
    private Block[,] blocks;


    private void Start()
    {
        init();
    }

    void init()
    {
        blocks = new Block[w, h];
        for (int i = 0; i < w; i++)
        
            for (int j = 0; j < h; j++)
            {
                var blockType = GetBlockType();
                blocks[i, j] = Instantiate(block, new Vector2(i, j), Quaternion.identity);
                blocks[i, j].blockType = blockType;
                blocks[i, j].i = i;
                blocks[i, j].j = j;
            }
        
        UpdateNearbyBlock();
        //RevealAll();
    }

    void UpdateNearbyBlock()
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                int n = 0;

                if (blocks[i, j].blockType == BlockType.Mine)
                {
                    continue;
                }

                var list = GetAllNearbyBlock(i, j);


                foreach (var block1 in list)
                {
                    if (block1.blockType == BlockType.Mine)
                    {
                        n++;
                    }
                }

                if (n > 0)
                {
                    blocks[i, j].blockType = (BlockType) (n + 7);
                }
            }
        }
    }

    BlockType GetBlockType()
    {
        if (Random.Range(1, 1000) % 5 == 0)
        {
            return BlockType.Mine;
        }
        else
        {
            return BlockType.Empty;
        }
    }

    Block GetBlock(int i, int j)
    {
        if (i < 0 || j < 0 || i>= w || j >= h)
        {
            return null;
        }

        return blocks[i, j];
    }

    List<Block> GetAllNearbyBlock(int i, int j, bool includeCorner = true)
    {
        List<Block> list = new List<Block>();
        list.Add(GetBlock(i + 1, j));
        list.Add(GetBlock(i, j + 1));
        list.Add(GetBlock(i - 1, j));
        list.Add(GetBlock(i, j - 1));

        if (includeCorner)
        {
            list.Add(GetBlock(i+1, j+1));
            list.Add(GetBlock(i-1, j-1));
            list.Add(GetBlock(i+1, j-1));
            list.Add(GetBlock(i-1, j+1));
        }

        return list
            .Where(x => x != null)
            .ToList();
    }

    void RevealAll()
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                blocks[i,j].Reveal();
            }
        }
    }

    public void RevealEmpty(int i, int j)
    {
        List<Block> list = GetAllNearbyBlock(i, j, false);
        list = list.Where(x => !x.show).ToList();

        foreach (var block1 in list)
        {
            if (block1.blockType == BlockType.Empty && !block1.show)
            {
                block1.Reveal();
                RevealEmpty(block1.i, block1.j);
            }
        }
    }

    public void Lose()
    {
        RevealAll();
    }
}
