using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace TMPro.Examples
{
    public class VertexZoom : MonoBehaviour
    {
        public float AngleMultiplier = 1.0f;
        public float SpeedMultiplier = 1.0f;
        public float CurveScale = 1.0f;

        private TMP_Text m_TextComponent;
        private bool hasTextChanged;

        void Awake()
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }

        void OnDisable()
        {
            // Unsubscribe from event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        }

        void Start()
        {
            StartCoroutine(AnimateVertexColors());
        }

        void ON_TEXT_CHANGED(Object obj)
        {
            // Only trigger the animation if the text component has changed
            if (obj == m_TextComponent)
                hasTextChanged = true;
        }

        /// <summary>
        /// Method to animate vertex colors of a TMP Text object.
        /// </summary>
        /// <returns></returns>
        IEnumerator AnimateVertexColors()
        {
            // Force an update of the text object
            m_TextComponent.ForceMeshUpdate();

            TMP_TextInfo textInfo = m_TextComponent.textInfo;
            TMP_MeshInfo[] cachedMeshInfoVertexData = textInfo.CopyMeshInfoVertexData();

            // Temporary lists for sorting character scales
            List<float> modifiedCharScale = new List<float>();
            List<int> scaleSortingOrder = new List<int>();

            while (true)
            {
                if (hasTextChanged)
                {
                    // Get updated vertex data
                    cachedMeshInfoVertexData = textInfo.CopyMeshInfoVertexData();
                    hasTextChanged = false;
                }

                int characterCount = textInfo.characterCount;

                // If there are no characters, just wait for new text
                if (characterCount == 0)
                {
                    yield return new WaitForSeconds(0.25f);
                    continue;
                }

                modifiedCharScale.Clear();
                scaleSortingOrder.Clear();

                // Loop through all characters in the text
                for (int i = 0; i < characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                    // Skip characters that are not visible and thus have no geometry
                    if (!charInfo.isVisible)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] sourceVertices = cachedMeshInfoVertexData[materialIndex].vertices;

                    // Calculate the center of the character
                    Vector2 charMidBaseline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
                    Vector3 offset = charMidBaseline;

                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                    // Apply the offset to all 4 vertices of the character's quad
                    destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                    destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                    destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                    destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                    // Random scale for the character
                    float randomScale = Random.Range(1f, 1.5f);
                    modifiedCharScale.Add(randomScale);
                    scaleSortingOrder.Add(modifiedCharScale.Count - 1);

                    // Setup matrix to scale the vertices
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * randomScale);

                    // Apply the matrix to each vertex
                    for (int j = 0; j < 4; j++)
                    {
                        destinationVertices[vertexIndex + j] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + j]) + offset;
                    }
                }

                // Sort vertices based on modified scales (to control animation order)
                scaleSortingOrder.Sort((a, b) => modifiedCharScale[a].CompareTo(modifiedCharScale[b]));

                // Push the updated vertex data to the meshes
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    // Sort geometry based on the modified scales
                    textInfo.meshInfo[i].SortGeometry(scaleSortingOrder);

                    // Update the mesh
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;

                    m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
