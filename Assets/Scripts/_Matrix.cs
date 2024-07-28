using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class _Matrix2
{
    public static Matrix4x4 Add(Matrix4x4 m1, Matrix4x4 m2) {
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 4; i++){
            for (int j = 0; j < 4; j++){
                result[i, j] = m1[i, j] + m2[i, j];
            }
        }
        return result;
    }

    public static float[,] Add(float[,] m1, float[,] m2) {
        int rowsA = m1.GetLength(0);
        int colsA = m1.GetLength(1);
        int rowsB = m2.GetLength(0);
        int colsB = m2.GetLength(1);

        if ((rowsA != rowsB) && (colsA != colsB)) {
            throw new ArgumentException("Liczba kolumn w pierwszej macierzy musi być równa liczbie wierszy w drugiej macierzy.");
        }

        float[,] result = new float[rowsA, colsA];
        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsA; j++){
                result[i, j] = m1[i, j] + m2[i, j];
            }
        }
        return result;
    }

    public static float[,] Subtract(float[,] m1, float[,] m2) {
        int rowsA = m1.GetLength(0);
        int colsA = m1.GetLength(1);
        int rowsB = m2.GetLength(0);
        int colsB = m2.GetLength(1);

        if ((rowsA != rowsB) && (colsA != colsB)) {
            throw new ArgumentException("Liczba kolumn w pierwszej macierzy musi być równa liczbie wierszy w drugiej macierzy.");
        }

        float[,] result = new float[rowsA, colsA];
        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsA; j++){
                result[i, j] = m1[i, j] - m2[i, j];
            }
        }
        return result;
    }

    public static Matrix4x4 Scale(Matrix4x4 m, float b){
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 4; i++){
            for (int j = 0; j < 4; j++){
                result[i, j] = m[i, j] * b;
            }
        }
        return result;
    }

    public static float[,] Multiply(float[,] matrixA, float[,] matrixB){
        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int rowsB = matrixB.GetLength(0);
        int colsB = matrixB.GetLength(1);

        if (colsA != rowsB) {
            throw new ArgumentException("Liczba kolumn w pierwszej macierzy musi być równa liczbie wierszy w drugiej macierzy.");
        }

        float[,] result = new float[rowsA, colsB];

        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsB; j++){
                result[i, j] = 0;
                for (int k = 0; k < colsA; k++){
                    result[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }
        }

        return result;
    }

    
    public static float[,] Multiply(Matrix4x4 matrixA, float[,] matrixB){
        int rowsB = matrixB.GetLength(0);
        int colsB = matrixB.GetLength(1);
        int rowsA = 4;
        int colsA = 4;

        if (colsA != rowsB) {
            throw new ArgumentException("Liczba kolumn w pierwszej macierzy musi być równa liczbie wierszy w drugiej macierzy.");
        }

        float[,] result = new float[rowsA, colsB];

        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsB; j++){
                result[i, j] = 0;
                for (int k = 0; k < colsA; k++){
                    result[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }
        }

        return result;
    }


     public static float[,] Multiply(float[,] matrixA, Matrix4x4 matrixB){
        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int rowsB = 4;
        int colsB = 4;

        if (colsA != rowsB) {
            throw new ArgumentException("Liczba kolumn w pierwszej macierzy musi być równa liczbie wierszy w drugiej macierzy.");
        }

        float[,] result = new float[rowsA, colsB];

        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsB; j++){
                result[i, j] = 0;
                for (int k = 0; k < colsA; k++){
                    result[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }
        }

        return result;
    }



    public static float[,] Scale(float[,] matrix, float s){
        int rowsA = matrix.GetLength(0);
        int colsA = matrix.GetLength(1);

        float[,] result = new float[rowsA, colsA];

        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsA; j++){
                result[i, j] = matrix[i, j] * s;
            }
        }

        return result;
    }

    public static Matrix4x4 ToMatrix4x4(float[,] matrix){
        if (matrix.GetLength(0) != 4 || matrix.GetLength(1) != 4){
            throw new ArgumentException("Macierz musi mieć rozmiar 4x4.");
        }

        Matrix4x4 result = new Matrix4x4{
            m00 = matrix[0, 0], m01 = matrix[0, 1], m02 = matrix[0, 2], m03 = matrix[0, 3],
            m10 = matrix[1, 0], m11 = matrix[1, 1], m12 = matrix[1, 2], m13 = matrix[1, 3],
            m20 = matrix[2, 0], m21 = matrix[2, 1], m22 = matrix[2, 2], m23 = matrix[2, 3],
            m30 = matrix[3, 0], m31 = matrix[3, 1], m32 = matrix[3, 2], m33 = matrix[3, 3]
        };

        return result;
    }






    public static float[,] Transpose(float[,] matrix){
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        float[,] transposedMatrix = new float[cols, rows];

        for (int i = 0; i < rows; i++){
            for (int j = 0; j < cols; j++) {
                transposedMatrix[j, i] = matrix[i, j];
            }
        }
        return transposedMatrix;
    }

    public static float[,] InvertMatrix(float[,] matrix)
    {
        int n = matrix.GetLength(0);
        float[,] result = new float[n, n];
        float[,] identity = new float[n, n];

        for (int i = 0; i < n; i++)
        {
            identity[i, i] = 1;
        }

        for (int i = 0; i < n; i++)
        {
            float diagElement = matrix[i, i];
            if (diagElement == 0)
            {
                throw new ArgumentException("Matrix is not invertible");
            }
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] /= diagElement;
                identity[i, j] /= diagElement;
            }
            for (int k = 0; k < n; k++)
            {
                if (k != i)
                {
                    float factor = matrix[k, i];
                    for (int j = 0; j < n; j++)
                    {
                        matrix[k, j] -= factor * matrix[i, j];
                        identity[k, j] -= factor * identity[i, j];
                    }
                }
            }
        }

        return identity;
    }



    public static float[,] Identity(){
        return new float[,]{
            {1, 0, 0, 0},
            {0, 1, 0, 0},
            {0, 0, 1, 0},
            {0, 0, 0, 1}
        };
    }


    public static string PrintMatrix(float[,] matrix){
        int rows = matrix.GetLength(0); // Liczba wierszy
        int cols = matrix.GetLength(1); // Liczba kolumn
        string result = "";
        for (int i = 0; i < rows; i++){
            for (int j = 0; j < cols; j++) {
                result +=$"{matrix[i, j],20:F8}"; 
            }
            result +="\n";
        }
        return result;
    }

    public static string PrintMatrix(Matrix4x4 matrix){

        // Tworzymy string z macierzy
        string matrixString = "";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                matrixString += $"{matrix[i, j],16:F8}"; 
            }
            matrixString += "\n";
        }
        
        return matrixString;
    }


}
