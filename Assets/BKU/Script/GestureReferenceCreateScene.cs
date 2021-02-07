using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GestureReferenceCreateScene : MonoBehaviour
{
    BKU_GestureData paperReference;
    BKU_GestureData rockReference;
    public BKU_Gesture customGesture;
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
        DataStream.Save(customGesture.gestureData, "paperReference");

        sceneText.text = "주먹쥐고 스페이스";
        yield return new WaitForSeconds(3.0f);

        DataStream.Save(customGesture.gestureData, "rockReference");
        sceneText.text = "완료. 로드 테스트 진행.";
        paperReference = DataStream.Load<BKU_GestureData>("paperReference");
        rockReference = DataStream.Load<BKU_GestureData>("rockReference");

        string debugtext;
        debugtext = customGesture.UIText(paperReference.Distance);
        sceneText.text = debugtext;
    }

    bool Detect()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

}
