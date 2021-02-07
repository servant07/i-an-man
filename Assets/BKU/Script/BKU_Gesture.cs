using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 /*
  * enum 변환이 많아 가비지가 많이 쌓일 수 있음. 최적화해야 할때 확인.
  * 핸드 트래킹이 시작한 이후로 update가 시작되어야함.
  * 
  * 제스쳐 제작 방법.
  * 1. enum BasePoint에 기준이 될 본 추가.
  * 2. 기준 본과 접촉할 본들을 enum 'BoneID'StandardGesture 형태의 비트플래그로 정의
  * 3. GestureDetect() 함수 안의 Switch(basePoint) 에 케이스 추가 후 string 형으로 변환.
  * 
  * 하드코딩부분 설계 바꿔보자.
  * 
  * 손의 크기는 사람마다 다르기 때문에 체크 기준을 거리가 아니라 거리 비율로 해야함.
  * 
  * 
  * scriptableobject가 json이고, 기준 데이터를 저장해야 하기 때문에 데이터 json으로 파싱해 save, load하는 부분도 제작해야한다.
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
enum Hand
{
    Left=0,
    Right,
    Any
}

public class BKU_Gesture : MonoBehaviour
{
    public struct BoneData
    {
        public BoneId id;
        public float distance;
    }


    BoneData[] boneData;
    [SerializeField]
    OVRHand hand;
    public OVRSkeleton skeleton;

    OVRHand rightHand;
    OVRHand leftHand;

    OVRSkeleton rightSkeleton;
    OVRSkeleton leftSkeleton;


    BKU_GestureData rightPaperReference;
    BKU_GestureData rightRockReference;
    [HideInInspector]
    public BKU_GestureData gestureData;
    int boneCount;
    public Text debugText;
    [HideInInspector]
    public bool isInitialized;


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
            if (skeleton.IsInitialized)
            {
                Initialize();
                isInitialized = true;
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
        if (isInitialized)
        {
            if (hand.IsTracked && hand.IsDataHighConfidence)
            {

                DataSettig();
                Grab();

                GestureState = GestureList.none;
                if (IsGrab)
                {
                    GestureState = GestureList.grab;
                }
                if (IsGrabReady)
                {

                    GestureState = GestureList.grabReady;
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
        IsGrabReady = false;
        GrabStrength = 0;
    }

    void Initialize()
    {

        boneCount = skeleton.Bones.Count;
        boneData = new BoneData[boneCount - 1];
        gestureData.Distance = new float[boneCount - 1][];
  
  


        rightPaperReference = DataStream.Load<BKU_GestureData>("rightPaperReference");
        rightRockReference = DataStream.Load<BKU_GestureData>("rightRockReference");
        gestureData = new BKU_GestureData(boneCount - 1, (int)BoneId.Hand_End);

        for (int i = 0; i < boneCount - 1; i++)
        {
            gestureData.Distance[i] = new float[i + 1];

        }


        isInitialized = true;


    }
    



    void DataSettig()
    {
        float[] pingerAngle = new float[(int)BoneId.Hand_End];
        for (int i = 0; i < gestureData.Distance.Length; i++)
        {
            for (int j = 0; j < gestureData.Distance[i].Length; j++)

            {
                gestureData.Distance[i][j] = (skeleton.Bones[j].Transform.position - skeleton.Bones[i + 1].Transform.position).magnitude;
                
                gestureData.Distance[i][j] = gestureData.Distance[i][j];

            }
        }
        for(int i=(int)BoneId.Hand_Thumb0;i< (int)BoneId.Hand_End; i++)
        {
            Vector3 my;
            Vector3 parent;
            Vector3 parent2;
            float dot;
            float sing;

            my = skeleton.Bones[i].Transform.right;
            if (i < (int)BoneId.Hand_MaxSkinnable)
            {
                parent = skeleton.Bones[i].Transform.parent.right;
                parent2 = skeleton.Bones[i].Transform.parent.up;
                dot = Vector3.Dot(my, parent);
                sing = Vector3.Dot(my, parent2) * -1;
                pingerAngle[i] = Mathf.Acos(dot) * Mathf.Rad2Deg * Mathf.Sign(sing);


            }
            else
            {
                pingerAngle[i] = PingerTipAngleRecursive(skeleton.Bones[i].Transform, ref pingerAngle);
            }
                gestureData.Angle[i] = pingerAngle[i];
        }


    }   
    //각각의 손가락 관절의 각도를 재귀로 탐색해 더하는 함수
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

        return gestureData.Distance[max - 1][min];
  



    }
    BoneData[] GetBonesDistance()
    {

        BoneId basePoint = BoneId.Hand_ThumbTip;
        int index = (int)BoneId.Hand_Start;
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
    #region 제스쳐 하드코딩

    void Grab()
    {
        float result = 0;
        bool isGrab = true;
        bool isGrabReady = true;
        for (int i = (int)BoneId.Hand_ThumbTip; i < (int)BoneId.Hand_End; i++)
        {
            float angle = gestureData.Angle[i];
            float rockAngle = rightRockReference.Angle[i];
            float paperAngle = rightPaperReference.Angle[i];

            angle -= rockAngle;
            paperAngle -= rockAngle;
            float ratio = angle / paperAngle;
            isGrab = isGrab & (ratio < 0.4f);
            isGrabReady = isGrabReady & (0.4f<= ratio && ratio < 0.8f);
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
    #region 외부에서 가져온 함수
        void Merge(BoneData[] list, ref BoneData[] buffer, int left, int mid, int right)
        {

            int i, j, k, l;
            i = left;
            j = mid + 1;
            k = left;

            /* 분할 정렬된 list의 합병 */
            while (i <= mid && j <= right)
            {
                if (list[i].distance <= list[j].distance)
                    buffer[k++] = list[i++];
                else
                    buffer[k++] = list[j++];
            }

            // 남아 있는 값들을 일괄 복사
            if (i > mid)
            {
                for (l = j; l <= right; l++)
                    buffer[k++] = list[l];
            }
            // 남아 있는 값들을 일괄 복사
            else
            {
                for (l = i; l <= mid; l++)
                    buffer[k++] = list[l];
            }

            // 배열 buffer[](임시 배열)의 리스트를 배열 list[]로 재복사
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
                mid = (left + right) / 2; // 중간 위치를 계산하여 리스트를 균등 분할 -분할(Divide)
                Merge_sort(list, ref buffer, left, mid); // 앞쪽 부분 리스트 정렬 -정복(Conquer)
                Merge_sort(list, ref buffer, mid + 1, right); // 뒤쪽 부분 리스트 정렬 -정복(Conquer)
                Merge(list, ref buffer, left, mid, right); // 정렬된 2개의 부분 배열을 합병하는 과정 -결합(Combine)
            }
        }
        #endregion
    #region 디버깅용 함수

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
   #region 레거시 함수
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


   //    //기준점에서 부터 접촉민감도 값 보다 작은 본들의 ID를 비트플래그로 저장.
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


   //    //저장된 비트 플래그를 기준점에 따라 정의된 비트플래그의 string으로 변환.
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

