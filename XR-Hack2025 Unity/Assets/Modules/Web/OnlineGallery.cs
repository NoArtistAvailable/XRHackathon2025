using System;
using System.Collections.Generic;
using System.Linq;
using elZach.Common;
using UnityEngine;

public class OnlineGallery : MonoBehaviour
{
    public AnimatableChildren emptyPrefab;
    void Start()
    {
        OnlineManager.instance.onGotObjects += OnObjectsLoaded;
        OnlineManager.instance.GetObjectsAsync();
    }

    private void OnDestroy()
    {
        OnlineManager.instance.onGotObjects -= OnObjectsLoaded;
    }

    private async void OnObjectsLoaded(List<OnlineManager.ScoreData> dataList)
    {
        var positionInParent = new Vector3();
        for (int i = 0; i < dataList.Count; i++)
        {
            await WebTask.Delay(1);
            var data = dataList[i];
            var spawned = CreateFromData(data, out var bounds);
            Debug.Log(bounds);
            spawned.SetParent(this.transform, false);
            spawned.localPosition = positionInParent;
            positionInParent.x += bounds.size.x;
            var anim = spawned.GetComponent<AnimatableChildren>();
            anim.SetTo(0);
            anim.Play(1);
        }
    }

    public Transform CreateFromData(OnlineManager.ScoreData data, out Bounds bounds)
    {
        var empty = Instantiate(emptyPrefab).transform;
        
        bounds = new Bounds();
        var list = new List<Transform>();
        foreach (var partData in data.parts)
        {
            if(partData.index < 0 || partData.index >= OnlineManager.instance.library.prefabs.Length) continue;
            var prefab = OnlineManager.instance.library.prefabs[partData.index];
            var instance = Instantiate(prefab, empty);
                
            var localPos = new Vector3(partData.posX, partData.posY, partData.posZ);
            var localRot = Quaternion.Euler(partData.rotX, partData.rotY, partData.rotZ);
            var localScale = Vector3.one * partData.scale;
            instance.transform.localPosition = localPos;
            instance.transform.localRotation = localRot;
            instance.transform.localScale = localScale;
                
            var allComponents = instance.GetComponentsInChildren<Component>();
            for (int j = allComponents.Length - 1; j >= 0; j--)
            {
                if (allComponents[j] as Transform) continue;
                if (allComponents[j] as MeshFilter) continue;
                if (allComponents[j] is Renderer rend)
                {
                    if (bounds.extents.x == 0)
                    {
                        bounds = rend.bounds;
                    }
                    else
                    {
                        bounds.Encapsulate(rend.bounds);
                    }
                    continue;
                }
                Destroy(allComponents[j]);
            }
            list.Add(instance.transform);
        }

        foreach (var instance in list)
        {
            instance.localPosition -= bounds.center;
            instance.localPosition += Vector3.up * bounds.extents.y;
        }
        
        return empty;
    }
}
