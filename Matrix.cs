using System;
using System.Text;

public class Matrix
{
    private double[,] values;
    private int? hashCode;

    
    public Matrix(double[,] initialValues)
    {
        values = (double[,])initialValues.Clone();
    }

  
    public double this[int i, int j]
    {
        get { return values[i, j]; }
    }

    
    public int Rows => values.GetLength(0);
    public int Columns => values.GetLength(1);

   
    public static Matrix Zero(int rows, int columns)
    {
        return new Matrix(new double[rows, columns]);
    }

    public static Matrix Zero(int n)
    {
        return Zero(n, n);
    }

    public static Matrix Identity(int n)
        {
            var result = Zero(n);
            for (int i = 0; i < n; i++)
            {
                result.values[i, i] = 1.0;
            }
            return result;
        }

      
        public Matrix Transpose()
        {
            int rows = Rows;
            int columns = Columns;
            double[,] result = new double[columns, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[j, i] = values[i, j];
                }
            }
            return new Matrix(result);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int rows = Rows;
            int columns = Columns;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    sb.Append(values[i, j] + "\t");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }


        public override bool Equals(object obj)
        {
            if (obj is Matrix other)
            {
                if (Rows != other.Rows || Columns != other.Columns)
                    return false;

                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (values[i, j] != other.values[i, j])
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

   
        public override int GetHashCode()
        {
            if (hashCode.HasValue)
                return hashCode.Value;

            int hash = 17;
            foreach (double value in values)
            {
                hash = hash * 23 + value.GetHashCode();
            }
            hashCode = hash;
            return hash;
        }

  
        public static Matrix operator +(Matrix a, Matrix b)
        {
            return MatrixOperations.Add(a, b);
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            return MatrixOperations.Subtract(a, b);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return MatrixOperations.Multiply(a, b);
        }

        public static Matrix operator *(Matrix a, double scalar)
        {
            return MatrixOperations.Multiply(a, scalar);
        }

        public static Matrix operator *(double scalar, Matrix a)
        {
            return MatrixOperations.Multiply(a, scalar);
        }

        public static Matrix operator /(Matrix a, double scalar)
        {
            return MatrixOperations.Divide(a, scalar);
        }

        public static Matrix operator +(Matrix a)
        {
            return a;
        }

        public static Matrix operator -(Matrix a)
        {
            return MatrixOperations.Negate(a);
        }

        public static Matrix operator ~(Matrix a)
        {
            return a.Transpose();
        }
    }

    
    public static class MatrixOperations
    {
        public static Matrix Add(Matrix a, Matrix b)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            if (rows != b.Rows || columns != b.Columns)
            {
                throw new InvalidOperationException("Matrix dimensions must match for addition.");
            }

            double[,] resultValues = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultValues[i, j] = a[i, j] + b[i, j];
                }
            }
            return new Matrix(resultValues);
        }

        public static Matrix Subtract(Matrix a, Matrix b)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            if (rows != b.Rows || columns != b.Columns)
            {
                throw new InvalidOperationException("Matrix dimensions must match for subtraction.");
            }

            double[,] resultValues = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultValues[i, j] = a[i, j] - b[i, j];
                }
            }
            return new Matrix(resultValues);
        }

        public static Matrix Multiply(Matrix a, Matrix b)
        {
            int aRows = a.Rows;
            int aColumns = a.Columns;
            int bRows = b.Rows;
            int bColumns = b.Columns;
            if (aColumns != bRows)
            {
                throw new InvalidOperationException("Matrix dimensions are not valid for multiplication.");
            }

            double[,] resultValues = new double[aRows, bColumns];
            for (int i = 0; i < aRows; i++)
            {
                for (int j = 0; j < bColumns; j++)
                {
                    for (int k = 0; k < aColumns; k++)
                    {
                        resultValues[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return new Matrix(resultValues);
        }

        public static Matrix Multiply(Matrix a, double scalar)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            double[,] resultValues = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultValues[i, j] = a[i, j] * scalar;
                }
            }
            return new Matrix(resultValues);
        }

        public static Matrix Divide(Matrix a, double scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }

            int rows = a.Rows;
            int columns = a.Columns;
            double[,] resultValues = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultValues[i, j] = a[i, j] / scalar;
                }
            }
            return new Matrix(resultValues);
        }

        public static Matrix Negate(Matrix a)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            double[,] resultValues = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultValues[i, j] = -a[i, j];
                }
            }
            return new Matrix(resultValues);
        }
    }