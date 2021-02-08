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
using BoneId = OVRSkeleton.BoneId;
 public enum GestureList
 {

     error = -1,
     none = 0,
     grab,
     grabReady,
     gun


 }



[Flags]
enum ThumbTipGesture
{
    error = -1,
    none = 0,
    dog = 1 << BoneId.Hand_MiddleTip | 1 << BoneId.Hand_RingTip,
    peace = 1 << BoneId.Hand_Middle3 | 1 << BoneId.Hand_Ring3,
    max = int.MaxValue
}

public enum HandType
{
    Left,
    Right
}

public class BKU_GestureInput : MonoBehaviour
{
    public struct BoneData
    {
        public BoneId id;
        public float distance;
    }

    [SerializeField]
    public HandType handType;
    BoneData[] boneData;
    BKU_GestureData PaperReference;
    BKU_GestureData RockReference;
    
    int boneCount;
    public Text debugText;


    public bool IsInitialized { get; private set; }
    public OVRHand Hand { get; private set; }
    public OVRSkeleton Skeleton { get; private set; }
    public BKU_GestureInput OtherHandGesture { get; private set; }
    public GestureList GestureState { get; private set; }
    public BKU_GestureData GestureData { get; private set; }
    public bool IsGrab { get; private set; }
    public float GrabStrength { get; private set; }


    

    private void Awake()
    {
        if (handType == HandType.Left)
        {
            OtherHandGesture = GameObject.Find("Left_hand_BKU").GetComponent<BKU_GestureInput>();
            PaperReference = DataStream.Load<BKU_GestureData>("leftPaperReference");
            RockReference = DataStream.Load<BKU_GestureData>("leftRockReference");
        }

        else if(handType == HandType.Right)
        {
 
            OtherHandGesture = GameObject.Find("Right_hand_BKU").GetComponent<BKU_GestureInput>();
            RockReference = DataStream.Load<BKU_GestureData>("rightRockReference");
            PaperReference = DataStream.Load<BKU_GestureData>("rightPaperReference");

        }
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Hand = GetComponent<OVRHand>();
        Skeleton = GetComponent<OVRSkeleton>();

        while (true)
        {
            if (Skeleton.IsInitialized)
            {
                Initialize();
                IsInitialized = true;
                yield break;
            }
            else
            {

                yield return null;
            }
        }
    }

    void Update()
    {
        if (IsInitialized)
        {
            if (Hand.IsTracked && Hand.IsDataHighConfidence)
            {

                DataSettig();
                Grab();

                GestureState = GestureList.none;
                if (IsGrab)
                {
                    GestureState = GestureList.grab;
                }
            }
            else
            {
                Defulte();
            }
        }
        
    }
    void Defulte()
    {
        GestureState = GestureList.none;
        IsGrab = false;
        GrabStrength = 0;
    }

    void Initialize()
    {
        boneCount = Skeleton.Bones.Count;
        GestureData = new BKU_GestureData(boneCount - 1, (int)BoneId.Hand_End);
        boneData = new BoneData[boneCount - 1];
        IsInitialized = true;
    }
    



    void DataSettig()
    {
        float[] pingerAngle = new float[(int)BoneId.Hand_End];
        for (int i = 0; i < GestureData.Distance.Length; i++)
        {
            for (int j = 0; j < GestureData.Distance[i].Length; j++)

            {
                GestureData.Distance[i][j] = (Skeleton.Bones[j].Transform.position - Skeleton.Bones[i + 1].Transform.position).magnitude;
                
                GestureData.Distance[i][j] = GestureData.Distance[i][j];

            }
        }
        for(int i=(int)BoneId.Hand_Thumb0;i< (int)BoneId.Hand_End; i++)
        {
            Vector3 my;
            Vector3 parent;
            Vector3 parent2;
            float dot;
            float sing;

            my = Skeleton.Bones[i].Transform.right;
            if (i < (int)BoneId.Hand_MaxSkinnable)
            {
                parent = Skeleton.Bones[i].Transform.parent.right;
                parent2 = Skeleton.Bones[i].Transform.parent.up;
                dot = Vector3.Dot(my, parent);
                sing = Vector3.Dot(my, parent2) * -1;
                pingerAngle[i] = Mathf.Acos(dot) * Mathf.Rad2Deg * Mathf.Sign(sing);


            }
            else
            {
                pingerAngle[i] = PingerTipAngleRecursive(Skeleton.Bones[i].Transform, ref pingerAngle);
            }
                GestureData.Angle[i] = pingerAngle[i];
        }


    }   
    //������ �հ��� ������ ������ ��ͷ� Ž���� ���ϴ� �Լ�
    float PingerTipAngleRecursive(Transform transform, ref float[] pingerAngle, float result = 0)
    {
        Transform parent = transform.parent;
        BoneId boneId;
        Enum.TryParse(parent.name, out boneId);

        if (boneId==BoneId.Hand_Start)
        {
            return result;
        }
        else
        {
            return PingerTipAngleRecursive(parent, ref pingerAngle, result + pingerAngle[(int)boneId]);
        }
    }


    float GetBoneDistance(BoneId start, BoneId end)
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

        return GestureData.Distance[max - 1][min];
  



    }
    BoneData[] GetBonesDistance()
    {

        BoneId basePoint = BoneId.Hand_ThumbTip;
        int index = (int)BoneId.Hand_Start;
        Vector3 standard = Skeleton.Bones[(int)basePoint].Transform.position;
        BoneData[] result = new BoneData[boneCount];

        foreach (var e in Skeleton.BindPoses)
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
        for (int i = (int)BoneId.Hand_ThumbTip; i < (int)BoneId.Hand_End; i++)
        {
            float angle = GestureData.Angle[i];
            float rockAngle = RockReference.Angle[i];
            float paperAngle = PaperReference.Angle[i];

            angle -= rockAngle;
            paperAngle -= rockAngle;
            float ratio = angle / paperAngle;
            isGrab = isGrab & (ratio < 0.5f);
            result += ratio;

        }
        result /= 5;
        IsGrab = result < 0.4f & isGrab;
        if (IsGrab  && result > 0)
            GrabStrength = 1 - result;
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
    string UIText2(float[][] data, BoneId start, BoneId end)
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

   //    int index = (int)BoneId.Hand_Start;
   //    Vector3 standard = skeleton.Bones[(int)BoneId.Hand_ThumbTip].Transform.position;


   //    foreach (var e in skeleton.BindPoses)
   //    {

   //        if (e.Id == BoneId.Hand_ThumbTip || e.Id == BoneId.Hand_Thumb3 || e.Id == BoneId.Hand_Thumb2)
   //        {
   //            continue;
   //        }
   //        thumbDistance[index].id = e.Id;
   //        thumbDistance[index].distance = GetBoneDistance(BoneId.Hand_ThumbTip, e.Id);
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

