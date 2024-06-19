using UnityEngine;
using UnityEngine.UI;
using System;

public class Test : MonoBehaviour {
    [SerializeField] Text datetimeText;
    private void Start ( ) 
    {
        DateTime currentDateTime = WorldTimeAPI.Instance.GetCurrentDateTime ( );

        datetimeText.text = currentDateTime.ToString ( );
    }

    void Update ( ) {
        if ( Input.GetMouseButtonUp ( 0 ) && WorldTimeAPI.Instance.IsTimeLodaed ) {
            DateTime currentDateTime = WorldTimeAPI.Instance.GetCurrentDateTime ( );

            datetimeText.text = currentDateTime.ToString ( );
        }
    }
}