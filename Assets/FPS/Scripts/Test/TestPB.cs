using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPS.Scripts.Test
{
    public class TestPB : MonoBehaviour
    {
        public                  Renderer              renderer;
        public                  Color                 Color = Color.red;
        public                  MaterialPropertyBlock pb;
        private static readonly int                   BaseColor = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            renderer = GetComponentInChildren<Renderer>();
            pb = new MaterialPropertyBlock();
        }

        [Button]
        public void ChangeColor()
        {
            pb.SetColor(BaseColor, Color);
            renderer.SetPropertyBlock(pb);
            
        }
    }
}