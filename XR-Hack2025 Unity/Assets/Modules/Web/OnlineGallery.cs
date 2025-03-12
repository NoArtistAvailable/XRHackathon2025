using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnlineGallery : MonoBehaviour
{
    
    void Start()
    {
        OnlineManager.instance.onGotObjects += OnObjectsLoaded;
        OnlineManager.instance.GetObjectsAsync();
    }

    private void OnDestroy()
    {
        OnlineManager.instance.onGotObjects -= OnObjectsLoaded;
    }

    private void OnObjectsLoaded(List<OnlineManager.ScoreData> dataList)
    {
        var positionInParent = new Vector3();
        for (int i = 0; i < dataList.Count; i++)
        {
            var data = dataList[i];
            var empty = new GameObject(data.name).transform;
            empty.SetParent(this.transform);
            empty.localPosition = positionInParent;
            foreach (var partData in data.parts)
            {
                var prefab = OnlineManager.instance.library.prefabs[partData.index];
                var instance = Instantiate(prefab, empty);

                // leave only renderer data
                var allComponents = instance.GetComponentsInChildren<Component>();
                for (int j = allComponents.Length - 1; j >= 0; j--)
                {
                    if (allComponents[j] as Transform) continue;
                    if (allComponents[j] as Renderer) continue;
                    if (allComponents[j] as MeshFilter) continue;
                    Destroy(allComponents[j]);
                }
                
                var localPos = new Vector3(partData.posX, partData.posY, partData.posZ);
                var localRot = Quaternion.Euler(partData.rotX, partData.rotY, partData.rotZ);
                var localScale = Vector3.one * partData.scale;
                instance.transform.localPosition = localPos;
                instance.transform.localRotation = localRot;
                instance.transform.localScale = localScale;
            }
        }
    }
}
