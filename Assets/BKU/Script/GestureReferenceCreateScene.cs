using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GestureReferenceCreateScene : MonoBehaviour
{

    public BKU_GestureInput leftGesture;
    public BKU_GestureInput RightGesture;
    public Text sceneText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IUpdate());
    }
    IEnumerator IUpdate()
    {
    
        sceneText.text = "손바닥 펴고 스페이스";
        yield return new WaitForSeconds(3.0f);
        DataStream.Save(leftGesture.GestureData, "leftPaperReference");
        DataStream.Save(RightGesture.GestureData, "rightPaperReference");

        sceneText.text = "주먹쥐고 스페이스";
        yield return new WaitForSeconds(3.0f);

        DataStream.Save(leftGesture.GestureData, "leftRockReference");
        DataStream.Save(RightGesture.GestureData, "rightRockReference");
        sceneText.text = "완료.";

    }
}
