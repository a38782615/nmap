using UnityEngine;
using ET;

public class Main : MonoBehaviour
{
    BiomeMap _biomeMap;
    const int _textureScale = 10;
    GameObject _selector;

//    void Update()
//    {
//        if (_map != null && _map.SelectedCenter != null)
//        {
//            _selector.transform.localPosition = new Vector3(_map.SelectedCenter.point.x, _map.SelectedCenter.point.y, 1);
//        }
//    }

	void Awake ()
	{
        _selector = GameObject.Find("Selector");

        //Random.seed = 1;
            
        _biomeMap = new BiomeMap();
        _biomeMap.Init(1);

        GameObject.Find("Main MyCamera").GetComponentInChildren<MyCamera>().BiomeMap = _biomeMap;

        NoisyEdges noisyEdge = new NoisyEdges();
        noisyEdge.BuildNoisyEdges(_biomeMap);

        new MapTexture(_textureScale).AttachTexture(GameObject.Find("Map"), _biomeMap,noisyEdge);
	}
}