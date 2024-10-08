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

    public _Quaternion(Quaternion q){
        matrix = new float[,]{{q.w}, {q.x}, {q.y}, {q.z}};
    }


    public _Quaternion(float[,] m){

        int rowsA = m.GetLength(0);
        int colsA = m.GetLength(1);

        if (rowsA != 4 || colsA != 1) {
            throw new ArgumentException("Quaternion should has structure 4x1");
        }

        matrix = m;
    }

    public float this[int row, int column] {
        get { return matrix[row, column]; }
        set { matrix[row, column] = value; }
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

    public _Quaternion normalized{
        get { return Normalize(this); }
    }

    public static _Quaternion Normalize(_Quaternion q) {
        float norm = (float)Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
        if (norm == 0) 
            return new _Quaternion(0,0,0,0);
         else 
            return  q / norm;
    }

    public Quaternion Unity(){
        return new Quaternion(x, y, z, w);
    }

    public _Quaternion Inverse(){
        return new _Quaternion(w, -x, -y, -z);
    }

    public _Matrix toMatrix(){
        return new _Matrix(this.matrix);
    }

    
    public _Matrix T{
        get { return _Matrix.Transpose(matrix); }
    }

    public static _Quaternion operator +( _Quaternion q1, _Quaternion q2) {

        float[,] result = new float[4, 1];
        for (int i = 0; i < 4; i++){
                result[i, 0] = q1[i, 0] + q2[i, 0];
        }
        return new _Quaternion(result);
    }

        public static _Quaternion operator -( _Quaternion q1, _Quaternion q2) {

        float[,] result = new float[4, 1];
        for (int i = 0; i < 4; i++){
                result[i, 0] = q1[i, 0] - q2[i, 0];
        }
        return new _Quaternion(result);
    }



    public _Matrix toDirectionCosineMatrix(){
        float[,] result = new float[,]{
            {w*w + x*x - y*y - z*z,     2*(x*y + w*z),              2*(x*z - w*y)},
            {2*(x*y - w*z),             w*w - x*x + y*y - z*z,      2*(y*z + w*x)},
            { 2*(x*z + w*y),            2*(y*z - w*x),            w*w - x*x - y*y + z*z}
        };
        return new _Matrix(result);
    }

//------------------SCALE BY VALUE---------------------------------------
    private static _Quaternion MultiplyByScalar(_Quaternion q, float scalar) {
        float[,] result = new float[4, 1];
        for (int i = 0; i < 4; i++) {
                result[i, 0] = q[i, 0] * scalar;
        }
        return new _Quaternion(result);
    }

    public static _Quaternion operator *( _Quaternion q, float scalar){
        return MultiplyByScalar(q, scalar);
    }

    public static _Quaternion operator *(float scalar, _Quaternion q){
        return MultiplyByScalar(q, scalar);
    }

    private static _Quaternion DivideByScalar(_Quaternion q, float scalar) {
        float[,] result = new float[4, 1];
        for (int i = 0; i < 4; i++) {
            result[i, 0] = q[i, 0] / scalar;
            
        }
        return new _Quaternion(result);
    }

    public static _Quaternion operator /( _Quaternion q, float scalar){
        return DivideByScalar(q, scalar);
    }

    public static _Quaternion operator /( _Quaternion q, int scalar){
        return DivideByScalar(q, scalar);
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


public class _Matrix
{
    public float[,] matrix;

    public _Matrix(float[,] matrix){
        this.matrix = matrix;
    }   

    public _Matrix(_Matrix A){
        int rows = A.GetLength(0);
        int cols = A.GetLength(1);

        this.matrix = new float[rows, cols];

        for (int i = 0; i < rows; i++){
            for (int j = 0; j < cols; j++) {
                matrix[i, j] = A[i, j];
            }
        }
        
    }   

    public float this[int row, int column] {
        get { return matrix[row, column]; }
        set { matrix[row, column] = value; }
    }

    public int GetLength(int i){
        return (i < 2)? matrix.GetLength(i) : -1;
    }

    public int rows{
        get { return matrix.GetLength(0); }
    }

    public int cols{
        get { return matrix.GetLength(1); }
    }



//------------------TRANSPOSE---------------------------------------

    public _Matrix T{
        get { return Transpose(this.matrix); }
    }

    public static _Matrix Transpose(float[,] A){
        int rows = A.GetLength(0);
        int cols = A.GetLength(1);

        float[,] transposedMatrix = new float[cols, rows];

        for (int i = 0; i < rows; i++){
            for (int j = 0; j < cols; j++) {
                transposedMatrix[j, i] = A[i, j];
            }
        }
        return new _Matrix(transposedMatrix);
    }
    

//---------------------------------------------------------------------

//------------------IDENTITY---------------------------------------
    public static _Matrix Identity(int n){
        float[,] result = new float[n, n];
        for (int i = 0; i < n; i++){
            result[i, i] = 1;
        }
        return new _Matrix(result);
    }
//---------------------------------------------------------------------

   public static _Matrix Homogen(int I, int J, float value = 0f){
        float[,] result = new float[I, J];
        for (int i = 0; i < I; i++){
            for (int j = 0; j < J; j++) {
                result[i, j] = value;
            }
        }
        return new _Matrix(result);
    }


//------------------MULTIPLY---------------------------------------
    public static _Matrix operator *( _Matrix A, _Matrix B) {
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

        return new _Matrix(result);
    }

    
    public static _Quaternion operator *( _Matrix A, _Quaternion Q) {
        int rowsA = A.GetLength(0);
        int colsA = A.GetLength(1);
        int rowsB = Q.GetLength(0);

        if (colsA != rowsB) {
            throw new ArgumentException("The number of columns in the first matrix must be equal to the number of rows in the second matrix.");
        }

        float[,] result = new float[rowsA, 1];

        for (int i = 0; i < rowsA; i++){
            result[i, 0] = 0;
            for (int j = 0; j < colsA; j++){
                result[i, 0] += A[i, j] * Q[j, 0];
            }
            
        }

        return new _Quaternion(result);
    }

//---------------------------------------------------------------------

//------------------ADD------------------------------------------

    public static _Matrix operator +( _Matrix A, _Matrix B) {
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

        return new _Matrix(result);
    }

//---------------------------------------------------------------------

//------------------SUBTRACT------------------------------------------
    public static _Matrix operator - (_Matrix A, _Matrix B) {
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
                result[i, j] = A[i, j] - B[i, j];
            }
        }

        return new _Matrix(result);
    }



//------------------SCALE BY VALUE---------------------------------------
    private static _Matrix MultiplyByScalar(_Matrix matrix, float scalar) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = matrix[i, j] * scalar;
            }
        }
        return new _Matrix(result);
    }

    public static _Matrix operator *( _Matrix matrix, float scalar){
        return MultiplyByScalar(matrix, scalar);
    }

    public static _Matrix operator *(float scalar, _Matrix matrix){
        return MultiplyByScalar(matrix, scalar);
    }



    private static _Matrix DivideByScalar(_Matrix matrix, float scalar) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = matrix[i, j] / scalar;
            }
        }
        return new _Matrix(result);
    }

    public static _Matrix operator /( _Matrix matrix, float scalar){
        return DivideByScalar(matrix, scalar);
    }

    public static _Matrix operator /( _Matrix matrix, int scalar){
        return DivideByScalar(matrix, scalar);
    }

    public static _Matrix operator /( _Matrix m1,  _Matrix m2){
        int rowsA = m1.GetLength(0);
        int colsA = m1.GetLength(1);
        int rowsB = m2.GetLength(0);
        int colsB = m2.GetLength(1);

        if (rowsA != rowsB && colsA != colsB) {
            throw new ArgumentException("m1 & m2 should have same shape");
        }

        float[,] result = new float[rowsA, colsA];
        for (int i = 0; i < rowsA; i++) {
            for (int j = 0; j < colsA; j++) {
                result[i, j] = (m2[i, j]==0f)? 0f :  m1[i, j] / m2[i, j];
            }
        }
        

        return new _Matrix(result);
    }

