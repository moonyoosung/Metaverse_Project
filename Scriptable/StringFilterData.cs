using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "FilterData", menuName = "MindPlus/FilterData", order = 0)]
public class StringFilterData : ScriptableObject
{
    public string[] filterDatas;
    //#if Bul
    public void SetFilterData()
    {
        //텍스트 리소스에서 가져오기 -> 추후 서버에서 내려받
        TextAsset temp = Resources.Load<TextAsset>("TextFilter");

        //사이즈 초기화
        Array.Resize(ref filterDatas, 0);

        //문자 배열에 넣기
        filterDatas = temp.text.Replace("\n", String.Empty).Split(',');
    }

    void SaveFile()
    {
        string directory = "/TextTest";
        string fileName = "/FilterData.txt";

        //폴더 생성
        if (!Directory.Exists(Application.persistentDataPath + directory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + directory);
        }

        //텍스트 파일 미 존재 시 생성
        //if (!File.Exists(Application.persistentDataPath + directory + fileName))
        {
            //현재 스크립터블 데이터 참조하여 파일 생성
            //추후 서버에서 파일 내려받아야함
            StreamWriter sw;
            sw = File.CreateText(Application.persistentDataPath + directory + fileName);
            for (int i = 0; i < filterDatas.Length; i++)
            {
                string str = filterDatas[i].Trim();
                if (i != filterDatas.Length - 1)
                    sw.Write(str + ",");
                else
                    sw.Write(str);
            }
            sw.Close();
        }
        File.ReadAllText(Application.persistentDataPath + directory + fileName).Split(',');
    }

    //public string TextFilter(string text)
    //{
    //    //text = Replace(text);
    //    //for (int i = 0; i < filterDatas.Length; i++)
    //    //{
    //    //    text = Regex.Replace(text, filterDatas[i], "*", RegexOptions.IgnoreCase);
    //    //}
    //    return text;
    //}

    public string TextFilter(string content)
    {
        string sTemp = content.Replace(" ", "");    //띄어쓰기 제거
        string cTemp = "";  //변경 할 문자
        string result = ""; //반환 할 문자
        string content_ok = "";

        string[] arrContent = content.Split(' ');   //문자열 띄어쓰기 기준으로 배열로 반환

        for (int i = 0; i < filterDatas.Length; i++)
        {
            string fTemp = filterDatas[i];

            if (sTemp.IndexOf(fTemp) != -1)
            {
                cTemp = "";
            }

            for (int j = 0; j < fTemp.Length; j++)
            {
                cTemp += "*";
            }

            //대소문자 가리지 않고 변경 후 문자열 담기
            sTemp = Regex.Replace(sTemp, fTemp, cTemp, RegexOptions.IgnoreCase);
        }

        //문자열 재조립
        for (int i = 0; i < arrContent.Length; i++)
        {
            int first = content_ok.Length;
            int last = arrContent[i].Length;
            content_ok = content_ok + sTemp.Substring(first, last);
            result = result + " " + sTemp.Substring(first, last);
        }
        return result;
    }
}
