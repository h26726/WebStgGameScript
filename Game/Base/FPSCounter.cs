using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsText;
    private float deltaTime;

    private readonly StringBuilder sb = new StringBuilder(16);

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        sb.Clear();
        sb.Append((int)fps); // fps:0. 表示取整數
        sb.Append(" FPS");

        fpsText.text = sb.ToString();
    }
}
