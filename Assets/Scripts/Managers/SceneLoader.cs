using SheetCodes;
using System;
using System.Collections;
using System.Threading;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance { private set; get; }

    public static string applicationPath;

    private const string MAIN_MENU_SCENE = "Main Menu";
    private const string GAME_ESSENTIALS_SCENE_NAME = "Game Essentials";
    private const string LOADING_SCENE_NAME = "Scene Loader";

    public bool isLoading => routine_Load != null;

    [SerializeField] private LoadingScreenUI loadingScreenUI = default;

    public SceneIdentifier currentSceneIdentifier => currentScene != null ? currentScene.Identifier : SceneIdentifier.None;
    public SceneRecord currentScene { private set; get; }

    private SceneRecord preparingSceneRecord;
    private bool isPreparing;

    private Coroutine routine_Load;
    private Coroutine routine_PrepareScene;

    private byte[] binaryMapData;
    private PathFindingData_Level preparedMapData;
    private bool loadedMapData;
    private PathFindingSystem pathFindingSystem;

    private void Awake()
    {
        instance = this;

        Time.fixedDeltaTime = 1 / 60f;

        pathFindingSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PathFindingSystem>();
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        Application.runInBackground = true;
        applicationPath = Application.dataPath;
        SceneIdentifier[] identifiers = Enum.GetValues(typeof(SceneIdentifier)) as SceneIdentifier[];

        bool found = false;
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {

            Scene scene = SceneManager.GetSceneAt(i);
            if(scene.name == MAIN_MENU_SCENE)
            {
                found = true;
                break;
            }

            foreach(SceneIdentifier identifier in identifiers)
            {
                if (identifier == SceneIdentifier.None)
                    continue;

                SceneRecord record = identifier.GetRecord();
                if (!scene.name.Equals(record.SceneName))
                    continue;

                currentScene = record;
                found = true;
                break;
            }
        }

        if(!found)
            StartCoroutine(ForceLoadMainMenu());
    }

    private IEnumerator ForceLoadMainMenu()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(MAIN_MENU_SCENE, LoadSceneMode.Additive);
        while (!operation.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(MAIN_MENU_SCENE));
    }

    public void LoadMainMenu()
    {
        if (routine_Load != null)
        {
            UnityEngine.Debug.Log("Cannot load while loading is in progress");
            return;
        }

        routine_Load = StartCoroutine(Routine_LoadMainMenu());
    }

    private IEnumerator Routine_LoadMainMenu()
    {
        loadingScreenUI.showUI.value = true;
        while (loadingScreenUI.isFading)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(LOADING_SCENE_NAME));
        Time.timeScale = 0;

        AsyncOperation operation;

        operation = SceneManager.UnloadSceneAsync(currentScene.SceneName);
        currentScene.Identifier.UnloadScene();
        while (!operation.isDone)
            yield return null;

        operation = SceneManager.UnloadSceneAsync(GAME_ESSENTIALS_SCENE_NAME);
        while (!operation.isDone)
            yield return null;

        preparingSceneRecord = null;
        currentScene = null;

        operation = SceneManager.LoadSceneAsync(MAIN_MENU_SCENE, LoadSceneMode.Additive);
        while (!operation.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(MAIN_MENU_SCENE));

        Time.timeScale = 1;
        loadingScreenUI.showUI.value = false;
        routine_Load = null;
    }

    public void PrepareLoadScene(SceneIdentifier identifier)
    {
        if (routine_PrepareScene != null)
            StopCoroutine(routine_PrepareScene);

        SceneRecord record = identifier.GetRecord();
        routine_PrepareScene = StartCoroutine(Routine_PrepareGameScene(record));
    }

    public void LoadGameScene(SceneIdentifier identifier)
    {
        if(routine_Load != null)
        {
            UnityEngine.Debug.Log("Cannot load while loading is in progress");
            return;
        }

        SceneRecord record = identifier.GetRecord();
        routine_Load = StartCoroutine(Routine_LoadGameScene(record));
    }

    private IEnumerator Routine_LoadGameScene(SceneRecord record)
    {
        routine_PrepareScene = StartCoroutine(Routine_PrepareGameScene(record));

        loadingScreenUI.showUI.value = true;
        while (loadingScreenUI.isFading)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(LOADING_SCENE_NAME));
        Time.timeScale = 0;

        AsyncOperation operation;
        if (currentScene != null)
        {
            //Coming from another game scene
            operation = SceneManager.UnloadSceneAsync(currentScene.SceneName);
            currentScene.Identifier.UnloadScene();
            while (!operation.isDone)
                yield return null;

            currentScene = null;
        }
        else
        {
            //Coming from main menu
            operation = SceneManager.UnloadSceneAsync(MAIN_MENU_SCENE);
            while (!operation.isDone)
                yield return null;

            operation = SceneManager.LoadSceneAsync(GAME_ESSENTIALS_SCENE_NAME, LoadSceneMode.Additive);
            while (!operation.isDone)
                yield return null;
        }

        operation = SceneManager.LoadSceneAsync(record.SceneName, LoadSceneMode.Additive);
        currentScene = record;

        while (!operation.isDone)
            yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentScene.SceneName));

        while (routine_PrepareScene != null)
            yield return null;

        if (loadedMapData)
            pathFindingSystem.Initialize(preparedMapData);
        else
            pathFindingSystem.SetCreateMapData();

        yield return ManagerController.instance.Routine_Initialize();

        Time.timeScale = 1;
        loadingScreenUI.showUI.value = false;
        routine_Load = null;
    }

    private IEnumerator Routine_PrepareGameScene(SceneRecord record)
    {
        //Can't interrupt a threaded load
        while (isPreparing)
            yield return null;

        if (preparingSceneRecord == record)
        {
            routine_PrepareScene = null;
            yield break;
        }

        preparingSceneRecord = record;

        string fileToLoad = PathFindingSystem.GetFileToLoad(record);
        ResourceRequest request = Resources.LoadAsync<TextAsset>(fileToLoad);
        while (!request.isDone)
            yield return null;

        TextAsset textAsset = request.asset as TextAsset;
        if (textAsset == null)
        {
            routine_PrepareScene = null;
            binaryMapData = null;
            loadedMapData = false;
            preparedMapData = default;
            yield break;
        }

        binaryMapData = textAsset.bytes;

        isPreparing = true;

        Thread t = new Thread(new ThreadStart(PrepareScene));
        t.Start();
        
        while (isPreparing)
            yield return null;

        routine_PrepareScene = null;
    }

    private void PrepareScene()
    {
        loadedMapData = true;
        preparedMapData = ByteSerializer.ConvertToObject_Unsafe<PathFindingSaveData_Level>(binaryMapData).CreateData();
        isPreparing = false;
    }
}

