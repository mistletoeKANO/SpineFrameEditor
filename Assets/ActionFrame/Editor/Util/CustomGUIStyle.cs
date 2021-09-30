using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ENode.CustomGUIStyle
{
    public class CustomGUIStyle : EditorWindow
    {
        private IMGUIContainer GraphView;
        private VisualElement GraphBG;
        
        private Object[] _Icons;
        private static GUIStyle[] Styles => GUI.skin.customStyles.OrderBy(item => item.name).ToArray();
        private static GUIStyle Line => "sv_iconselector_sep";
        
        private Vector2 _scrollPos = Vector2.zero;

        [MenuItem("Tool/Unity内置样式")]
        private static void GetCustomWindow()
        {
            GetWindow<CustomGUIStyle>(typeof(SceneView)).Focus();
        }

        private void OnEnable()
        {
            this.titleContent = new GUIContent("Unity内置样式预览");
            this.InitData();
            this.InitView();
        }
        private void InitData()
        {
            this._Icons = Resources.FindObjectsOfTypeAll(typeof(Texture)).OrderBy(item => item.name).ToArray();
        }

        private void InitView()
        {
            var uss = Resources.Load<StyleSheet>("Uss/StyleWindowS");
            this.rootVisualElement.styleSheets.Add(uss);

            this.GraphBG = new IMGUIContainer(() =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    UnityEditor.Graphs.Styles.graphBackground.Draw(this.GraphBG.layout, false, false, false, false);
                }
            });
            this.GraphBG.name = "GraphBG";
            this.rootVisualElement.Add(GraphBG);
            
            this.GraphView = new IMGUIContainer(this.DrawCustomStyle);
            this.GraphView.name = "GraphView";
            
            this.rootVisualElement.Add(GraphView);
        }

        private void DrawCustomStyle()
        {
            this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos);
            foreach (var style in Styles)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("", style, GUILayout.Width(160));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("拷贝", GUILayout.Width(160)))
                {
                    UnityEngine.GUIUtility.systemCopyBuffer = style.name;
                }
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                GUILayout.Space(12);
                GUILayout.Box("", Line, GUILayout.Height(2));
                GUILayout.Space(4);
            }
            
            int iconWidth = 60;
            float width = this.GraphView.contentContainer.layout.width;
            int col = Mathf.FloorToInt(width / (iconWidth));
            float headSpace = (width - col * (iconWidth)) / 2f;
            int row = Mathf.FloorToInt((float) this._Icons.Length / col);
            for (int i = 0; i < row; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(headSpace);
                for (int j = 0; j < col; j++)
                {
                    int index = i * col + j;
                    if (index >= this._Icons.Length) return;
                    Texture2D cur = this._Icons[index] as Texture2D;
                    if (cur == null) continue;
                    
                    if (GUILayout.Button(cur, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth)))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("拷贝"), false, () =>
                        {
                            UnityEngine.GUIUtility.systemCopyBuffer = cur.name;
                        });
                        menu.ShowAsContext();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
}