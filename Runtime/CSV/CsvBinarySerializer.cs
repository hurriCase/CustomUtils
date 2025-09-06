﻿using System.IO;
using CustomUtils.Runtime.CSV.Base;
using Cysharp.Text;
using JetBrains.Annotations;
using MemoryPack;
using UnityEngine;

namespace CustomUtils.Runtime.CSV
{
    /// <summary>
    /// Provides functionality to convert CSV files to binary format using MemoryPack serialization.
    /// </summary>
    [UsedImplicitly]
    public sealed class CsvBinarySerializer
    {
        private readonly CsvParser _csvParser = new();

        /// <summary>
        /// Converts a CSV file to binary format by parsing it with the specified converter and serializing the result.
        /// </summary>
        /// <typeparam name="T">The type of objects to convert CSV rows into.</typeparam>
        /// <param name="csvConverter">The converter that maps CSV rows to objects of type T.</param>
        /// <param name="csvFilePath">The file path to the CSV file to convert.</param>
        /// <param name="binaryOutputPath">The file path where the binary output will be saved.</param>
        [UsedImplicitly]
        public void ConvertCSVToBinary<T>(ICsvConverter<T> csvConverter, string csvFilePath, string binaryOutputPath)
            where T : new()
        {
            var csvContent = File.ReadAllText(csvFilePath);
            var csvTable = _csvParser.Parse(csvContent);
            var objects = csvConverter.ConvertToObjects(csvTable);

            var binaryData = MemoryPackSerializer.Serialize(objects);
            File.WriteAllBytes(binaryOutputPath, binaryData);

            Debug.Log(ZString.Format("[CsvBinarySerializer::ConvertCSVToBinary] " +
                                     "Converted {0} objects to binary: {1}", objects.Length, binaryOutputPath));
        }
    }
}