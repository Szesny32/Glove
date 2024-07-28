using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        //MultiplicationTest();
        //Debug.Log(_Matrix.Identity(6).Print());
        //ScalingTest();
        //Debug.Log(new _Quaternion(1, 2, 3, 4).Print());
        //Debug.Log(new _Quaternion(1, 2, 3, 4).z);
        //TransposeTest();s
        //AddTest();
        //SubtractTest();        
        //MultiplicationTest2();
        //Debug.Log(new _Quaternion(1, 2, 3, 4).T.Print());
        //AddTest2();
        InverseTest();

    }


    public void MultiplicationTest(){
        _Matrix A = new _Matrix(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});

        _Matrix B = new _Matrix(new float[,]{
            {17, 18, 19, 20},
            {21, 22, 23, 24},
            {25, 26, 27, 28},
            {29, 30, 31, 32}});

        _Matrix C = A*B;
        Debug.Log(C.Print());
    }


    public void ScalingTest(){
        _Matrix A = new _Matrix(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});


        _Matrix B = 2f * A;
        Debug.Log(B.Print());

         _Matrix C =  A * 3f;
        Debug.Log(C.Print());
    }

    
    public void TransposeTest(){
        _Matrix A = new _Matrix(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});
        Debug.Log(A.Print());
        Debug.Log(A.T.Print());
        Debug.Log(A.T.T.Print());
    }



    public void AddTest(){
        _Matrix A = new _Matrix(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});

        _Matrix B = new _Matrix(new float[,]{
            {17, 18, 19, 20},
            {21, 22, 23, 24},
            {25, 26, 27, 28},
            {29, 30, 31, 32}});

        _Matrix C = A+B;
        Debug.Log(C.Print());
    }

        public void SubtractTest(){
        _Matrix A = new _Matrix(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});

        _Matrix C = A-_Matrix.Identity(4);
        Debug.Log(C.Print());
    }

  public void MultiplicationTest2(){
        _Matrix A = new _Matrix(new float[,]{
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12},
            {13, 14, 15, 16}});

        _Quaternion Q1 = new _Quaternion(new float[,]{
            {10},
            {20},
            {30},
            {40}
        });


        _Quaternion Q2 = A*Q1;
        Debug.Log(Q2.Print());
    }


    public void AddTest2(){
          _Quaternion Q1 = new _Quaternion(new float[,]{
            {10},
            {20},
            {30},
            {40}
        });

        _Quaternion Q2 = new _Quaternion(new float[,]{
            {1},
            {2},
            {3},
            {4}
        });
        _Quaternion C = Q1+Q2;
        Debug.Log(C.Print());
    }

     public void InverseTest(){
        _Matrix A = new _Matrix(new float[,]{
            {1, 2, 3, 4}, 
            {0, 1, 4, 5},
            {2, 3, 1, 2}, 
            {1, 0, 2, 3}});
        Debug.Log(A.Inv.Print());


        _Matrix B = new _Matrix(new float[,]{   
            {4, 0, 0, 0, 2, 0},   
            {0, 1, 0, 0, 0, 0},   
            {0, 0, 4, 0, 0, 0},   
            {0, 0, 0, 5, 0, 0},   
            {2, 0, 0, 0, 4, 0},   
            {0, 0, 0, 0, 0, 6} });
        Debug.Log(B.Inv.Print());
    }



}
