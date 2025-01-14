using UnityEngine;
using TMPro;  // Ensure to include TMPro namespace


namespace TMPro.Examples
{
    public class SimpleScript : MonoBehaviour
    {
        private TextMeshPro m_textMeshPro;
        private const string label = "The <#0050FF>count is: </color>{0:2}";
        private float m_frame;

        void Start()
        {
            // Add new TextMesh Pro Component
            m_textMeshPro = gameObject.AddComponent<TextMeshPro>();

            m_textMeshPro.autoSizeTextContainer = true;

            // Set various font settings.
            m_textMeshPro.fontSize = 48;

            m_textMeshPro.alignment = TextAlignmentOptions.Center;

            // Enable or disable word wrapping based on your requirement
            m_textMeshPro.enableWordWrapping = false;  // Set this to true if you want word wrapping

            // Optional: Set the color or material if needed
            // m_textMeshPro.fontColor = new Color32(255, 255, 255, 255);
        }

        void Update()
        {
            m_textMeshPro.SetText(label, m_frame % 1000);
            m_frame += 1 * Time.deltaTime;
        }
    }
}
