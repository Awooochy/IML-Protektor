using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Net;

class Program
{
    static void Main()
    {
        // Check if the SetupCheck file exists and skip setup if it does
        if (!File.Exists("SetupCheck"))
        {
            // Download the zip folder and extract its contents
            DownloadAndExtractZip();

            // Create the SetupCheck file to indicate that setup is complete
            File.Create("SetupCheck").Close();
        }

        // Check if Input.txt exists and create it if not
        if (!File.Exists("Input.txt"))
        {
            File.Create("Input.txt").Close(); // Create an empty file
            Console.WriteLine("Input.txt created. Please add content to it and run the program again.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return; //kill program
        }

        // Check if Parameters.txt exists and create it if not
        if (!File.Exists("Parameters.txt"))
        {
            File.Create("Parameters.txt").Close(); // Create an empty file
            Console.WriteLine("Parameters.txt created. Please add instructions to it and run the program again.");
            Console.WriteLine("Also theres a file with the instructions of use if you wanna check it.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return; //kill program
        }

        // Read input content from Input.txt
        string inputContent = File.ReadAllText("Input.txt");

        // Read instructions from Parameters.txt
        string instructions = File.ReadAllText("Parameters.txt");

        // Process the content based on the instructions
        string processedContent = ProcessContent(inputContent, instructions);

        // Generate the output for Processed.txt
        string output = GenerateOutput(inputContent, instructions, processedContent);

        // Output the processed content to Processed.txt
        File.WriteAllText("Processed.txt", output);

        Console.WriteLine("Processing complete. Please check Processed.txt for the result.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static string ProcessContent(string content, string instructions)
    {
        string processedContent = content;
        string[] instructionList = instructions.Split('-');

        for (int i = 0; i < instructionList.Length; i++)
        {
            string instruction = instructionList[i];
            char operation = instruction[0];
            string operationType = instruction.Substring(1);

            if (operation == 'P')
            {
                Console.WriteLine("Enter the new content:");
                string newContent = Console.ReadLine();
                processedContent = ApplyProtectionLayer(processedContent, newContent);
            }
            else
            {
                switch (operation)
                {
                    case 'E':
                        processedContent = ApplyEncoding(processedContent, operationType, "Encoded");
                        break;
                    case 'D':
                        processedContent = ApplyDecoding(processedContent, operationType, "Decoded");
                        break;
                    default:
                        Console.WriteLine($"Invalid operation: {operation}");
                        break;
                }
            }
        }

        return processedContent;
    }

    static string ApplyProtectionLayer(string content, string newContent)
    {
        Console.WriteLine("---------------------------------------------");
        Console.WriteLine("[Protection Layer] Operation");
        Console.WriteLine("Result:");
        Console.WriteLine("---------------------------------------------");
        Console.WriteLine(newContent);
        Console.WriteLine();

        return newContent;
    }

    static string ApplyEncoding(string content, string encodingType, string operationName)
    {
        string result = content;
        switch (encodingType)
        {
            case "H":
                result = EncodeToHex(content);
                break;
            case "64":
                result = EncodeToBase64(content);
                break;
            case "Z":
                result = CompressWithGzip(content);
                break;
            case "DF":
                result = CompressWithDeflate(content);
                break;
            case "RL":
                result = RunLengthEncode(content);
                break;
            default:
                Console.WriteLine($"Invalid encoding type: {encodingType}");
                break;
        }

        Console.WriteLine("---------------------------------------------");
        Console.WriteLine($"[{operationName}] Operation {encodingType}");
        Console.WriteLine("Result:");
        Console.WriteLine("---------------------------------------------");
        Console.WriteLine(result);
        Console.WriteLine();

        return result;
    }

    static string ApplyDecoding(string content, string decodingType, string operationName)
    {
        string result = content;
        switch (decodingType)
        {
            case "H":
                result = DecodeFromHex(content);
                break;
            case "64":
                result = DecodeFromBase64(content);
                break;
            case "Z":
                result = DecompressWithGzip(content);
                break;
            case "DF":
                result = DecompressWithDeflate(content);
                break;
            case "RL":
                result = RunLengthDecode(content);
                break;
            default:
                Console.WriteLine($"Invalid decoding type: {decodingType}");
                break;
        }

        Console.WriteLine("---------------------------------------------");
        Console.WriteLine($"[{operationName}] Operation {decodingType}");
        Console.WriteLine("Result:");
        Console.WriteLine("---------------------------------------------");
        Console.WriteLine(result);
        Console.WriteLine();

        return result;
    }

    static string RunLengthEncode(string content)
    {
        StringBuilder encodedBuilder = new StringBuilder();
        int count = 1;
        for (int i = 1; i < content.Length; i++)
        {
            if (content[i] == content[i - 1])
            {
                count++;
            }
            else
            {
                encodedBuilder.Append(count);
                encodedBuilder.Append(content[i - 1]);
                count = 1;
            }
        }
        // Append the last character and count
        encodedBuilder.Append(count);
        encodedBuilder.Append(content[content.Length - 1]);

        return encodedBuilder.ToString();
    }
    static string RunLengthDecode(string encodedContent)
    {
        StringBuilder decodedBuilder = new StringBuilder();
        for (int i = 0; i < encodedContent.Length; i += 2)
        {
            int count = int.Parse(encodedContent[i].ToString());
            char character = encodedContent[i + 1];
            decodedBuilder.Append(new string(character, count));
        }

        return decodedBuilder.ToString();
    }

    static string CompressWithDeflate(string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
            {
                deflateStream.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    static string DecompressWithDeflate(string compressedContent)
    {
        byte[] bytes = Convert.FromBase64String(compressedContent);
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
            {
                using (StreamReader streamReader = new StreamReader(deflateStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }

    static string EncodeToHex(string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        return BitConverter.ToString(bytes).Replace("-", string.Empty);
    }

    static string DecodeFromHex(string hex)
    {
        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return Encoding.UTF8.GetString(bytes);
    }

    static string EncodeToBase64(string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        return Convert.ToBase64String(bytes);
    }

    static string DecodeFromBase64(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }

    static string CompressWithGzip(string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    static string DecompressWithGzip(string compressedContent)
    {
        byte[] bytes = Convert.FromBase64String(compressedContent);
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                using (StreamReader streamReader = new StreamReader(gzipStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
    static void DownloadAndExtractZip()
    {
        string zipUrl = "https://github.com/Awooochy/Custom-E64-Plugin/releases/download/1.0/IMLSetup.zip";
        string downloadPath = "IMLSetup.zip";

        using (WebClient webClient = new WebClient())
        {
            webClient.DownloadFile(zipUrl, downloadPath);
        }

        // Extract the contents of the zip file to the current directory
        ZipFile.ExtractToDirectory(downloadPath, Directory.GetCurrentDirectory());

        // Delete the downloaded zip file if needed
        File.Delete(downloadPath);
    }

    static string GenerateOutput(string originalContent, string instructions, string processedContent)
    {
        StringBuilder outputBuilder = new StringBuilder();
        outputBuilder.AppendLine("Original Content: " + originalContent);
        outputBuilder.AppendLine("Instructions: " + instructions);
        outputBuilder.AppendLine("Inverted Instructions: " + InvertInstructions(instructions));
        outputBuilder.AppendLine("Final Result: " + processedContent);
        return outputBuilder.ToString();
    }

    static string InvertInstructions(string instructions)
    {
        StringBuilder invertedBuilder = new StringBuilder();
        string[] instructionList = instructions.Split('-');

        for (int i = instructionList.Length - 1; i >= 0; i--)
        {
            string instruction = instructionList[i];
            char operation = instruction[0];
            string operationType = instruction.Substring(1);

            if (operation == 'P')
            {
                invertedBuilder.Append("P");
                // Inverting protection layer operation doesn't require additional information
            }
            else
            {
                invertedBuilder.Append(operation == 'E' ? 'D' : 'E'); // Invert encoding/decoding operation
                invertedBuilder.Append(operationType);
            }

            if (i > 0)
            {
                invertedBuilder.Append('-');
            }
        }

        return invertedBuilder.ToString();
    }
}
