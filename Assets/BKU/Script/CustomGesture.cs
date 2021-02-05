using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 /*
  * enum ��ȯ�� ���� �������� ���� ���� �� ����. ����ȭ�ؾ� �Ҷ� Ȯ��.
  * �ڵ� Ʈ��ŷ�� ������ ���ķ� update�� ���۵Ǿ����.
  * 
  * ������ ���� ���.
  * 1. enum BasePoint�� ������ �� �� �߰�.
  * 2. ���� ���� ������ ������ enum 'BoneID'StandardGesture ������ ��Ʈ�÷��׷� ����
  * 3. GestureDetect() �Լ� ���� Switch(basePoint) �� ���̽� �߰� �� string ������ ��ȯ.
  * 
  * �ϵ��ڵ��κ� ���� �ٲ㺸��.
  * 
  * ���� ũ��� ������� �ٸ��� ������ üũ ������ �Ÿ��� �ƴ϶� �Ÿ� ������ �ؾ���.
  * 
  * 
  * scriptableobject�� json�̰�, ���� �����͸� �����ؾ� �ϱ� ������ ������ json���� �Ľ��� save, load�ϴ� �κе� �����ؾ��Ѵ�.
  * */
 public enum GestureList
 {

     error = -1,
     none = 0,
     dog,
     peace,
     grab,
     grabReady,
     gun


 }



[Flags]
enum ThumbTipStandardGesture
{
    error = -1,
    none = 0,
    dog = 1 << OVRSkeleton.BoneId.Hand_MiddleTip | 1 << OVRSkeleton.BoneId.Hand_RingTip,
    peace = 1 << OVRSkeleton.BoneId.Hand_Middle3 | 1 << OVRSkeleton.BoneId.Hand_Ring3,
    max = int.MaxValue
}


public class CustomGesture : MonoBehaviour
{
    public struct BoneData
    {
        public OVRSkeleton.BoneId id;
        public float distance;
    }


    BoneData[] boneData;
    public OVRHand hand;
    public OVRSkeleton skeleton;
    GestureData paperReference;
    GestureData rockReference;
    [HideInInspector]
    public GestureData gestureData;

    float[][] referenceDistance;
    float[][] bonesDistance;
    float[][] bonesDistanceRatio;
    float[] pingerAngle;
    int boneCount;
    public Text debugText;

    [Range(-1,1)]
    public float testFloat;



    [HideInInspector]
    public bool isInitialized;


    Vector3 temp23;

    public GestureList GestureState { get; private set; }
    public bool IsGrab { get; private set; }
    public bool IsGrabReady { get; private set; }
    public float GrabStrength { get; private set; }
    

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {

        while (true)
        {
            if (hand.IsTracked)
            {
                Initialize();
                yield return StartCoroutine(IUpdate());
            }
            else
            {

                yield return null;
            }
        }
    }

    IEnumerator IUpdate()
    {
        while (true)
        {
            if (hand.IsTracked && hand.IsDataHighConfidence)
            {

                DataSettig();
                Grab();
               
                    GestureState = GestureList.none;
                if(IsGrab)
                {
                    GestureState = GestureList.grab;
                }
                if(IsGrabReady)
                {

                    GestureState = GestureList.grabReady;
                }


            }
            yield return null;
        }
    }
    void Update()
    {
        
        
    }
    void Initialize()
    {

        boneCount = skeleton.Bones.Count;
        boneData = new BoneData[boneCount - 1];
        bonesDistance = new float[boneCount - 1][];
        bonesDistanceRatio = new float[boneCount - 1][];
        referenceDistance = new float[boneCount - 1][];


        paperReference = DataStream.Load<GestureData>("paperReference");
        rockReference = DataStream.Load<GestureData>("rockReference");
        gestureData = new GestureData(boneCount - 1, (int)OVRSkeleton.BoneId.Hand_End);

        for (int i = 0; i < boneCount - 1; i++)
        {
            bonesDistance[i] = new float[i + 1];
            bonesDistanceRatio[i] = new float[i + 1];
            referenceDistance[i] = new float[i + 1];
        }

        pingerAngle = new float[(int)OVRSkeleton.BoneId.Hand_End];

        isInitialized = true;


    }
    


