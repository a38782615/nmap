using System;
using UnityEngine;
using System.Collections;
using Assets.Map;
using Random = UnityEngine.Random;

public class UI_Main : MonoBehaviour
{
    private uint MapSeed = 1;
    const int TextureScale = 20;
	// Use this for initialization
    void Start()
    {
    }

    #region Tutorial
    void GenMap0()
    {
        Map map = new Map();
        map.Init(MapSeed);
        //扰乱边缘
        NoisyEdges noisyEdge = new NoisyEdges();
        noisyEdge.BuildNoisyEdges(map);

        new MapTexture(TextureScale).AttachTexture(GameObject.Find("Map"), map, noisyEdge);
    }

    private void GenMap1()
    {
        Map1 map = new Map1(MapSeed);

        new MapTexture1(TextureScale).AttachTexture(GameObject.Find("Map"), map);
    }

    private void GenMap2()
    {
        Map1 map = new Map1(MapSeed,true);

        new MapTexture1(TextureScale).AttachTexture(GameObject.Find("Map"), map);
    }

    private void GenMap3()
    {
        Map1 map = new Map1(MapSeed,true);

        new MapTexture1(TextureScale).DrawTwoGraph(GameObject.Find("Map"), map);
    }

    private void GenMap4()
    {
        Map2 map = new Map2();
        map.Init(MapSeed);
        new MapTexture2(TextureScale).AttachTexture(GameObject.Find("Map"), map);
    }

    private void GenMap5()
    {
        Map2 map = new Map2();
        map.Init(MapSeed);
        new MapTexture2(TextureScale).ShowElevation(GameObject.Find("Map"), map);
    }

	void GenMap6()
	{
        Map2 map = new Map2();
        map.Init(MapSeed);
		new MapTexture2(TextureScale).ShowRivers(GameObject.Find("Map"), map);
	}
	
	void GenMap7()
	{
        Map2 map = new Map2();
        map.Init(MapSeed);
		new MapTexture2(TextureScale).DrawMoisture(GameObject.Find("Map"), map);
	}

    void GenMap8()
    {
        Map2 map = new Map2();
        map.Init(MapSeed);
        new MapTexture2(TextureScale).DrawBiome(GameObject.Find("Map"), map);
    }
    #endregion

    #region UI
    public void ResetSeed()
    {
        MapSeed = (uint)DateTime.Now.Ticks;
    }
    public void ClickGenMap(int index)
    {
		gameObject.SendMessage ("GenMap" + index);
    }

    #endregion
}