//---------------------------------------------------------------------


    public _Matrix Inv{
        get { return Invert(this); }
    }

    public static _Matrix Invert(_Matrix matrix){

        
        if (matrix.GetLength(0) != matrix.GetLength(1)) {
            throw new ArgumentException("Matrix to invert should be NxN");
        }


        int n = matrix.GetLength(0);
        float[,] augmented = new float[n, n * 2];

        // Initialize augmented matrix with the input matrix and the identity matrix
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++){
                augmented[i, j] = matrix[i, j];
                augmented[i, j + n] = (i == j) ? 1 : 0;
            }
        }

        // Apply Gaussian elimination
        for (int i = 0; i < n; i++){
            int pivotRow = i;
            for (int j = i + 1; j < n; j++){
                if (Mathf.Abs(augmented[j, i]) > Mathf.Abs(augmented[pivotRow, i])){
                    pivotRow = j;
                }
            }

            if (pivotRow != i){
                for (int k = 0; k < 2 * n; k++){
                    float temp = augmented[i, k];
                    augmented[i, k] = augmented[pivotRow, k];
                    augmented[pivotRow, k] = temp;
                }
            }

            if (Mathf.Abs(augmented[i, i]) < 1e-10){
                //throw new ArgumentException($"{Mathf.Abs(augmented[i, i])} < 1e-10");
                augmented[i, i] = 1e-10f; //TODO
            }

            float pivot = augmented[i, i];
            for (int j = 0; j < 2 * n; j++){
                augmented[i, j] /= pivot;
            }

            for (int j = 0; j < n; j++){
                if (j != i){
                    float factor = augmented[j, i];
                    for (int k = 0; k < 2 * n; k++){
                        augmented[j, k] -= factor * augmented[i, k];
                    }
                }
            }
        }

        float[,] result = new float[n, n];
        for (int i = 0; i < n; i++){
            for (int j = 0; j < n; j++){
                result[i, j] = augmented[i, j + n];
            }
        }

        return new _Matrix(result);
    }


