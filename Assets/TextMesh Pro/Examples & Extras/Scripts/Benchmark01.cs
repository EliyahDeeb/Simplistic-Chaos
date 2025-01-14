using UnityEngine;
using TMPro;
using System.Collections;

namespace TMPro.Examples
{
    public class Benchmark01 : MonoBehaviour
    {
        public int BenchmarkType = 0;
        public TMP_FontAsset TMProFont;
        public Font TextMeshFont;

        private TMP_Text m_textMeshPro;
        private TextMesh m_textMesh;

        private const string label01 = "The <#0050FF>count is: </color>{0}";
        private const string label02 = "The <color=#0050FF>count is: </color>";

        private Material m_material01;
        private Material m_material02;

        IEnumerator Start()
        {
            if (BenchmarkType == 0) // TextMesh Pro Component
            {
                m_textMeshPro = gameObject.AddComponent<TextMeshPro>();
                m_textMeshPro.enableAutoSizing = true;
                m_textMeshPro.fontSizeMin = 10;
                m_textMeshPro.fontSizeMax = 48;

                if (TMProFont != null)
                    m_textMeshPro.font = TMProFont;
                else
                    m_textMeshPro.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

                m_textMeshPro.alignment = TextAlignmentOptions.Center;
                m_textMeshPro.extraPadding = true;
                m_textMeshPro.enableWordWrapping = false;

                m_material01 = m_textMeshPro.font.material;
                m_material02 = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Drop Shadow");
            }
            else if (BenchmarkType == 1) // TextMesh
            {
                m_textMesh = gameObject.AddComponent<TextMesh>();

                if (TextMeshFont != null)
                {
                    m_textMesh.font = TextMeshFont;
                    m_textMesh.GetComponent<Renderer>().sharedMaterial = m_textMesh.font.material;
                }
                else
                {
                    m_textMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    m_textMesh.GetComponent<Renderer>().sharedMaterial = m_textMesh.font.material;
                }

                m_textMesh.fontSize = 48;
                m_textMesh.anchor = TextAnchor.MiddleCenter;
            }

            for (int i = 0; i <= 1000000; i++)
            {
                if (BenchmarkType == 0)
                {
                    m_textMeshPro.SetText(label01, i % 1000);
                    if (i % 1000 == 999)
                        m_textMeshPro.fontSharedMaterial = m_textMeshPro.fontSharedMaterial == m_material01 ? m_material02 : m_material01;
                }
                else if (BenchmarkType == 1)
                {
                    m_textMesh.text = label02 + (i % 1000).ToString();
                }

                yield return null;
            }
        }
    }
}
