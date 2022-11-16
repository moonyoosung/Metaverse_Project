using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CSV_Writer
{
    /// <summary>
    /// CSV 파일로 저장하기
    /// </summary>
    /// <param name="savePath">저장 경로</param>
    /// <param name="titles">데이터 이름 배열</param>
    /// <param name="values">데이터 값 2차원 배열</param>
    public static void Write(string savePath, string[] titles, string[,] values)
    {
        // 1. 스트림 생성하기
        FileStream fs = new FileStream(Application.dataPath + "/" + savePath, FileMode.OpenOrCreate, FileAccess.Write);

        // 2. 파일 쓰기 준비
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
        //StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("euc-kr"));

        // 3. 제목 배열을 파일에 쓰기
        string ti = "";
        for (int i = 0; i < titles.Length; i++)
        {
            ti += titles[i];

            if (i < titles.Length - 1)
            {
                ti += ",";
            }
        }
        sw.WriteLine(ti);
        
        // 4. 값 행렬을 파일에 쓰기
        for(int i = 0; i < values.GetLength(0); i++)
        {
            string vals = "";

            for(int j = 0; j < values.GetLength(1); j++)
            {
                vals += values[i, j].ToString();

                if (j < values.GetLength(1) - 1)
                {
                    vals += ",";
                }
            }
            sw.WriteLine(vals);
        }

        // 5. 스트림 닫기
        sw.Flush();
        sw.Close();
        fs.Close();
        
        Debug.Log("파일 저장이 완료되었습니다!");
    }
}
