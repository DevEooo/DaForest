using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StarterAssets
{
    public partial class StarterAssetsDeployMenu : ScriptableObject
    {
        public const string MenuRoot = "Tools/Starter Assets";

        private const string MainCameraPrefabName = "MainCamera";
        private const string PlayerCapsulePrefabName = "PlayerCapsule";
        private const string CinemachineVirtualCameraName = "PlayerFollowCamera";

        private const string PlayerTag = "Player";
        private const string MainCameraTag = "MainCamera";
        private const string CinemachineTargetTag = "CinemachineTarget";

        private static string StarterAssetsPath => PathToThisFile;
        private static GameObject _cinemachineVirtualCamera;

        public static string StarterAssetsInstallPath
        {
            get
            {
                string path = PathToThisFile;
                return path.Substring(0, path.LastIndexOf("StarterAssets"));
            }
        }

        private static string PathToThisFile
        {
            get
            {
                var dummy = CreateInstance<StarterAssetsDeployMenu>();
                string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy));
                DestroyImmediate(dummy);
                return path.Substring(0, path.LastIndexOf("/Editor/StarterAssetsDeployMenu.cs"));
            }
        }

        [MenuItem(MenuRoot + "/Reinstall Dependencies", false)]
        static void ResetPackageChecker()
        {
            Debug.Log("Reinstall Dependencies clicked (placeholder).");
        }

        [MenuItem(MenuRoot + "/Reset Player Position", false)]
        static void ResetPlayerPosition()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(PlayerTag);
            foreach (GameObject player in players)
            {
                Undo.RecordObject(player.transform, "Reset Player Position");
                player.transform.position = Vector3.zero;
            }
        }

        private static void CheckCameras(string prefabPath, Transform targetParent)
        {
            CheckMainCamera(prefabPath);

            GameObject vcam = GameObject.Find(CinemachineVirtualCameraName);
            if (!vcam)
            {
                HandleInstantiatingPrefab(StarterAssetsPath + prefabPath, CinemachineVirtualCameraName, out GameObject vcamPrefab);
                _cinemachineVirtualCamera = vcamPrefab;
            }
            else
            {
                _cinemachineVirtualCamera = vcam;
            }

            GameObject[] targets = GameObject.FindGameObjectsWithTag(CinemachineTargetTag);
            GameObject target = targets.FirstOrDefault(t => t.transform.IsChildOf(targetParent));
            if (target == null)
            {
                target = new GameObject("PlayerCameraRoot");
                target.transform.SetParent(targetParent);
                target.transform.localPosition = new Vector3(0f, 1.375f, 0f);
                target.tag = CinemachineTargetTag;
                Undo.RegisterCreatedObjectUndo(target, "Created new cinemachine target");
            }

            // Try setting the follow target only if Cinemachine exists
            SetVirtualCameraFollowReference(target, _cinemachineVirtualCamera);
        }

        private static void CheckMainCamera(string prefabPath)
        {
            GameObject[] mainCameras = GameObject.FindGameObjectsWithTag(MainCameraTag);

            if (mainCameras.Length < 1)
            {
                HandleInstantiatingPrefab(StarterAssetsPath + prefabPath, MainCameraPrefabName, out _);
            }
            else
            {
                // Try adding CinemachineBrain dynamically
                var brainType = System.Type.GetType("Cinemachine.CinemachineBrain, Cinemachine");
                if (brainType != null && !mainCameras[0].GetComponent(brainType))
                {
                    mainCameras[0].AddComponent(brainType);
                }
            }
        }

        private static void SetVirtualCameraFollowReference(GameObject target, GameObject cinemachineVirtualCamera)
        {
            var vcamType = System.Type.GetType("Cinemachine.CinemachineVirtualCamera, Cinemachine");
            if (vcamType == null) return;

            var vcam = cinemachineVirtualCamera?.GetComponent(vcamType);
            if (vcam == null) return;

            var so = new SerializedObject(vcam);
            var followProp = so.FindProperty("m_Follow");
            followProp.objectReferenceValue = target.transform;
            so.ApplyModifiedProperties();
        }

        private static void HandleInstantiatingPrefab(string path, string prefabName, out GameObject prefab)
        {
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<Object>($"{path}{prefabName}.prefab"));
            Undo.RegisterCreatedObjectUndo(prefab, "Instantiate Starter Asset Prefab");

            prefab.transform.localPosition = Vector3.zero;
            prefab.transform.localEulerAngles = Vector3.zero;
            prefab.transform.localScale = Vector3.one;
        }
    }
}
