using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Update()
    {
        // Média do deltaTime para suavizar a leitura
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        GUI.Label(new Rect(10, 10, 100, 25), "FPS: " + Mathf.Ceil(fps));
    }
}