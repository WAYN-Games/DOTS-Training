using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
//    ,IEntitySceneBuildAdditions
//          -> in 1.0.16 this interface does not exists and the build process will not detect the
//             List<EntitySceneReference> so we need to add all the levels with a sub-scene authoring
//             component in one of the game scene and add the scene to the build setting of the game
//          -> in 1.2.0 this interface allows you to self register sub scenes to the build 
{
    public static LevelManager Instance { get; private set; }

    public string GameScene = "LevelScene";
    public string MenuScene = "GameMenu";
 
    public SubScene Scene;
    public UIDocument UI;
    
    public List<EntitySceneReference> LevelScenes = new();

    private Entity _currentLevel;
    private int _currentLevelIndex;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(UI);
        _currentLevel = Entity.Null;
        _currentLevelIndex = -1;
    }

    private VisualElement _levelPicker;
    private VisualElement _gameOverScreen;
    // Start is called before the first frame update
    void Start()
    {
        _levelPicker = UI.rootVisualElement.Q<VisualElement>(name: "LevelPicker");
        var levelList = _levelPicker.Query<Button>().ToList();

        for (int i = 0; i < levelList.Count; i++)
        {
            var level = i;
            levelList[i].clicked += () => StartGameAtLevel(level);
        }

        _gameOverScreen = UI.rootVisualElement.Q<VisualElement>(name: "GameOver");
        _gameOverScreen.visible = false;
        
        var back = _gameOverScreen.Q<Button>(name: "Back");
        if(back != null)
            back.clicked += LoadMainMenu;
        var retry = _gameOverScreen.Q<Button>(name: "Retry");
        if(retry != null)
            retry.clicked += () => LoadLevel(_currentLevelIndex);
    }



    private void StartGameAtLevel(int level)
    {
        LoadGameScene();
        LoadLevel(level);
    }
    
    public void LoadGameScene()
    {
        SceneManager.LoadScene(GameScene,LoadSceneMode.Single);
        
        SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, Scene.SceneGUID,
            SceneSystem.UnloadParameters.DestroyMetaEntities);
    }
    
    public void LoadLevel(int level)
    {
        _levelPicker.visible = false;
        _gameOverScreen.visible = false;
        Debug.Log($"Loading level {level}");
        if (!Entity.Null.Equals(_currentLevel))
        {
            SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged,
                _currentLevel,SceneSystem.UnloadParameters.DestroyMetaEntities);
        }

        // If the level is not valid, load the main main menu instead
        if (LevelScenes.Count <= level)
        {
            LoadMainMenu();
        }
        else
        {
            _currentLevelIndex = level;
            _currentLevel =
                SceneSystem.LoadSceneAsync(
                    World.DefaultGameObjectInjectionWorld.Unmanaged, 
                    LevelScenes[level]);
        }
    }

    public void LoadMainMenu()
    {
        if (!Entity.Null.Equals(_currentLevel))
        {
            SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged,_currentLevel,SceneSystem.UnloadParameters.DestroyMetaEntities);
        }
        _currentLevelIndex = -1;
        _currentLevel = Entity.Null;
        SceneManager.LoadScene(MenuScene, LoadSceneMode.Single);
        _gameOverScreen.visible = false;
    }

    public void ShowGameOverScreen()
    {
        Debug.Log("ShowGameOverScreen");
        _gameOverScreen.visible = true;
    }
    
    public void NextLevel()
    {
        LoadLevel(_currentLevelIndex + 1);
    }


}