//---------------------------------------------------------------------
    public static _Matrix StackByRows(_Matrix A, _Matrix B){
        int colsA = A.GetLength(1);
        int colsB = B.GetLength(1);

        if(colsA != colsB ){
            throw new ArgumentException("The matrix should have the same number of columns to be able to stack them"); 
        }


        int rowsA = A.GetLength(0);
        int rowsB = B.GetLength(0);
        float[,] result = new float[rowsA + rowsB, colsA];

        for (int i = 0; i < rowsA; i++){
            for (int j = 0; j < colsA; j++){
                result[i, j] = A[i, j];
            }
        }

        for (int i = rowsA; i < rowsA + rowsB; i++){
            for (int j = 0; j < colsA; j++){
                result[i, j] = B[i - rowsA, j];
            }
        }

        return new _Matrix(result);

    }



//---------------------------------------------------------------------
    public _Matrix CholeskyDecomposition(){

        int rows = GetLength(0);
        int cols = GetLength(1);
        
        if (rows != cols) {
            throw new ArgumentException("The matrix should have rows == cols"); 
        }

        _Matrix result = Homogen(rows, cols, 0f);

        for (int j = 0; j < cols; j++){
            for (int i = j; i < rows; i++){
                float tempFloat = this[i, j];

                if (i == j){
                    for (int k = 0; k < j; k++){
                        tempFloat -= result[i, k] * result[i, k];
                    }
                    if (tempFloat < 1e-7){
                        tempFloat = 1e-7f; //TODO
                        //throw new ArgumentException("tempFloat < 1e-7"); 
                    }
                    if (Mathf.Abs(tempFloat) < 1e-7){
                        tempFloat = 0f;
                    }
                    result[i, i] = Mathf.Sqrt(tempFloat);
                }
                else {
                    for (int k = 0; k < j; k++){
                        tempFloat -= result[i, k] * result[j, k];
                    }
                    if (Math.Abs(result[j, j]) < 1e-7){
                        result[j, j] = 1e-7f;
                        //throw new ArgumentException("CholeskyDecomposition: Math.Abs(result[j, j]) < 1e-7");  TODO
                    }
                    result[i, j] = tempFloat / result[j, j];
                }
            }
        }
        return result;
    }

    

//------------------CONVERSIONS------------------------------------------
    
    public _Quaternion toQuaternion(){
        int rowsA = GetLength(0);
        int colsA = GetLength(1);

        if(rowsA != 4 || colsA != 1){
            throw new ArgumentException("Matrix should has structure 4x1 to convert it to Quaternion"); 
        }
        return new _Quaternion(this.matrix);

    }

//---------------------------------------------------------------------

//------------------OTHER------------------------------------------
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
