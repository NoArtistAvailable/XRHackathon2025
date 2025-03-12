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
            var empty = Instantiate(emptyPrefab).transform;
            empty.SetParent(this.transform);
            empty.localPosition = positionInParent;
            var bounds = new Bounds();
            foreach (var partData in data.parts)
            {
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
                            bounds = rend.localBounds;
                        }
                        else
                        {
                            bounds.Encapsulate(rend.localBounds);
                        }
                        continue;
                    }
                    Destroy(allComponents[j]);
                }
                
            }

            positionInParent.x += bounds.size.x;
            var anim = empty.GetComponent<AnimatableChildren>();
            anim.SetTo(0);
            anim.Play(1);
        }
    }
}
