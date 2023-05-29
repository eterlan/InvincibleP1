//
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using UnityEditor;
// using UnityEditor.Animations;
// using UnityEngine;
// using UnityEngine.Rendering;
//
// public class VFXToolWindow : EditorWindow
// {
//     [MenuItem("Tools/Cy/打开特效工具窗口 &V")]
//     public static void CreateVFXToolWindow()
//     { 
//         GetWindow<VFXToolWindow>();
//     }
//
//     #region InternalFields
//
//     private GUIContent       m_sortingLayerName;
//     private SerializedObject m_soWindow;
//     private Material         m_distortionMaterial;
//
//     #endregion
//     
//     #region InputFields
//
//     private bool         m_foldoutHeader;
//     private bool         m_toggleChangeSorting, m_toggleChangeSortingFudge, m_toggleChangeLayer = true, m_toggleDeleteMissingReference = true, m_toggleResetPosition = true, m_toggleFixDistortion = true, m_toggleMoveToTestFolder = true;
//
//     private float m_sortingFudge;
//     [SerializeField]
//     private int m_sortingLayerID, m_sortingLayerOrder;
//     [SerializeField]
//     private int m_layer;
//     [SerializeField]
//     private string m_testFolderName;
//
//     #endregion
//
//     private void OnEnable()
//     {
//         m_sortingLayerName   = new GUIContent("排序层");
//         m_soWindow           = new SerializedObject(this);
//         m_sortingLayerID     = SortingLayer.NameToID("Default");
//         m_sortingLayerOrder  = 0;
//         m_sortingFudge       = 0;
//         m_layer              = LayerMask.NameToLayer("Scene");
//         m_distortionMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Bundles/effect/res/material/niuqu/kuosan_niuqu_2.mat");
//         m_testFolderName     = "GUIGU";
//     }
//
//     private void OnGUI()
//     {
//         Header();
//         
//         Options();
//         Fields();
//         Buttons();
//     }
//
//     private void Buttons()
//     {
//         EditorGUILayout.Separator();
//
//         var fixSelectVFX       = GUILayout.Button("处理选中特效");
//         var fixSelectFolderVFX = GUILayout.Button("处理文件夹中所有特效");
//         
//         
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("",GUI.skin.horizontalSlider);
//         EditorGUILayout.Space();
//         
//         
//         m_toggleMoveToTestFolder = EditorGUILayout.ToggleLeft("移动到测试文件夹", m_toggleMoveToTestFolder);
//         if (m_toggleMoveToTestFolder) 
//             m_testFolderName = EditorGUILayout.TextField("测试目录名", m_testFolderName);
//         if (GUILayout.Button("资源目录归类")) ClassifyVFXResource();
//
//
//         if (fixSelectVFX || fixSelectFolderVFX)
//         {
//             var gos    = (fixSelectVFX ? GetAllSelectedVFX() : GetAllVFXInFolder()).ToList();
//             var psList = gos.Select(go => go.TryGetComponent(out ParticleSystem _) ? go : null).ToList();
//             
//             DeleteSelectedVFXMissingScripts(gos);
//             foreach (var ps in psList)
//             {
//                 ResetPosition(ps.transform);
//         
//                 var childrenPs = ps.GetComponentsInChildren<ParticleSystem>(true);
//                 for (var j = 0; j < childrenPs.Length; j++)
//                 {
//                     ChangeSelectedVFXSortingLayer(childrenPs[j]);
//                     ChangeSelectedVFXLayer(childrenPs[j]); 
//                     FixDistortionLayer(childrenPs[j]);
//                 }
//             }
//             
//             DeleteSelectedVFXMissingScripts(psList);
//             
//             for (var i = 0; i < psList.Count; i++)
//             {
//                 var go = psList[i];
//                 EditorUtility.SetDirty(go);
//                 if (go.scene.name != null) 
//                     continue;
//                 PrefabUtility.RecordPrefabInstancePropertyModifications(go);
//                 PrefabUtility.SavePrefabAsset(go.transform.root.gameObject);
//             }
//         }
//
//         EditorGUILayout.Separator();
//     }
//
//     private void Header()
//     {
//         m_foldoutHeader = EditorGUILayout.BeginFoldoutHeaderGroup(m_foldoutHeader, "批量处理选中特效");
//         if (m_foldoutHeader)
//         {
//             EditorGUI.indentLevel++;
//             EditorGUILayout.Separator();
//             EditorGUILayout.LabelField("说明", "按ctrl或者shift可以多选预制体, 所有操作都会处理所有子物体");
//             EditorGUILayout.Separator();
//             EditorGUI.indentLevel--;
//         }
//         EditorGUILayout.EndFoldoutHeaderGroup();
//     }
//
//     private void Options()
//     {
//         EditorGUILayout.Separator();
//         m_toggleChangeSorting          = EditorGUILayout.ToggleLeft("修改排序层 SortingLayer", m_toggleChangeSorting);
//         if (m_toggleChangeSorting) 
//             m_toggleChangeSortingFudge = EditorGUILayout.ToggleLeft("修改距离偏移 Sorting Fudge", m_toggleChangeSortingFudge);
//         m_toggleChangeLayer            = EditorGUILayout.ToggleLeft("修改场景层 Layer", m_toggleChangeLayer);
//         m_toggleDeleteMissingReference = EditorGUILayout.ToggleLeft("删除空脚本", m_toggleDeleteMissingReference);
//         m_toggleResetPosition          = EditorGUILayout.ToggleLeft("重置位置", m_toggleResetPosition);
//         m_toggleFixDistortion          = EditorGUILayout.ToggleLeft("修复扭曲层级", m_toggleFixDistortion);
//         EditorGUILayout.Separator();
//     }
//
//     private void Fields()
//     {
//         if (m_toggleChangeSorting)
//         {
//             EditorGUILayout.Separator();
//             SortingLayerEditorUtility.SortingLayerFieldLayout(m_sortingLayerName, m_soWindow.FindProperty(nameof(m_sortingLayerID)));
//             m_sortingLayerOrder = EditorGUILayout.IntField("排序层顺序", m_sortingLayerOrder);
//         }
//
//         if (m_toggleChangeSortingFudge)
//         {
//             m_sortingFudge = EditorGUILayout.FloatField("距离偏移 Soring Fudge", m_sortingFudge);
//         }
//         
//         EditorGUILayout.Separator();    
//
//         if (m_toggleChangeLayer)
//         {
//             m_layer = EditorGUILayout.LayerField("场景层", m_layer);
//             EditorGUILayout.Separator();
//         }
//
//         if (m_toggleFixDistortion)
//         {
//             m_distortionMaterial = (Material)EditorGUILayout.ObjectField("扭曲材质", m_distortionMaterial, typeof(Material), false);
//         }
//     }
//
//     private void ClassifyVFXResource()
//     {
//         var folderPath = GOAndAssetsUtil.GetSelectedFolderPath(false);
//         var guids = AssetDatabase.FindAssets("", new []{folderPath});
//         foreach (var guid in guids)
//         {
//             var assetPath = AssetDatabase.GUIDToAssetPath(guid);
//             // var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
//             var assetExtensionIndex = assetPath.LastIndexOf('.');
//             // 文件夹
//             if (assetExtensionIndex <= 0) 
//                 continue;
//             var assetExtension = assetPath.Substring(assetExtensionIndex + 1).ToLower();
//
//             var assetNameIndex = assetPath.LastIndexOf('/');
//             var assetName      = assetPath.Substring(assetNameIndex + 1);
//             
//             var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
//             // 不用asset类型是因为fbx格式显示为GameObject
//             var newPath = assetType.Name switch
//             {
//                 nameof(GameObject) 
//                     when assetExtension != "fbx" => Res.EFFECT_PREFAB_PATH,
//                 nameof(GameObject)                                  => Res.EFFECT_MODEL_PATH,
//                 nameof(Material)                                    => Res.EFFECT_MAT_PATH,
//                 nameof(Shader) or "SubGraphAsset"       => Res.EFFECT_SHADER_PATH,
//                 nameof(AnimationClip) or nameof(AnimatorController) => Res.EFFECT_ANIMATION_PATH,
//                 nameof(Texture2D)                                   => Res.EFFECT_TEXTURE_PATH,
//                 _                                                   => string.Empty
//             };
//             if (newPath.IsNullOrEmpty())
//             {
//                 // 看看有什么遗漏情况    
//                 Debug.Log($"{assetExtension} + {assetType.Name} => {newPath}");
//             }
//             
//             if (m_toggleMoveToTestFolder)
//             {
//                 if (!AssetDatabase.IsValidFolder($"{newPath}{m_testFolderName}"))
//                 {
//                     AssetDatabase.CreateFolder(newPath.TrimEnd('/'), m_testFolderName);
//                 }
//                 newPath += $"{m_testFolderName}/";
//             }
//             
//             newPath += assetName;
//             
//             if (!newPath.IsNullOrEmpty()) AssetDatabase.MoveAsset(assetPath, newPath);
//         }
//
//         //var filePaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
//
//         // if (filePaths.Length == 0)
//         // {
//         //     Debug.LogWarning("选中目录下没有文件");
//         //     return;
//         // }
//         //
//         // var subStringIndex = filePaths[0].LastIndexOf("Assets", StringComparison.Ordinal);
//         //
//         // foreach (var filePath in filePaths)
//         // {
//         //     var assetRelativePath = filePath.Substring(subStringIndex);
//         //     var assetType         = AssetDatabase.GetMainAssetTypeAtPath(assetRelativePath);
//         //
//         //     var newPath = assetType.Name switch
//         //     {
//         //         nameof(Material) => "",
//         //         nameof(Texture)  => "",
//         //         nameof(Shader)   => "",
//         //         _                => ""
//         //     };
//         //     // AssetDatabase.MoveAsset(assetRelativePath, newPath);
//         // }
//
//         // 根据文件种类把资源放进相应的文件夹
//     }
//
//     #region 选中处理对象
//
//     private static IEnumerable<GameObject> GetAllSelectedVFX()
//     {
//         var selectedGOs = Selection.gameObjects;
//         if (selectedGOs.Length == 0)
//             Debug.LogWarning("没有选中物体");        
//
//         // Debug.LogWarning("选中物体不包含特效");
//
//         return selectedGOs;
//     }
//
//     private static IEnumerable<GameObject> GetAllVFXInFolder()
//     {
//         var selectedFolder = GOAndAssetsUtil.GetSelectedFolderPath(false);
//         var gos = AssetDatabase.FindAssets("t:prefab", new[] { selectedFolder }).
//             Select(guid =>
//             {
//                 var path = AssetDatabase.GUIDToAssetPath(guid);
//                 var go   = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//                 return go;
//             });
//         return gos;
//     }
//
//     #endregion
//
//     #region 处理特效
//
//     private void ChangeSelectedVFXSortingLayer(ParticleSystem childrenPs)
//     {
//         if (!m_toggleChangeSorting)
//             return;
//         var psRenderer = childrenPs.GetComponent<ParticleSystemRenderer>();
//         psRenderer.sortingFudge = 
//         psRenderer.sortingLayerID = m_sortingLayerID;
//         psRenderer.sortingOrder   = m_sortingLayerOrder;
//     }
//
//     private void FixDistortionLayer(ParticleSystem childrenPs)
//     {
//         if (!m_toggleFixDistortion)
//             return;
//         var renderer = childrenPs.GetComponent<ParticleSystemRenderer>();
//         if (renderer.sharedMaterial == null)
//             return;
//
//         var shaderName         = renderer.sharedMaterial.shader.name;
//         var isDistortionEffect = IsDistortionEffect(renderer.gameObject.name);
//         var materialOutdated   = !shaderName.Equals(m_distortionMaterial.shader.name, StringComparison.OrdinalIgnoreCase);
//         if (isDistortionEffect && materialOutdated)
//         {
//             renderer.sharedMaterial = m_distortionMaterial; 
//         }
//         
//         if (shaderName.Contains("distortion", StringComparison.OrdinalIgnoreCase))
//         {
//             childrenPs.gameObject.layer = LayerMask.NameToLayer("DistortionEffect");
//         }
//
//         bool IsDistortionEffect(string effectName) => 
//             effectName.Contains("distortion", StringComparison.OrdinalIgnoreCase) || effectName.Contains("niuqu", StringComparison.OrdinalIgnoreCase);
//     }
//
//     private void ChangeSelectedVFXLayer(ParticleSystem ps)
//     {
//         if (!m_toggleChangeLayer)
//         {
//             return;
//         }
//
//         ps.gameObject.layer = m_layer;
//     }
//
//     private void DeleteSelectedVFXMissingScripts(IEnumerable<GameObject> gos)
//     {
//         if (!m_toggleDeleteMissingReference)
//             return;
//         
//         FindMissingScriptsRecursively.FindAndRemoveMissingScripts(gos);
//     }
//
//     private void ResetPosition(Transform psTransform)
//     {
//         if (!m_toggleResetPosition)
//             return;
//
//         psTransform.localPosition = Vector3.zero;
//     }
//
//     #endregion
// }
