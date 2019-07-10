using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGrower : MonoBehaviour {
    RectTransform rt;
    Text txt;

    // Start is called before the first frame update
    void Start() {
        rt = gameObject.GetComponent<RectTransform>();
        txt = gameObject.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update() {
        rt.sizeDelta = new Vector2(rt.rect.width, txt.preferredHeight);
    }
}
