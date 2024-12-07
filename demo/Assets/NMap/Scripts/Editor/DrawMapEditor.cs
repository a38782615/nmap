// using UnityEditor;
// using UnityEngine;
//
// namespace ET.ETEditor
// {
//     /// <summary>
//     /// Unity Event.current.clickCount is not always working as intended, so this class is used to check mouse double clicks safely
//     /// </summary>
//     internal class MouseDblClick
//     {
//         public bool IsDblClick { get { return m_isDblClick; } }
//         float m_lastClickTime = 0f;
//         bool m_isDblClick = false;
//
//         public void Update()
//         {
//             Event e = Event.current;
//             m_isDblClick = false;
//             if (e.isMouse && e.type == EventType.MouseDown)
//             {
//                 m_isDblClick = (Time.realtimeSinceStartup - m_lastClickTime) <= 0.2f;
//                 m_lastClickTime = Time.realtimeSinceStartup;
//             }
//         }
//     }
//     [CanEditMultipleObjects]
//     [CustomEditor(typeof(DrawMap))]
//     public partial class DrawMapEditor : Editor
//     {
//         [MenuItem("GameObject/DrawMapEditor/DrawMap", false, 11)]
//         static void CreateTilemap(MenuCommand menuCommand)
//         {
//             GameObject obj = new GameObject("New Tilemap");
//             obj.AddComponent<DrawMap>();
//             if (menuCommand.context is GameObject)
//                 obj.transform.SetParent((menuCommand.context as GameObject).transform);
//             Selection.activeGameObject = obj;
//         }
//         
//         private DrawMap m_tilemap;
//         void OnEnable()
//         {
//             m_tilemap = (DrawMap)target;
//         }
//
//         void OnDisable()
//         {
//             m_tilemap = null;
//         }
//     }
// }