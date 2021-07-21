using System;
using System.Collections;
using System.Reflection;
using ActionFrame.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    public static class VisualElementEx
    {
        /// <summary>
        /// 移除所有子节点
        /// </summary>
        /// <param name="self"></param>
        public static void RemoveAllChild(this VisualElement self)
        {
            if (self.childCount == 0)
            {
                return;
            }
            int count = self.childCount;
            for (int i = 0; i < count; i++)
            {
                self.RemoveAt(0);
            }
        }

        public static void RemoveAllExcludeZero(this VisualElement self)
        {
            if (self.childCount == 0) return;
            int count = self.childCount;
            for (int i = 0; i < count - 1; i++)
            {
                self.RemoveAt(1);
            }
        }

        public static void RemoveSelectedLength(this VisualElement self, int startIndex, int length)
        {
            if (self.childCount == 0) return;
            int count = self.childCount;
            for (int i = 0; i < length; i++)
            {
                self.RemoveAt(startIndex);
            }
        }

        public static void DrawObject(this VisualElement self, string labelName, string toolTip, object obj, Type objType, Action<object> callback = null)
        {
            if (obj == null)
            {
                obj = objType == typeof(string) ? string.Empty : Activator.CreateInstance(objType);
            }

            if (string.IsNullOrEmpty(labelName))
            {
                labelName = (string) null;
            }
            
            void DrawValueType<T>(BaseField<T> view)
            {
                view.RegisterValueChangedCallback(value =>
                {
                    obj = value.newValue;
                    callback?.Invoke(value.newValue);
                });
                self.Add(view);
            }
            switch (obj)
            {
                case float v:
                    FloatField floatField = new FloatField(labelName);
                    floatField.value = v;
                    floatField.tooltip = toolTip;
                    DrawValueType<float>(floatField);
                    break;
                case double v:
                    DoubleField doubleField = new DoubleField(labelName);
                    doubleField.value = v;
                    doubleField.tooltip = toolTip;
                    DrawValueType<double>(doubleField);
                    break;
                case int v:
                    IntegerField intField = new IntegerField(labelName);
                    intField.value = v;
                    intField.tooltip = toolTip;
                    DrawValueType<int>(intField);
                    break;
                case string v:
                    TextField strField = new TextField(labelName);
                    strField.value = v;
                    strField.tooltip = toolTip;
                    DrawValueType<string>(strField);
                    break;
                case bool v:
                    Toggle toggle = new Toggle(labelName);
                    toggle.value = v;
                    toggle.tooltip = toolTip;
                    DrawValueType<bool>(toggle);
                    break;
                case Vector2 v:
                    Vector2Field v2Field = new Vector2Field(labelName);
                    v2Field.value = v;
                    v2Field.tooltip = toolTip;
                    DrawValueType<Vector2>(v2Field);
                    break;
                case Vector3 v:
                    Vector3Field v3Field = new Vector3Field(labelName);
                    v3Field.value = v;
                    v3Field.tooltip = toolTip;
                    DrawValueType<Vector3>(v3Field);
                    break;
                case Vector2Int v:
                    Vector2IntField v2IntField = new Vector2IntField(labelName);
                    v2IntField.value = v;
                    v2IntField.tooltip = toolTip;
                    DrawValueType<Vector2Int>(v2IntField);
                    break;
                case Vector3Int v:
                    Vector3IntField v3IntField = new Vector3IntField(labelName);
                    v3IntField.value = v;
                    v3IntField.tooltip = toolTip;
                    DrawValueType<Vector3Int>(v3IntField);
                    break;
                case Enum v:
                    EnumField enumField = new EnumField(labelName, v);
                    enumField.tooltip = toolTip;
                    enumField.RegisterValueChangedCallback(e =>
                    {
                        obj = e.newValue;
                        callback?.Invoke(e.newValue);
                    });
                    self.Add(enumField);
                    break;
                case LayerMask v:
                    MaskField layerMaskField = new LayerMaskField(labelName, v);
                    layerMaskField.tooltip = toolTip;
                    layerMaskField.RegisterValueChangedCallback(e =>
                    {
                        obj = e.newValue;
                        callback?.Invoke(new LayerMask{value = e.newValue});
                    });
                    self.Add(layerMaskField);
                    break;
                case IList v:
                    Foldout foldout = new Foldout();
                    foldout.name = "ListFoldOut";
                    foldout.text = labelName;
                    foldout.value = false;
                    foldout.tooltip = toolTip;

                    Button foldOutBtn = new Button(() =>
                    {
                        Type itemType = objType.GenericTypeArguments[0];
                        object item = Activator.CreateInstance(itemType);
                        ((IList)obj).Add(item);
                        bool isOdd = (((IList) obj).Count - 1) % 2 == 1;
                        DrawItem(foldout, itemType, obj, item, ((IList) obj).Count - 1, isOdd);
                        callback?.Invoke(obj);
                    });
                    foldOutBtn.name = "ListFoldOutBtn";
                    foldOutBtn.text = "+";
                    foldout.hierarchy.Add(foldOutBtn);
                    for (int i = 0; i < v.Count; i++)
                    {
                        bool isOdd = i % 2 == 1;
                        Type type = ((IList)obj)[i].GetType();
                        DrawItem(foldout, type, ((IList)obj),((IList)obj)[i], i, isOdd);
                    }

                    self.Add(foldout);
                    void DrawItem(VisualElement root, Type type, object list, object item, int index, bool isOdd)
                    {
                        VisualElement container = new VisualElement();
                        container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                        container.name = isOdd? "OddNumberContainer" : "EvenNumberContainer";
                        var labelNameAttr = type.GetCustomAttribute<LabelNameAttribute>();
                        string name = labelNameAttr != null ? labelNameAttr.LabelName : labelName;
                        string tip = labelNameAttr != null ? labelNameAttr.ToolTip : String.Empty;
                        
                        VisualElement containerLeft = new VisualElement();
                        containerLeft.style.flexGrow = 1;
                        container.Add(containerLeft);
                        containerLeft.DrawObject(name, tip, ((IList)list)[index], type, o =>
                        {
                            ((IList)list)[index] = o;
                        });
                        
                        Button removeBtn = new Button(() =>
                        {
                            ((IList)list).Remove(item);
                            root.Remove(container);
                        });
                        removeBtn.text = "x";
                        removeBtn.name = "ListItemRemoveBtn";
                        container.Add(removeBtn);
                        root.Add(container);
                    };
                    
                    break;
                default:
                    {
                        if (!objType.IsPrimitive && (objType.IsClass || objType.IsValueType))
                        {
                            Foldout foldoutClass = new Foldout();
                            foldoutClass.text = string.IsNullOrEmpty(labelName)? obj.ToString() : labelName;
                            foldoutClass.tooltip = string.IsNullOrEmpty(toolTip)? String.Empty : toolTip;
                            FieldInfo[] fields = objType.GetFields();
                            foreach (var field in fields)
                            {
                                if (field.GetCustomAttribute<HideInViewAttribute>() != null)
                                {
                                    continue;
                                }
                                obj = foldoutClass.DrawField(obj, field, callback);
                            }
                            self.Add(foldoutClass);
                        }
                    }
                    break;
            }
        }
        
        private static object DrawField(this VisualElement self, object target, FieldInfo fieldInfo, Action<object> callback = null)
        {
            object fieldValue = fieldInfo.GetValue(target);
            
            var labelNameAttr = fieldInfo.GetCustomAttribute<LabelNameAttribute>();
            string labelName = labelNameAttr != null? labelNameAttr.LabelName : fieldInfo.Name;
            string toolTip = labelNameAttr != null? labelNameAttr.ToolTip : String.Empty;
            
            self.DrawObject(labelName, toolTip, fieldValue, fieldInfo.FieldType, o =>
            {
                fieldInfo.SetValue(target, o);
                callback?.Invoke(target);
            });

            return target;
        }
    }
}