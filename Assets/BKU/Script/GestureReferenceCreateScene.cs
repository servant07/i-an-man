using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GestureReferenceCreateScene : MonoBehaviour
{
    GestureData paperReference;
    GestureData rockReference;
    public CustomGesture customGesture;
    public Text sceneText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IUpdate());
    }
    IEnumerator IUpdate()
    {
    
        sceneText.text = "�չٴ� ��� �����̽�";
        yield return new WaitForSeconds(3.0f);
        DataStream.Save(customGesture.gestureData, "paperReference");

        sceneText.text = "�ָ���� �����̽�";
        yield return new WaitForSeconds(3.0f);

        DataStream.Save(customGesture.gestureData, "rockReference");
        sceneText.text = "�Ϸ�. �ε� �׽�Ʈ ����.";
        paperReference = DataStream.Load<GestureData>("paperReference");
        rockReference = DataStream.Load<GestureData>("rockReference");

        string debugtext;
        debugtext = customGesture.UIText(paperReference.Distance);
        sceneText.text = debugtext;
    }

    bool Detect()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

}
