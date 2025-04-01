using System;
using HKY;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UrgObjectData
{
    public ProcessedObject urgObject;
    public bool cancel;
    public GameObject instance;
}

public class UrgObjectManager : MonoBehaviour
{
    public GameObject urgPrefab;
    public GameObject msgObj;
    public int distance;
    public int offsetY = 270;
    
    private List<UrgObjectData> _updateCoroutines = new List<UrgObjectData>();

    private void Start()
    {
        Application.targetFrameRate = 60;
        
        
        URGSensorObjectDetector.OnNewObject += e =>
        {
            _updateCoroutines.Add(new UrgObjectData()
            {
                urgObject = e,
                cancel = false
            });
            StartCoroutine(UpdateUrgObject(_updateCoroutines[^1]));
        };

        URGSensorObjectDetector.OnLostObject += e =>
        {
            var target = _updateCoroutines.FirstOrDefault(c => c.urgObject == e);
            if (target is null)
                return;
            
            target.cancel = true;
            // Debug.Log(target.cancel);
        };
    }

    private IEnumerator UpdateUrgObject(UrgObjectData data)
    {
        yield return null;
        
        var camPos = new Vector3(0, 0, 10);
        var urgPos = data.urgObject.position + new Vector3(URGSensorObjectDetector.Instance.detectRectWidth * .5f, 0);
        data.instance = Instantiate(urgPrefab);

        while (!data.cancel)
        {
            // while (urgPos.y < offsetY)
            // {
            //     data.instance.SetActive(false);
            //     yield return new WaitForFixedUpdate();
            // }

            
            data.instance.SetActive(true);

            urgPos = data.urgObject.position + new Vector3(URGSensorObjectDetector.Instance.detectRectWidth * .5f, 0);
            var norPos = new Vector3(urgPos.x / URGSensorObjectDetector.Instance.detectRectWidth, (urgPos.y - offsetY) / (URGSensorObjectDetector.Instance.detectRectHeight - offsetY));
            
            data.instance.transform.position =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * norPos.x, Screen.height * norPos.y) + camPos);
            
            yield return new WaitForFixedUpdate();
        }
        
        Destroy(data.instance);
        _updateCoroutines.Remove(data);
    }

    private void Update()
    {
        if (_updateCoroutines.Count == 0)
            return;
        
        _updateCoroutines.Sort((a, b) => a.urgObject.position.magnitude.CompareTo(b.urgObject.position.magnitude));
        
        var minDistance = _updateCoroutines[0].urgObject.position.magnitude;
        //Debug.Log(minDistance);

        if (minDistance < distance)
        {
            if (!msgObj.activeSelf)
            {
                msgObj.SetActive(true);
            }
        }
        else
        {
            if (msgObj.activeSelf)
            {
                msgObj.SetActive(false);
            }
        }
    }
}
