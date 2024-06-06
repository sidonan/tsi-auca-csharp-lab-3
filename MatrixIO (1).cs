using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class MatrixIO
{
    public static async Task WriteMatrixAsync(Matrix matrix, Stream stream, string sep = "\t")
    {
        using (StreamWriter writer = new StreamWriter(stream))
        {
            await writer.WriteLineAsync($"{matrix.Rows} {matrix.Columns}");
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    if (j > 0) await writer.WriteAsync(sep);
                    await writer.WriteAsync(matrix[i, j].ToString());
                }
                await writer.WriteLineAsync();
            }
        }
    }

    
    public static async Task<Matrix> ReadMatrixAsync(Stream stream, string sep = "\t")
    {
        using (StreamReader reader = new StreamReader(stream))
        {
            string[] dimensions = (await reader.ReadLineAsync()).Split();
            int rows = int.Parse(dimensions[0]);
            int columns = int.Parse(dimensions[1]);

            double[,] values = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                string[] line = (await reader.ReadLineAsync()).Split(sep);
                for (int j = 0; j < columns; j++)
                {
                    values[i, j] = double.Parse(line[j]);
                }
            }

            return new Matrix(values);
        }
    }


    public static void WriteMatrixBinary(Matrix matrix, Stream stream)
    {
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(matrix.Rows);
            writer.Write(matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    writer.Write(matrix[i, j]);
                }
            }
        }
    }


    public static Matrix ReadMatrixBinary(Stream stream)
    {
        using (BinaryReader reader = new BinaryReader(stream))
        {
            int rows = reader.ReadInt32();
            int columns = reader.ReadInt32();
            double[,] values = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    values[i, j] = reader.ReadDouble();
                }
            }
            return new Matrix(values);
        }
    }

 
    public static async Task WriteMatrixJsonAsync(Matrix matrix, Stream stream)
    {
        double[][] values = new double[matrix.Rows][];
        for (int i = 0; i < matrix.Rows; i++)
        {
            values[i] = new double[matrix.Columns];
            for (int j = 0; j < matrix.Columns; j++)
            {
                values[i][j] = matrix[i, j];
            }
        }

        await JsonSerializer.SerializeAsync(stream, values);
    }

    
    public static async Task<Matrix> ReadMatrixJsonAsync(Stream stream)
    {
        double[][] values = await JsonSerializer.DeserializeAsync<double[][]>(stream);
        int rows = values.Length;
        int columns = values[0].Length;
        double[,] result = new double[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                result[i, j] = values[i][j];
            }
        }

        return new Matrix(result);
    }


    public static void WriteMatrixToFile(string directory, string fileName, Matrix matrix, Action<Matrix, Stream> writeMethod)
    {
        string filePath = Path.Combine(directory, fileName);
        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            writeMethod(matrix, stream);
        }
    }

   
    public static async Task WriteMatrixToFileAsync(string directory, string fileName, Matrix matrix, Func<Matrix, Stream, Task> writeMethod)
    {
        string filePath = Path.Combine(directory, fileName);
        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await writeMethod(matrix, stream);
        }
    }

    
    public static Matrix ReadMatrixFromFile(string filePath, Func<Stream, Matrix> readMethod)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            return readMethod(stream);
        }
    }

 
    public static async Task<Matrix> ReadMatrixFromFileAsync(string filePath, Func<Stream, Task<Matrix>> readMethod)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            return await readMethod(stream);
        }
    }
}
