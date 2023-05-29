using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lib.Editor
{
    public class RiggingTool
    {
        /// <summary>
        /// 测试专用
        /// </summary>
        [MenuItem("Tools/Cy/绑骨/测试专用")]
        public static void Testing()
        {
            // 确实会选中UI选中的物体..
            //Debug.Log(EventSystem.current.currentSelectedGameObject);
            // var activeSkills = new List<int>(){1000};
            // var passiveSkills = new List<int>(){3,5};
            //g.ui.OpenUI<UIGetImmortalSkill>(UIType.GetImmortalSkill).OnOpenUI(activeSkills, passiveSkills);

            //g.ui.OpenUI<UIGetImmortalPointTipBase>(UIType.GetImmortalPointTip);
            var oldPath    = "G:\\Immortal_Test\\immortal_plan\\配置";
            var outputPath = $"{oldPath}/../配置合并(自动生成)";
            Directory.CreateDirectory(outputPath);
        }

        /// <summary>
        /// 根据第二个选中的物体下的所有子物体相对位置, 来设置第一个选中的物体下的所有子物体相对位置. 主要能用于不小心修改了Prefab后试图还原位置.
        /// 要求: 1. 需要所有子物体名字都对应的上. 2. 所有子物体不能同名(骨骼要先使用SpriteSkin生成之后, 再用GameTool->Other->绑骨->根据root下GameObject名字重命名骨骼)
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="Exception"></exception>
        [MenuItem("Tools/Cy/绑骨/复制指定目标的Transform(不能有同名子物体)")]
        private static void ResetTransform()
        {
            if (Selection.count != 2 || Selection.gameObjects.Any(go => go.name != "root"))
            {
                throw new IndexOutOfRangeException("请先选择要修改的物体的root, 再选择参照目标的root");
            }

            var refObj               = Selection.activeGameObject;
            var modifyObjIndex       = refObj == Selection.gameObjects[0] ? 1 : 0;
            var modifyObj            = Selection.gameObjects[modifyObjIndex];
            var refObjChildren       = refObj.GetComponentsInChildren<Transform>();
            var modifyObjChildren    = modifyObj.GetComponentsInChildren<Transform>();
            var modObjChildrenSorted = new List<Transform>(refObjChildren.Length);
            ValidateChildrenName();
            CopyTransform();

            void ValidateChildrenName()
            {
                for (var i = 0; i < refObjChildren.Length; i++)
                {
                    var refChild                = refObjChildren[i];
                    var sameNameCurrentObjChild = modifyObjChildren.FirstOrDefault(tr => tr.name == refChild.name);
                    if (sameNameCurrentObjChild == null)
                        throw new Exception($"在待修改目标上无法找到同名物体, 请检查待修改目标身上是否有{refChild}");

                    modObjChildrenSorted.Add(sameNameCurrentObjChild);
                }
            }

            void CopyTransform()
            {
                for (var i = 0; i < refObjChildren.Length; i++)
                {
                    var refChild = refObjChildren[i];
                    if (refChild.name == "root")
                    {
                        continue;
                    }

                    var modChild       = modObjChildrenSorted[i];
                    var parentName     = modChild.parent.name;
                    var refChildParent = refObjChildren.First(tr => tr.name == parentName);
                    refChild.SetParent(refChildParent);
                    var modTr = modChild.transform;
                    modTr.localPosition = refChild.localPosition;
                    modTr.localRotation = refChild.localRotation;
                    modTr.localScale    = refChild.localScale;
                }
            }
        }
    

        /// <summary>
        /// 根据图片文件名重命名骨骼 (e.g. bone_1 => 文件名_1)
        /// </summary>
        [MenuItem("Tools/Cy/绑骨/根据图片文件名重命名骨骼")]
        private static void RenameBonesWithImageFileName()
        {
            var currentFolderPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs.FirstOrDefault());

            // 选中文件：处理文件同目录所有文件
            // 选中目录：默认处理

            string folderFullPath;
            if (Directory.Exists(currentFolderPath))
            {
                folderFullPath = Application.dataPath.Replace("Assets", string.Empty) + currentFolderPath;
                //folderFullPath = Application.dataPath.TrimEnd("Assets".ToCharArray()) + currentFolderPath;
            }
            else
            {
                folderFullPath = Directory.GetParent(currentFolderPath).FullName;
            }

            var pngMetaFilesPath = Directory.GetFiles(folderFullPath).ToList();
            //Debug.Log($"parent:{folderFullPath}>>\n\rfiles:\n\r{string.Join(",\n\r", pngMetaFilesPath)}");
            foreach (var s in pngMetaFilesPath.Where(filePath => filePath.Contains(".png.meta")))
            {
                var text = File.ReadAllText(s);
                //var fileName = Path.GetFileNameWithoutExtension(s).TrimEnd(".png".ToCharArray()) + "_";
                var fileName = Path.GetFileNameWithoutExtension(s).Replace(".png", string.Empty) + "_";
                Debug.Log(fileName);
                text = text.Replace("bone_", fileName);
                File.WriteAllText(s, text);
            }
        }
    }
}