using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class _Quaternion 
{
    public float[,] matrix;

    public _Quaternion(float w, float x, float y, float z){
        matrix = new float[,]{{w}, {x}, {y}, {z}};
    }

    public float w{
        get { return matrix[0,0]; }
        set { matrix[0,0] = value; }
    }

    public float x{
        get { return matrix[1,0]; }
        set { matrix[1,0] = value; }
    }

    public float y{
        get { return matrix[2,0]; }
        set { matrix[2,0] = value; }
    }

    public float z{
        get { return matrix[3,0]; }
        set { matrix[3,0] = value; }
    }

    public int GetLength(int i){
        return (i < 2)? matrix.GetLength(i) : 0;
    }

    public string Print(){
        int rowsA = GetLength(0);
        int colsA = GetLength(1);

        string matrixString = "";
        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsA; j++){
                matrixString += $"{matrix[i, j],32:F8}"; 
            }
            matrixString += "\n";
        }
        return matrixString;
    }

}


public class _Vector
{

}


public class _Matrix2
{
    public float[,] matrix;

    public _Matrix2(float[,] matrix){
        this.matrix = matrix;
    }   

    public float this[int row, int column] {
        get { return matrix[row, column]; }
        set { matrix[row, column] = value; }
    }

    public int GetLength(int i){
        return (i < 2)? matrix.GetLength(i) : 0;
    }



//------------------TRANSPOSE---------------------------------------

    public _Matrix2 T{
        get { return Transpose(this); }
    }

    public static _Matrix2 Transpose(_Matrix2 A){
        int rows = A.GetLength(0);
        int cols = A.GetLength(1);

        float[,] transposedMatrix = new float[cols, rows];

        for (int i = 0; i < rows; i++){
            for (int j = 0; j < cols; j++) {
                transposedMatrix[j, i] = A[i, j];
            }
        }
        return new _Matrix2(transposedMatrix);
    }

//---------------------------------------------------------------------


    public static _Matrix2 Identity(int n){
        float[,] result = new float[n, n];
        for (int i = 0; i < n; i++){
            result[i, i] = 1;
        }
        return new _Matrix2(result);
    }


    public static _Matrix2 operator *( _Matrix2 A, _Matrix2 B) {
        int rowsA = A.GetLength(0);
        int colsA = A.GetLength(1);
        int rowsB = B.GetLength(0);
        int colsB = B.GetLength(1);

        if (colsA != rowsB) {
            throw new ArgumentException("The number of columns in the first matrix must be equal to the number of rows in the second matrix.");
        }

        float[,] result = new float[rowsA, colsB];

        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsB; j++){
                result[i, j] = 0;
                for (int k = 0; k < colsA; k++){
                    result[i, j] += A[i, k] * B[k, j];
                }
            }
        }

        return new _Matrix2(result);
    }

    public static _Matrix2 operator +( _Matrix2 A, _Matrix2 B) {
        int rowsA = A.GetLength(0);
        int colsA = A.GetLength(1);
        int rowsB = B.GetLength(0);
        int colsB = B.GetLength(1);

        if (colsA != colsB || rowsA != rowsB) {
            throw new ArgumentException("The number of rows/columns in the first matrix must be equal in the second matrix.");
        }

        float[,] result = new float[rowsA, colsB];

        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsA; j++){
                result[i, j] = A[i, j] + B[i, j];
            }
        }

        return new _Matrix2(result);
    }



//------------------SCALE BY VALUE---------------------------------------
    private static _Matrix2 MultiplyByScalar(_Matrix2 matrix, float scalar) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = matrix[i, j] * scalar;
            }
        }
        return new _Matrix2(result);
    }

    public static _Matrix2 operator *( _Matrix2 matrix, float scalar){
        return MultiplyByScalar(matrix, scalar);
    }

    public static _Matrix2 operator *(float scalar, _Matrix2 matrix){
        return MultiplyByScalar(matrix, scalar);
    }
//---------------------------------------------------------------------

    public string Print(){
        int rowsA = GetLength(0);
        int colsA = GetLength(1);

        string matrixString = "";
        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsA; j++){
                matrixString += $"{matrix[i, j],32:F8}"; 
            }
            matrixString += "\n";
        }
        return matrixString;
    }

}