public static class SceneIdentifierExtention
{
    public static void UnloadScene(this SceneIdentifier identifier)
    {
        switch (identifier)
        {
            case SceneIdentifier.Level1:
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SceneSystem_Unload_Scene1>().UnloadScene();
                return;
            case SceneIdentifier.Level2:
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SceneSystem_Unload_Scene2>().UnloadScene();
                return;
        }

        throw new Exception("Missing Scene Identifier Implementation");
    }

    public static void AddSceneTag(this SceneIdentifier identifier, EntityManager entityManager, Entity target)
    {
        switch (identifier)
        {
            case SceneIdentifier.Level1:
                entityManager.AddComponentData(target, new SceneTag_Scene1 { });
                return;
            case SceneIdentifier.Level2:
                entityManager.AddComponentData(target, new SceneTag_Scene2 { });
                return;
        }

        throw new Exception("Missing Scene Identifier Implementation");
    }

    public static void AddSceneTag(this SceneIdentifier identifier, EntityCommandBuffer.ParallelWriter commandBuffer, int entityIndex, Entity target)
    {
        switch(identifier)
        {
            case SceneIdentifier.Level1:
                commandBuffer.AddComponent(entityIndex, target, new SceneTag_Scene1 { });
                return;
            case SceneIdentifier.Level2:
                commandBuffer.AddComponent(entityIndex, target, new SceneTag_Scene2 { });
                return;
        }

        throw new Exception("Missing Scene Identifier Implementation");
    }

    public static void AddSceneTag(this SceneIdentifier identifier, EntityCommandBuffer commandBuffer, Entity target)
    {
        switch (identifier)
        {
            case SceneIdentifier.Level1:
                commandBuffer.AddComponent(target, new SceneTag_Scene1 { });
                return;
            case SceneIdentifier.Level2:
                commandBuffer.AddComponent(target, new SceneTag_Scene2 { });
                return;
        }

        throw new Exception("Missing Scene Identifier Implementation");
    }
}