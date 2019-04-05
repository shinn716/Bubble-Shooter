using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectForTimelineEvents: MonoBehaviour {

    public  void SetGameobjectEnable()
    {
        gameObject.SetActive(true);
    }

    public void SetGameobjectDisable()
    {
        gameObject.SetActive(false);
    }

    public void DestroyObjs()
    {
        Destroy(gameObject);
    }
 
}