    void SetReferenceDistanceMatrix()
    {
        for (int i = 0; i < referenceDistance.Length; i++)
        {
            for (int j = 0; j < referenceDistance[i].Length; j++)
            {
                referenceDistance[i][j] = (skeleton.Bones[j].Transform.position - skeleton.Bones[i + 1].Transform.position).magnitude;
            }
        }
    }
    void DataSettig()
    {
        for (int i = 0; i < bonesDistance.Length; i++)
        {
            for (int j = 0; j < bonesDistance[i].Length; j++)
            {
                bonesDistance[i][j] = (skeleton.Bones[j].Transform.position - skeleton.Bones[i + 1].Transform.position).magnitude;
                bonesDistanceRatio[i][j] = bonesDistance[i][j] / referenceDistance[i][j];

                gestureData.Distance[i][j] = bonesDistance[i][j];

            }
        }
        for(int i=(int)OVRSkeleton.BoneId.Hand_Thumb0;i< (int)OVRSkeleton.BoneId.Hand_End; i++)
        {
            Vector3 my;
            Vector3 parent;
            Vector3 parent2;
            float dot;
            float sing;

            my = skeleton.Bones[i].Transform.right;
            if (i < (int)OVRSkeleton.BoneId.Hand_MaxSkinnable)
            {
                parent = skeleton.Bones[i].Transform.parent.right;
                parent2 = skeleton.Bones[i].Transform.parent.up;
                dot = Vector3.Dot(my, parent);
                sing = Vector3.Dot(my, parent2) * -1;
                pingerAngle[i] = Mathf.Acos(dot) * Mathf.Rad2Deg * Mathf.Sign(sing);


            }
            else
            {
                pingerAngle[i] = PingerTipAngleRecursive(skeleton.Bones[i].Transform);
            }
                gestureData.Angle[i] = pingerAngle[i];
        }


    }   
    //������ �հ��� ������ ������ ��ͷ� Ž���� ���ϴ� �Լ�
    float PingerTipAngleRecursive(Transform transform, float result = 0)
    {
        Transform parent = transform.parent;
        OVRSkeleton.BoneId boneId;
        Enum.TryParse(parent.name, out boneId);

        if (boneId==OVRSkeleton.BoneId.Hand_Start)
        {
            return result;
        }
        else
        {
            return PingerTipAngleRecursive(parent, result + pingerAngle[(int)boneId]);
        }
    }


    float GetBoneDistance(OVRSkeleton.BoneId start, OVRSkeleton.BoneId end)
    {
        int min = (int)start;
        int max = (int)end;
        int temp;
        if (max < min)
        {
            temp = max;
            max = min;
            min = temp;
        }

        return bonesDistance[max - 1][min];
  



    }
    BoneData[] GetBonesDistance()
    {

        OVRSkeleton.BoneId basePoint = OVRSkeleton.BoneId.Hand_ThumbTip;
        int index = (int)OVRSkeleton.BoneId.Hand_Start;
        Vector3 standard = skeleton.Bones[(int)basePoint].Transform.position;
        BoneData[] result = new BoneData[boneCount];

        foreach (var e in skeleton.BindPoses)
        {

            if (e.Id == basePoint)

            {
                continue;
            }

            boneData[index].id = e.Id;
            boneData[index].distance = GetBoneDistance(basePoint, e.Id);
            index++;
        }

        Merge_sort(boneData, ref result, 0, boneData.Length - 1);

        return result;
    }
    #region ������ �ϵ��ڵ�

    void Grab()
    {
        float result = 0;
        bool isGrab = true;
        bool isGrabReady = true;
        for (int i = (int)OVRSkeleton.BoneId.Hand_ThumbTip; i < (int)OVRSkeleton.BoneId.Hand_End; i++)
        {
            float angle = gestureData.Angle[i];
            float rockAngle = rockReference.Angle[i];
            float paperAngle = paperReference.Angle[i];

            angle -= rockAngle;
            paperAngle -= rockAngle;
            float ratio = angle / paperAngle;
            isGrab = isGrab & (ratio < 0.1f);
            isGrabReady = isGrabReady & (0.1f<= ratio && ratio < 0.7f);
            result += ratio;

        }
        result /= 5;
        IsGrab = isGrab;
        IsGrabReady = isGrabReady ;
        if(isGrab && isGrabReady && result>0)
            GrabStrength = 1-result;
        else
        {
            GrabStrength = 0;
        }

    }
    #endregion
    #region �ܺο��� ������ �Լ�
        void Merge(BoneData[] list, ref BoneData[] buffer, int left, int mid, int right)
        {

            int i, j, k, l;
            i = left;
            j = mid + 1;
            k = left;

            /* ���� ���ĵ� list�� �պ� */
            while (i <= mid && j <= right)
            {
                if (list[i].distance <= list[j].distance)
                    buffer[k++] = list[i++];
                else
                    buffer[k++] = list[j++];
            }

            // ���� �ִ� ������ �ϰ� ����
            if (i > mid)
            {
                for (l = j; l <= right; l++)
                    buffer[k++] = list[l];
            }
            // ���� �ִ� ������ �ϰ� ����
            else
            {
                for (l = i; l <= mid; l++)
                    buffer[k++] = list[l];
            }

            // �迭 buffer[](�ӽ� �迭)�� ����Ʈ�� �迭 list[]�� �纹��
            for (l = left; l <= right; l++)
            {
                list[l] = buffer[l];
            }
        }
        void Merge_sort(BoneData[] list, ref BoneData[] buffer, int left, int right)
        {
            int mid;

            if (left < right)
            {
                mid = (left + right) / 2; // �߰� ��ġ�� ����Ͽ� ����Ʈ�� �յ� ���� -����(Divide)
                Merge_sort(list, ref buffer, left, mid); // ���� �κ� ����Ʈ ���� -����(Conquer)
                Merge_sort(list, ref buffer, mid + 1, right); // ���� �κ� ����Ʈ ���� -����(Conquer)
                Merge(list, ref buffer, left, mid, right); // ���ĵ� 2���� �κ� �迭�� �պ��ϴ� ���� -����(Combine)
            }
        }
        #endregion
    #region ������ �Լ�

