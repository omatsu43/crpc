using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleExt : Toggle
{
    public GameObject onGameObject;

    void Update() {
        if(isOn) {
            onGameObject.SetActive(true);
        } else {
            onGameObject.SetActive(false);
        }
    }
}
