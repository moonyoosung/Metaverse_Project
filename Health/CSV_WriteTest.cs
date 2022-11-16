using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV_WriteTest : MonoBehaviour
{
    public string savePath = "Scripts/TestWrite.csv";

    public static CSV_WriteTest wTest;

    string[] names = new string[] { "index", "age", "size" };
    float[,] data = new float[,] { { 1001, 40, 183 },
                                   { 1002, 44, 180 },
                                   { 1003, 25, 177 },
                                   { 1004, 38, 174 } };

    private void Awake()
    {
        if(wTest == null)
        {
            wTest = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //CSV_Writer.Write(savePath, names, data);
    }

}