    public string UIText(float[][] data)
    {
        string result = "";

        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                result += TextAssemble(data[i][j]);
            }
            result += "\n";
        }


        return result;
    }
     string UIText(BoneData[] data)

    {
        string result = "";

        for (int i = 0; i < data.Length; i++)
        {

            result += TextAssemble(data[i].id, 1);


        }


        return result;
    }
    string UIText2(float[][] data, OVRSkeleton.BoneId start, OVRSkeleton.BoneId end)
    {
        int min = (int)start;
        int max = (int)end;
        int temp;
        if (max < min)
        {
            temp = max;
            max = min;
            min = temp;
        }
        return TextAssemble(data[max - 1][min], 1);
    }
    string TextAssemble<T>(T data, int index = -1)
    {

        string result = "";
        switch (index)
        {
            case 0:

                int temp = -1;
                if (typeof(float).IsAssignableFrom(data.GetType()))
                {
                    if (data as float? < 0.3)
                    {
                        temp = 1;
                    }
                    else
                    {
                        temp = 0;
                    }
                }
                result += "(" + temp + ")" + ", ";
                break;

            case 1:
                float[] temp2 = data as float[];
                    
                foreach(var e in temp2)
                {
                    result += "(" + (int)e + ")" + ", ";
                } 
                break;

            case 2:
                
                    float[][] temp3 = data as float[][];

                    for(int i=0;i< temp3.Length;i++)
                    {
                        for(int j=0;j< temp3[i].Length;j++)
                        {
                            result += "(" + (temp3[i][j]) + ")" + ", ";
                        }
                        result += "\n";
                    }
                    break;
                
            default:
                result += "(" + (data) + ")" + ", ";
                break;

        }
        return result;

    }
   #endregion
   #region ���Ž� �Լ�
   //void ThumbDetect()
   //{

   //    int index = (int)OVRSkeleton.BoneId.Hand_Start;
   //    Vector3 standard = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_ThumbTip].Transform.position;


   //    foreach (var e in skeleton.BindPoses)
   //    {

   //        if (e.Id == OVRSkeleton.BoneId.Hand_ThumbTip || e.Id == OVRSkeleton.BoneId.Hand_Thumb3 || e.Id == OVRSkeleton.BoneId.Hand_Thumb2)
   //        {
   //            continue;
   //        }
   //        thumbDistance[index].id = e.Id;
   //        thumbDistance[index].distance = GetBoneDistance(OVRSkeleton.BoneId.Hand_ThumbTip, e.Id);
   //        index++;
   //    }

   //    Merge_sort(thumbDistance, 0, thumbDistance.Length - 1);



   //}
   //GestureIndex SetGestureWithThumb(int detectCount) 
   //{
   //    GestureIndex result = GestureIndex.none;
   //    for (int i = 0; i < detectCount; i++)
   //    {
   //        if (buffer[i].distance > attachSensitivity)
   //        {
   //            return GestureIndex.none;
   //        }
   //    }
   //    int temp = 0;
   //    for (int i = 0; i < detectCount; i++)
   //    {
   //        temp |= 1 << (int)buffer[i].id;
   //    }
   //    result = (GestureIndex)temp;
   //    return result;
   //}


   //public GestureList GestureDetect(BasePoint basePoint)
   //{
   //    BoneData[] sortedBoneDistance = GetBonesDistance(basePoint);


   //    //���������� ���� ���˹ΰ��� �� ���� ���� ������ ID�� ��Ʈ�÷��׷� ����.
   //    int temp = 0;
   //    for (int i = 0; i < sortedBoneDistance.Length; i++)
   //    {
   //        if (sortedBoneDistance[i].distance > attachSensitivity)
   //        {
   //            if (i == 0)
   //            {
   //                return GestureList.none;
   //            }

   //            break;
   //        }

   //        temp |= 1 << (int)sortedBoneDistance[i].id;
   //    }


   //    //����� ��Ʈ �÷��׸� �������� ���� ���ǵ� ��Ʈ�÷����� string���� ��ȯ.
   //    string gestureName = "none";
   //    switch (basePoint)
   //    {
   //        case BasePoint.Hand_ThumbTip:
   //            gestureName = ((ThumbTipStandardGesture)temp).ToString();

   //            break;
   //        case BasePoint.Hand_Thumb0:
   //            break;
   //        default:

   //            break;
   //    }


   //    GestureList result = GestureList.error;
   //    Enum.TryParse(gestureName, out result);

   //    return result;
   //}
   #endregion




}

