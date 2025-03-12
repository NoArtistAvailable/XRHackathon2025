using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using elZach.Common;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class OnlineManager : MonoBehaviour
{
    public static OnlineManager instance => _instance.OrSet(ref _instance, FindAnyObjectByType<OnlineManager>);
    private static OnlineManager _instance;
    public static string gameName => "hackathon-dessau-2025";
    public const string serverUrl = "https://elzach-gamejams.glitch.me";
    
    public PrefabLibrary library;

    public Transform pedestal;
    public Animatable showcaseParent;
    public Transform spawnedShowcaseObject;
    public GameObject titleCardPrefab;

    public async void GetSculptureAndUpload()
    {
        var list = new List<Transform>();
        foreach (var sticky in StickySurface.active)
        {
            if (!sticky.GetComponent<CreationBehaviour>()) continue;
            list.Add(sticky.transform);
            sticky.transform.SetParent(pedestal, true);
        }

        if (list.Count == 0) return;
        var data = CreateObjectFromTransforms(GetRandomName(), list);
        PostObjectAsync(data);
        if (spawnedShowcaseObject)
        {
            await showcaseParent.Play(0);
            Destroy(spawnedShowcaseObject.gameObject);
        }
        var clone = OnlineGallery.CreateFromData(data, new GameObject("Showcase").transform, out var bounds);
        clone.SetParent(showcaseParent.transform, false);
        spawnedShowcaseObject = clone;

        var titleCard = Instantiate(titleCardPrefab, clone);
        titleCard.transform.localPosition = Vector3.up * (bounds.size.y + 0.2f);
        
        showcaseParent.Play(1);
        foreach(var entry in list) Destroy(entry.gameObject);
    }

    public string GetRandomName()
    {
        return "dev_test_" + Random.Range(0, 10000);
    }
    
    [Serializable]
    public class ScoreData
    {
        public string name;
        public float score;
        public bool lowerIsBetter;
        public PartMeta[] parts;
    }

    [Serializable]
    public class PartMeta
    {
        public int index;
        public float posX;
        public float posY;
        public float posZ;
        public float scale;
        public float rotX;
        public float rotY;
        public float rotZ;

        public static PartMeta FromTransform(Transform target)
        {
            var meta = new PartMeta();
            meta.index = instance.library.prefabs.ToList().FindIndex(x => x.name == target.name);
            meta.posX = target.localPosition.x;
            meta.posY = target.localPosition.y;
            meta.posZ = target.localPosition.z;
            meta.scale = target.localScale.x;
            meta.rotX = target.localRotation.eulerAngles.x;
            meta.rotY = target.localRotation.eulerAngles.y;
            meta.rotZ = target.localRotation.eulerAngles.z;
            return meta;
        }
    }
    
    public class ScoreDataList
    {
        public List<ScoreData> scores;
    }

    [Serializable]
    public class ScoreGetData
    {
        public string name;
        public float score;
        public int placement;
        public PartMeta[] parts;
    }

    public event Action<List<ScoreData>> onGotObjects;

    public ScoreData CreateObjectFromTransforms(string objectName, IEnumerable<Transform> obj)
    {
        return new ScoreData
        {
            name = objectName,
            score = 1,
            lowerIsBetter = true,
            parts = obj.Select(x=>PartMeta.FromTransform(x)).ToArray()
        };
    }


    public void PostObjectAsync(string objectName, IEnumerable<Transform> obj) =>
        PostObjectAsync(CreateObjectFromTransforms(objectName, obj));
    public async void PostObjectAsync(ScoreData newScore)
    {
        string url = $"{serverUrl}/highscores/{gameName}";

        // Convert the data object to JSON
        string json = JsonUtility.ToJson(newScore);
        Debug.Log("Sending JSON: " + json);

        // Create a UnityWebRequest for POST
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and await the response
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("POST request successful: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"POST request failed: {request.error}");
        }
    }
    // Async method to GET high scores for a game
    public async void GetObjectsAsync()
    {
        string url = $"{serverUrl}/allhighscores/{gameName}";

        // Create a UnityWebRequest for GET
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Send the request and await the response
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("GET request successful: " + request.downloadHandler.text);
            // Deserialize JSON to a List<ScoreData>
            var json = request.downloadHandler.text;
            ScoreDataList scoreDataList = JsonUtility.FromJson<ScoreDataList>("{\"scores\":" + json + "}");
            for (var i = 0; i < scoreDataList.scores.Count; i++)
            {
                var entry = scoreDataList.scores[i];
                Debug.Log($"[{i}] {entry.name} : {entry.score}");
            }
            onGotObjects?.Invoke(scoreDataList.scores);
        }
        else
        {
            Debug.LogError($"GET request failed: {request.error}");
        }
    }
}
