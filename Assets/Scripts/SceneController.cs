using UnityEngine;
using UnityEngine.SceneManagement; // Very important for scene loading

public class SceneController : MonoBehaviour
{
    // Public function to call from Button
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
