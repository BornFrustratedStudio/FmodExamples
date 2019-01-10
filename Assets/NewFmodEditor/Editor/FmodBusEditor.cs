﻿using UnityEditor;

namespace FmodEditor
{
    [CustomEditor(typeof(FmodBus))]
    public class FmodBusEditor : Editor
    {
        private static FmodBus m_fmodBus;

        private void OnEnable()
        {
            m_fmodBus = (FmodBus)target;
            BuildingBus();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Init();
        }

        private void Init()
        {
            foreach (BusData data in m_fmodBus.busData)
            {
                EditorGUILayout.TextArea(data.BusName);
                ShowBusInfo(data);
            }
        }

        private void ShowBusInfo(BusData data)
        {
            data.Muted = EditorGUILayout.Toggle("Muted", data.Muted);
            data.BusVolume = EditorGUILayout.Slider("Volume:", data.BusVolume, 0, 1);
            EditorUtility.SetDirty(m_fmodBus);
        }

        [MenuItem("FmodEditor/BuildBus")]
        public static void BuildingBus()
        {
            m_fmodBus.Init();
        }

    }
}