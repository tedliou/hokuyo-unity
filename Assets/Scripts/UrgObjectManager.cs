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
    
    private List<UrgObjectData> _updateCoroutines = new List<UrgObjectData>();

    private void Start()
    {
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
            var target = _updateCoroutines.First(c => c.urgObject == e);
            target.cancel = true;
            Debug.Log(target.cancel);
        };
    }

    private IEnumerator UpdateUrgObject(UrgObjectData data)
    {
        yield return null;
        
        //data.instance = Instantiate(urgPrefab);
        while (!data.cancel)
        {
            // data.instance.transform.position =
            //     Camera.main.ScreenToWorldPoint(data.urgObject.position * .1f + new Vector3(960, 0, 10));
            
            // Debug.Log(data.urgObject.position.magnitude);
            
            yield return new WaitForFixedUpdate();
        }
        
        //Destroy(data.instance);
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
