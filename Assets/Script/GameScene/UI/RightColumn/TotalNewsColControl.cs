using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalNewsColControl : MonoBehaviour,ITotalColControl
{

    public GameValue gameValue;
    GameValue ITotalColControl.gameValue
    {
        get => gameValue;
        set => gameValue = value;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void ShowOrHide(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

}
