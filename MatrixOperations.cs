using System;
using System.Threading.Tasks;
public class MatrixOperationException : Exception
{
    public MatrixOperationException(string message) : base(message) { }
}

public static class MatrixOperations
{

    public static Matrix Transpose(Matrix matrix)
    {
        int rows = matrix.Rows;
        int columns = matrix.Columns;
        double[,] transposedValues = new double[columns, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                transposedValues[j, i] = matrix[i, j];
            }
        }

        return new Matrix(transposedValues);
    }

    public static Matrix Multiply(Matrix matrix, double scalar)
    {
        int rows = matrix.Rows;
        int columns = matrix.Columns;
        double[,] resultValues = new double[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                resultValues[i, j] = matrix[i, j] * scalar;
            }
        }

        return new Matrix(resultValues);
    }

    public static Matrix Add(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
        {
            throw new MatrixOperationException("Matrix dimensions must match for addition.");
        }

        int rows = a.Rows;
        int columns = a.Columns;
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

    // Метод для вычитания матриц
    public static Matrix Subtract(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
        {
            throw new MatrixOperationException("Matrix dimensions must match for subtraction.");
        }

        int rows = a.Rows;
        int columns = a.Columns;
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
        if (a.Columns != b.Rows)
        {
            throw new MatrixOperationException("Matrix dimensions are not valid for multiplication.");
        }

        int aRows = a.Rows;
        int aColumns = a.Columns;
        int bColumns = b.Columns;
        double[,] resultValues = new double[aRows, bColumns];
        Matrix bTransposed = Transpose(b);

        Parallel.For(0, aRows, i =>
        {
            for (int j = 0; j < bColumns; j++)
            {
                double sum = 0;
                for (int k = 0; k < aColumns; k++)
                {
                    sum += a[i, k] * bTransposed[j, k];
                }
                resultValues[i, j] = sum;
            }
        });

        return new Matrix(resultValues);
    }


    public static (Matrix inverse, double determinant) Inverse(Matrix matrix)
    {
        int n = matrix.Rows;
        if (n != matrix.Columns)
        {
            throw new MatrixOperationException("Matrix must be square to calculate its inverse.");
        }

        double[,] a = (double[,])matrix.Clone().GetValues();
        double[,] identity = Matrix.Identity(n).GetValues();
        double det = 1;

        for (int i = 0; i < n; i++)
        {
            if (a[i, i] == 0)
            {
                bool found = false;
                for (int j = i + 1; j < n; j++)
                {
                    if (a[j, i] != 0)
                    {
                        SwapRows(a, i, j);
                        SwapRows(identity, i, j);
                        det = -det;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new MatrixOperationException("Matrix is singular and cannot be inverted.");
                }
            }

            double diagVal = a[i, i];
            det *= diagVal;

            for (int j = 0; j < n; j++)
            {
                a[i, j] /= diagVal;
                identity[i, j] /= diagVal;
            }

            for (int j = 0; j < n; j++)
            {
                if (j != i)
                {
                    double factor = a[j, i];
                    for (int k = 0; k < n; k++)
                    {
                        a[j, k] -= factor * a[i, k];
                        identity[j, k] -= factor * identity[i, k];
                    }
                }
            }
        }

        return (new Matrix(identity), det);
    }


    private static void SwapRows(double[,] matrix, int row1, int row2)
    {
        int columns = matrix.GetLength(1);
        for (int i = 0; i < columns; i++)
        {
            double temp = matrix[row1, i];
            matrix[row1, i] = matrix[row2, i];
            matrix[row2, i] = temp;
        }
    }
}

public static class MatrixExtensions
{
    public static double[,] GetValues(this Matrix matrix)
    {
        return (double[,])matrix.Clone().GetType().GetProperty("values", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(matrix);
    }
}