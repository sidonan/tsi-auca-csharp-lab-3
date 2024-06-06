using System;
using System.IO;
using System.Threading.Tasks;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Write started");

        Matrix[] a = CreateRandomMatrices(50, 500, 100);
        Matrix[] b = CreateRandomMatrices(50, 100, 500);

        Task multiplicationTask1 = Task.Run(() => MultiplyMatrixArraysSequentially(a, b));
        Task multiplicationTask2 = Task.Run(() => MultiplyMatrixArraysSequentially(b, a));
        Task dotProductTask1 = Task.Run(() => CalculateDotProduct(a, b));
        Task dotProductTask2 = Task.Run(() => CalculateDotProduct(b, a));

        string resultsDir = "results";
        if (Directory.Exists(resultsDir))
        {
            Directory.Delete(resultsDir, true);
        }
        Directory.CreateDirectory(resultsDir);

        
        string binaryDir = "BinaryFormat";
        string textDir = "TextFormat";
        string jsonDir = "JsonFormat";

        if (Directory.Exists(binaryDir)) Directory.Delete(binaryDir, true);
        if (Directory.Exists(textDir)) Directory.Delete(textDir, true);
        if (Directory.Exists(jsonDir)) Directory.Delete(jsonDir, true);

        Directory.CreateDirectory(binaryDir);
        Directory.CreateDirectory(textDir);
        Directory.CreateDirectory(jsonDir);

        Console.WriteLine("Write async started");

       
        Task saveTextTask = WriteMatricesToDirectoryAsync(a, textDir, "a_", "csv", (m, s) => MatrixIO.WriteMatrixAsync(m, s, ";"));
        Task saveJsonTask = WriteMatricesToDirectoryAsync(b, jsonDir, "b_", "json", MatrixIO.WriteMatrixJsonAsync);

        await Task.WhenAll(saveTextTask, saveJsonTask);
        Console.WriteLine("Write async finished");

        Console.WriteLine("Binary a equals: " + CompareMatrixArrays(a, ReadMatricesFromDirectory(binaryDir, "a_", "bin", MatrixIO.ReadMatrixBinary)));

        
        Task readTextTask = Task.Run(async () =>
        {
            Matrix[] textReadMatrices = await ReadMatricesFromDirectoryAsync(textDir, "a_", "csv", (s) => MatrixIO.ReadMatrixAsync(s, ";"));
            Console.WriteLine($"Csv array equals: {CompareMatrixArrays(a, textReadMatrices)}");
        });

        Task readJsonTask = Task.Run(async () =>
        {
            Matrix[] jsonReadMatrices = await ReadMatricesFromDirectoryAsync(jsonDir, "b_", "json", MatrixIO.ReadMatrixJsonAsync);
            Console.WriteLine($"Json array equals: {CompareMatrixArrays(b, jsonReadMatrices)}");
        });

        await Task.WhenAny(readTextTask, readJsonTask);
        Console.WriteLine("Csv finished");
        Console.WriteLine("Json finished");

        await Task.WhenAll(multiplicationTask1, multiplicationTask2, dotProductTask1, dotProductTask2);
        Console.WriteLine("Write finished");
    }

    private static Matrix[] CreateRandomMatrices(int count, int rows, int columns)
    {
        Random rand = new Random();
        Matrix[] matrices = new Matrix[count];
        for (int i = 0; i < count; i++)
        {
            double[,] values = new double[rows, columns];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    values[r, c] = rand.NextDouble() * 20 - 10;
                }
            }
            matrices[i] = new Matrix(values);
        }
        return matrices;
    }

    private static void MultiplyMatrixArraysSequentially(Matrix[] first, Matrix[] second)
    {
        if (first.Length != second.Length) throw new InvalidOperationException("Differernt matrix lengths");
        for (int i = 0; i < first.Length; i++)
        {
            Matrix result = first[i] * second[i];
            
        }
    }

    private static void CalculateDotProduct(Matrix[] first, Matrix[] second)
    {
        if (first.Length != second.Length) throw new InvalidOperationException("Differernt matrix lengths");
        Matrix result = Matrix.Zero(first[0].Rows, first[0].Columns);
        for (int i = 0; i < first.Length; i++)
        {
            result = result + (first[i] * second[i]);
        }
        
    }

    private static async Task WriteMatricesToDirectoryAsync(Matrix[] matrices, string directory, string prefix, string extension, Func<Matrix, Stream, Task> writeFunc)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            string filePath = Path.Combine(directory, $"{prefix}{i}.{extension}");
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                await writeFunc(matrices[i], fs);
            }
            if (i % 10 == 0)
            {
                Console.WriteLine($"{prefix}{i}.{extension} write async finished");
            }
        }
    }

    private static void WriteMatricesToDirectory(Matrix[] matrices, string directory, string prefix, string extension, Action<Matrix, Stream> writeFunc)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            string filePath = Path.Combine(directory, $"{prefix}{i}.{extension}");
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                writeFunc(matrices[i], fs);
            }
            if (i % 10 == 0)
            {
                Console.WriteLine($"{prefix}{i}.{extension} write finished");
            }
        }
    }

    private static Matrix[] ReadMatricesFromDirectory(string directory, string prefix, string extension, Func<Stream, Matrix> readFunc)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        FileInfo[] files = dirInfo.GetFiles($"{prefix}*.{extension}");
        Matrix[] matrices = new Matrix[files.Length];

        foreach (FileInfo file in files)
        {
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
            {
                int index = int.Parse(file.Name.Replace(prefix, "").Replace($".{extension}", ""));
                matrices[index] = readFunc(fs);
            }
        }
        return matrices;
    }

    private static async Task<Matrix[]> ReadMatricesFromDirectoryAsync(string directory, string prefix, string extension, Func<Stream, Task<Matrix>> readFunc)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        FileInfo[] files = dirInfo.GetFiles($"{prefix}*.{extension}");
        Matrix[] matrices = new Matrix[files.Length];

        foreach (FileInfo file in files)
        {
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
            {
                int index = int.Parse(file.Name.Replace(prefix, "").Replace($".{extension}", ""));
                matrices[index] = await readFunc(fs);
            }
        }
        return matrices;
    }

    private static bool CompareMatrixArrays(Matrix[] first, Matrix[] second)
    {
        if (first.Length != second.Length) return false;
        for (int i = 0; i < first.Length; i++)
        {
            if (!first[i].Equals(second[i])) return false;
        }
        return true;
    }
}
