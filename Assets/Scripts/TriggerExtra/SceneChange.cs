using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    public enum SceneList
    {
        Floor1,
        Floor2,
        Floor3,
        Floor4,
        Floor5,
        EditScene
    }

    [SerializeField] private SceneList targetScene;

    public void OpenSelectedScene()
    {
        string sceneName = targetScene switch
        {
            SceneList.Floor1 => "Floor1",
            SceneList.Floor2 => "Floor2",
            SceneList.Floor3 => "Floor3",
            SceneList.Floor4 => "Floor4",
            SceneList.Floor5 => "Floor5",
            SceneList.EditScene => "Editor",
            _ => "Floor1"
        };

        SceneManager.LoadScene(sceneName);
    }

    
}