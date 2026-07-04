#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace StudioJAW
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class BoneRemapper : MonoBehaviour
    {
        public Transform[] bones = null; //we build the bones from the mesh.
    }

    [CustomEditor(typeof(BoneRemapper))]
    public class BoneRemapperEditor : Editor
    {
        private BoneRemapper br; //
        private SkinnedMeshRenderer smr;//the housed smr
        private SerializedProperty bonesProp;
        private bool autoCopyMissing = false;

        public enum Mode
        {
            None,
            AutoMap,
            SpawnBones
        }
        private Mode mode = Mode.None;

        private void InitCheck()
        {
            if (br == null)
            {
                br = (BoneRemapper)target;
            }
            if (smr == null)
            {
                smr = br.GetComponent<SkinnedMeshRenderer>();
            }
            if (bonesProp == null)
            {
                bonesProp = serializedObject.FindProperty("bones");
            }
            if (br.bones == null)
            {
                if (smr.bones != null || br.bones.Length != smr.bones.Length)
                {
                    br.bones = smr.bones;
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            InitCheck();
            DrawDefaultInspector();
            //EditorGUILayout.PropertyField(bonesProp, includeChildren: true);
            serializedObject.Update();
            if (smr.rootBone==null)
            {
                RenderError("No root bone specified on the Skinned Mesh Renderer.");
                return;
            }
            if (smr.sharedMesh==null)
            {
                RenderError("No mesh defined on the Skinned Mesh Renderer.");
                return;
            }
            RenderOptions();
        }

        public void RenderError(string error)
        {
            EditorGUILayout.HelpBox(error, MessageType.Error);
        }

        public void RenderOptions()
        {
            //use the root bone from smr to remap.
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("Reset Bones"))
            {
                Debug.Log(smr.bones);
                br.bones = smr.bones;
            }
            EditorGUILayout.HelpBox("Use this to reset the bones back to the last applied stated", MessageType.Info);            
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("Spawn Prefab Bones and Map"))
            {
                Run(Mode.SpawnBones);
            }
            EditorGUILayout.HelpBox("This tool spawns bones from the import prefab and sets the bone mapping to them. This is only meant to be used as reference for reassignment, and the bones should be deleted after they are remapped.",MessageType.Info);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("Auto-Map Bones"))
            {
                Run(Mode.AutoMap);
            }            
            autoCopyMissing = EditorGUILayout.Toggle("Copy Missing Bones If Exist.", autoCopyMissing);
            EditorGUILayout.HelpBox("This tool rebuilds the internal bones datastructure. It does this by using the skinned mesh renderers root bone and assigned mesh. This will load the import data from the fbx prefab to retrieve the vertex group names, and then remap those to the bone system defined by your root bone.",MessageType.Info);
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Apply Bones"))
            {
                smr.bones = br.bones;
                DestroyImmediate(target);
            }
        }

        private void SpawnBones(SkinnedMeshRenderer prefabSMR)
        {
            prefabSMR.rootBone.parent = br.transform;
            br.bones = prefabSMR.bones;
        }

        //the most important process of the script.
        private void AutoMapBones(SkinnedMeshRenderer prefabSMR)
        {
            if (prefabSMR.bones==null || prefabSMR.bones.Length == 0)
            {
                Debug.LogError("[BoneRemapper] No bones found in original prefab!");
                return;
            }
            Transform[] remapBonePool = smr.rootBone.GetComponentsInChildren<Transform>();
            Transform[] newBones = new Transform[prefabSMR.bones.Length];
            int found = 0;
            int duplicates = 0;
            int copied = 0;
            for (int b = 0; b < prefabSMR.bones.Length; b++)//b also aligns to newBones
            {
                bool isFound = false;
                for (int a = 0; a < remapBonePool.Length; a++)
                {
                    if (remapBonePool[a].name.CompareTo(prefabSMR.bones[b].name) == 0)
                    {
                        if (!isFound)
                        {
                            newBones[b] = remapBonePool[a];
                            isFound = true;
                        }
                        else
                        {
                            duplicates++;
                            Debug.LogWarning($"Duplicate bone found: {prefabSMR.bones[b].name}.");
                        }
                    }
                }
                if (isFound)
                {
                    found++;
                }
                else if(autoCopyMissing)
                {
                    //reverse to find the parent.
                    for (int p = 0; p < remapBonePool.Length; p++)
                    {
                        if (remapBonePool[p].name.CompareTo(prefabSMR.bones[b].parent.name)==0)
                        {
                            Transform newBone = Instantiate(prefabSMR.bones[b], prefabSMR.bones[b].position, prefabSMR.bones[b].rotation);
                            newBone.parent = remapBonePool[p];
                            newBone.localPosition = prefabSMR.bones[b].localPosition;
                            newBone.localScale = prefabSMR.bones[b].localScale;
                            newBone.localRotation = prefabSMR.bones[b].localRotation;
                            newBone.name = prefabSMR.bones[b].name;
                            newBones[b] = newBone;
                            copied++;
                            found++;
                            
                            //bone pool needs to be update with new bones.
                            remapBonePool = smr.rootBone.GetComponentsInChildren<Transform>();
                            break;
                        }
                    }
                }
            }
            
            string msg;
            msg = $"Found: {found}/{newBones.Length}\n";
            msg += $"Duplicates: {duplicates}\n";
            if (copied > 0)
            {
                msg += $"Copied Bones: {copied}\n";
            }
            if (found < newBones.Length || duplicates > 0){
                msg += "Possible Issues:\n";
            }
            if (found < newBones.Length)
            {
                msg += "- Not all bones were found, you can try creating the transforms manually and try again.\n";
            }
            if (duplicates > 0)
            {
                msg += "- There are duplicate bone names, try deleting ones that aren't used, or renaming them. Otherwise the first found in the hierarchy will be used.\n";
            }
            msg += "Would you like to set this mapping?";

            if (EditorUtility.DisplayDialog("Mapping Results",msg, "Remap", "Cancel"))
            {
                br.bones = newBones;
                Debug.Log("Bone mapping assigned and remapping component removed.");
            }
            else
            {
                Debug.Log("Canceled reassignment");
            }
        }

        //Daisy chain process for cleaning up.
        //I chose to push the stack in this way, for the convenience of keeping the editor's hierarchy and memory clean.
        private void GetSMR(GameObject prefabGO)
        {
            SkinnedMeshRenderer prefabSMR = null;
            SkinnedMeshRenderer[] smrs = prefabGO.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smrs == null)
            {
                Debug.LogError("[BoneRemapper] Could not find any Skinned Mesh Renderers in the import prefab!");
            }

            for (int i = 0; i < smrs.Length; i++)
            {
                if(smr.sharedMesh == smrs[i].sharedMesh)
                {
                    prefabSMR = smrs[i];
                };
            }
            
            if (prefabSMR!=null)
            {
                switch(mode)
                {
                    case Mode.SpawnBones:
                        SpawnBones(prefabSMR);
                        break;
                    case Mode.AutoMap:
                        AutoMapBones(prefabSMR);
                        break;
                }                
            }
            else
            {
                Debug.LogError("[BoneRemapper] Could not find original Skinned Mesh Renderer! Was the model imported correctly?");
            }
        }

        private void InstantiatePrefabGO(GameObject prefabObj)
        {
            GameObject prefabGO = Instantiate(prefabObj);
            if (prefabGO != null)
            {
                GetSMR(prefabGO);
            }
            else
            {
                Debug.LogError("[BoneRemapper] Could not create instance of prefab! Is this import corrupted?");
            }        
            DestroyImmediate(prefabGO); 
            prefabGO = null;
        }

        private void LoadPrefab(string path)
        {
            GameObject prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefabObj != null)
            {
                InstantiatePrefabGO(prefabObj);                
            }
            else
            {
                Debug.LogError("[BoneRemapper] Could not load imported object! Is this import corrupted?");
            };
        }

        private void Run(Mode inMode)
        {
            mode = inMode;
            string path = AssetDatabase.GetAssetPath(smr.sharedMesh);
            if (!string.IsNullOrEmpty(path))
            {
                LoadPrefab(path);
            }
            else
            {
                Debug.LogError("[BoneRemapper] Import for mesh missing! Is this mesh from an imported object?");
            };
            GC.Collect();
            mode = Mode.None;
        }
    }
}
#endif