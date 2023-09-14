using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class CreateTerrain : EditorWindow
    {
        private static Object _blockPrefab;
        private static CreateTerrain _window;
        private static GameObject _targetParent;
        private static Vector3 _terrainSize;

        [MenuItem("Tools/Create Terrain")]
        public static void ShowWindow()
        {
            if (_window == null)
            {
                _window = GetWindow(typeof(CreateTerrain)) as CreateTerrain;
            }

            _window.titleContent = new GUIContent("Create Terrain Tool");
            _window.Show();
        }

        void OnGUI()
        {
            _blockPrefab = EditorGUILayout.ObjectField("Block Prefab:", _blockPrefab, typeof(Object), false);
            _targetParent = (GameObject)EditorGUILayout.ObjectField("Parent:", _targetParent, typeof(GameObject), true);
            _terrainSize = EditorGUILayout.Vector3Field("size:", _terrainSize);
            
            if (GUILayout.Button("Create Terrain"))
            {
                SpawnChunks();
            }
        }
        
        void SpawnChunks()
        {
            for (var zi = 0; zi < _terrainSize.z; zi++)
            {
                for (var xi = 0; xi < _terrainSize.x; xi++)
                {
                    for (var yi = 0; yi < _terrainSize.y; yi++)
                    {
                        var block = (GameObject)Instantiate(_blockPrefab, _targetParent.transform, false);
                        block.transform.localPosition = new Vector3(xi, -zi, yi);
                    }
                }
            }
        }
    }
}

