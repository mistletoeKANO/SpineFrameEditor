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
        private ScrollView GraphView;
        private VisualElement GraphBG;
        
        private Object[] _Icons;
        private GUIStyle[] _Styles;
        private GUIStyle _Line = "sv_iconselector_sep";

        [MenuItem("Tool/Unity内置样式")]
        private static void GetCustomWindow()
        {
            GetWindow<CustomGUIStyle>(typeof(SceneView)).Focus();
        }

        private void OnEnable()
        {
            this.titleContent = new GUIContent("Unity内置样式预览");
            this.InitData();
        }
        private void InitData()
        {
            this._Icons = Resources.FindObjectsOfTypeAll(typeof(Texture)).OrderBy(item => item.name).ToArray();
        }

        private void OnGUI()
        {
            if (this._Styles == null)
            {
                this._Styles = GUI.skin.customStyles.OrderBy(item => item.name).ToArray();
                this.InitView();
            }
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
            
            this.GraphView = new ScrollView();
            this.GraphView.name = "GraphView";
            
            this.DrawCustomStyle();
            this.DrawTexture();
            this.rootVisualElement.Add(GraphView);
        }

        private void DrawCustomStyle()
        {
            foreach (var style in this._Styles)
            {
                VisualElement item = new IMGUIContainer(() =>
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
                    GUILayout.Box("", this._Line, GUILayout.Height(2));
                    GUILayout.Space(4);
                });
                item.name = "StyleItem";
                this.GraphView.Add(item);
            }
        }

        private void DrawTexture()
        {
            int iconWidth = 60;
            VisualElement textureItem = new IMGUIContainer(() =>
            {
                float width = this.GraphView.layout.width;
                float space = (width % (iconWidth + 10)) * (iconWidth + 10) / 100f;
                int col = Mathf.CeilToInt(width / (iconWidth + 10));
                int row = Mathf.FloorToInt((float) this._Icons.Length / col);
                for (int i = 0; i < row; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    for (int j = 0; j < col; j++)
                    {
                        int index = i * col + j;
                        if (index >= this._Icons.Length)
                        {
                            return;
                        }
                        Texture cur = this._Icons[index] as Texture;
                        if (cur == null)
                        {
                            continue;
                        }
                        if (GUILayout.Button(cur, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth)))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("拷贝"), false, () =>
                            {
                                UnityEngine.GUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(cur.GetInstanceID());
                            });
                            menu.ShowAsContext();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            });
            this.GraphView.Add(textureItem);
        }
    }
}