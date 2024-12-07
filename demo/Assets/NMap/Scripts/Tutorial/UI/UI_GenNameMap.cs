using System;
using UnityEngine;
using Assets.Map;
using ET;
using Unity.Mathematics;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class UI_GenNameMap : MonoBehaviour
{

    private InputField _inputName;
    private Button _btnGen;
	// Use this for initialization
    private Font _dFont;
    private RawImage _image;
    private Text _mouseBiome;
    private GameObject _showMap;
	void Start ()
	{
        _inputName = transform.Find("inputName").GetComponent<InputField>();
        _btnGen = transform.Find("btnGen").GetComponent<Button>();
        _image = transform.Find("RawImage").GetComponent<RawImage>();
        _mouseBiome = transform.Find("MouseBiome").GetComponent<Text>();
	    _dFont = _inputName.textComponent.font;

        _btnGen.onClick.AddListener(GenMap);

	    _showMap = GameObject.Find("Map");

        transform.Find("Toggles1/Toggle1").GetComponent<Toggle>().onValueChanged.AddListener(Toggle1);
        transform.Find("Toggles1/Toggle2").GetComponent<Toggle>().onValueChanged.AddListener(Toggle2);
        transform.Find("Toggles1/Toggle3").GetComponent<Toggle>().onValueChanged.AddListener(Toggle3);
        transform.Find("Toggles1/Toggle4").GetComponent<Toggle>().onValueChanged.AddListener(Toggle4);

        transform.Find("Toggles2/Toggle1").GetComponent<Toggle>().onValueChanged.AddListener(ToggleLand);
        transform.Find("Toggles2/Toggle2").GetComponent<Toggle>().onValueChanged.AddListener(ToggleLake);
	}

    void Update()
    {
        CheckMouseBiome();
    }

    private float _nextCheckTime;
    private void CheckMouseBiome()
    {
        if(Time.time < _nextCheckTime)
            return;
        _nextCheckTime = Time.time + 0.1f;

        Vector2 pos = Input.mousePosition; // Mouse position
        RaycastHit hit;
        Camera _cam = Camera.main; // Camera to use for raycasting
        Ray ray = _cam.ScreenPointToRay(pos);
        Physics.Raycast(_cam.transform.position, ray.direction, out hit, 10000.0f);
        Color c;
        if (hit.collider)
        {
            Texture2D tex = (Texture2D)hit.collider.gameObject.GetComponent<Renderer>().material.mainTexture; // Get texture of object under mouse pointer
            if (tex)
            {
                c = tex.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y); // Get color from texture

                Biome b = ChangeColorToBiome(c);
                _mouseBiome.text = BiomeProperties.Chinese[b];
            }
        }
    }

    private Biome ChangeColorToBiome(Color color)
    {
        Biome b = Biome.Ocean;
        foreach (var bc in BiomeProperties.Colors)
        {
            if (ColorNearby(bc.Value , color))
            {
                b = bc.Key;
                break;
            }
        }
        return b;
    }

    bool ColorNearby(Color ls, Color rs)
    {
        bool rSame = Mathf.Abs(ls.r - rs.r) < 0.02f;
        bool gSame = Mathf.Abs(ls.g - rs.g) < 0.02f;
        bool bSame = Mathf.Abs(ls.b - rs.b) < 0.02f;
        return rSame && gSame && bSame;
    }

    private static Texture2D _txtTexture;
    const int TextureScale = 20;
    private const int Width = 100;
    private const int Height = 50;
    private int _pointNum = 1000;
    private static bool _isLake = true;
    void Toggle1(bool check)
    {
        if (check)
            _pointNum = 1000;
    }
    void Toggle2(bool check)
    {
        if (check)
            _pointNum = 2000;
    }
    void Toggle3(bool check)
    {
        if (check)
            _pointNum = 3000;
    }
    void Toggle4(bool check)
    {
        if (check)
            _pointNum = 5000;
    }
    void ToggleLand(bool check)
    {
        if (check)
            _isLake = false;
    }
    void ToggleLake(bool check)
    {
        if (check)
            _isLake = true;
    }

    private uint MapSeed = 1;
    private void GenMap()
    {
        _txtTexture = GetTextTexture();

        Map.Width = Width;
        Map.Height = Height;
        Map map = new Map();
        map.SetPointNum(_pointNum);
        map.Init(MapSeed, CheckIsland());
        //扰乱边缘
        NoisyEdges noisyEdge = new NoisyEdges();
        noisyEdge.BuildNoisyEdges(map);

        var mapg = GameObject.Find("Map");
        mapg.GetComponent<DrawMap>().GenMap(map);
    }
    public static System.Func<float2, bool> CheckIsland()
    {
        System.Func<float2, bool> inside = q =>
        {
            int x = Convert.ToInt32(q.x / Width * _txtWidth);
            int y = Convert.ToInt32(q.y / Height * _txtHeight);
            Color tColor = _txtTexture.GetPixel(x,y);
            bool isLand = false;
            if (_isLake)
                isLand = tColor != Color.white;
            else
                isLand = tColor == Color.white;
            return isLand;
        };
        return inside;
    }

    private static int _txtWidth = 400;
    private static int _txtHeight = 200;
    private Texture2D GetTextTexture()
    {
        Texture2D output = new Texture2D(_txtWidth, _txtHeight);
        RenderTexture renderTexture = new RenderTexture(_txtWidth, _txtHeight, 24);
        RenderTexture.active = renderTexture;
        Camera myCamera = Camera.main;
        myCamera.orthographic = true;
        myCamera.orthographicSize = 100;
        myCamera.targetTexture = renderTexture;

        _showMap.SetActive(false);
        myCamera.Render();
        _showMap.SetActive(true);

        output.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        output.Apply();
        RenderTexture.active = null;

        //_image.texture = renderTexture;
        myCamera.targetTexture = null;
        myCamera.orthographic = false;
        myCamera.Render();
        return output;
    }
}
