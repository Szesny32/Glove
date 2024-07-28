using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        //MultiplicationTest();
        //Debug.Log(_Matrix2.Identity(6).Print());
        //ScalingTest();
        //Debug.Log(new _Quaternion(1, 2, 3, 4).Print());
        //Debug.Log(new _Quaternion(1, 2, 3, 4).z);
        //TransposeTest();
        //AddTest();
        
        
    }


    public void MultiplicationTest(){
        _Matrix2 A = new _Matrix2(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});

        _Matrix2 B = new _Matrix2(new float[,]{
            {17, 18, 19, 20},
            {21, 22, 23, 24},
            {25, 26, 27, 28},
            {29, 30, 31, 32}});

        _Matrix2 C = A*B;
        Debug.Log(C.Print());
    }


    public void ScalingTest(){
        _Matrix2 A = new _Matrix2(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});


        _Matrix2 B = 2f * A;
        Debug.Log(B.Print());

         _Matrix2 C =  A * 3f;
        Debug.Log(C.Print());
    }

    
    public void TransposeTest(){
        _Matrix2 A = new _Matrix2(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});
        Debug.Log(A.Print());
        Debug.Log(A.T.Print());
        Debug.Log(A.T.T.Print());
    }



    public void AddTest(){
        _Matrix2 A = new _Matrix2(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});

        _Matrix2 B = new _Matrix2(new float[,]{
            {17, 18, 19, 20},
            {21, 22, 23, 24},
            {25, 26, 27, 28},
            {29, 30, 31, 32}});

        _Matrix2 C = A+B;
        Debug.Log(C.Print());
    }
}
