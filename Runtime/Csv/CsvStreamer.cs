namespace MK.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using CsvHelper.Configuration;
    using Cysharp.Threading.Tasks;

    internal sealed class CsvStreamer
    {
        private readonly ConverterResolver resolver;

        public CsvStreamer(ConverterResolver resolver)
        {
            this.resolver = resolver;
        }

        public async UniTask<IData> ReadAsync(Type type, string csvData)
        {
            var stringMatrix = await this.ParseFileToMatrixAsync(csvData);
            if (typeof(CsvColumnData).IsAssignableFrom(type))
            {
                return (IData)this.ParseToCsvRowData(stringMatrix, type);
            }

            if (typeof(CsvRowData<,>).IsAssignableFrom(type))
            {
                return (IData)this.ParseToCsvRowData(stringMatrix, type);
            }

            return null;
        }

        public UniTask WriteAsync(IData data)
        {
            return UniTask.CompletedTask;
        }

        private object ParseToCsvColumnData(string[,] dataMatrix, Type type)
        {
            var rowIndex   = 0;
            var columIndex = 0;

            return GetRecord(rowIndex, columIndex, type);

            object GetRecord(int startRowIndex, int startColumIndex, Type recordType)
            {
                var record     = Activator.CreateInstance(recordType);
                var properties = recordType.GetProperties();
                var row        = startRowIndex;
                var col        = startColumIndex;
                foreach (var property in properties)
                {
                    var converter = this.resolver.GetConverter(property.PropertyType);
                    property.SetValue(record, converter.ConvertFromString(dataMatrix[col++, row++], property.PropertyType));
                }

                return record;
            }
        }

        private object ParseToCsvRowData(string[,] dataMatrix, Type type)
        {
            var data       = Activator.CreateInstance(type);
            var properties = type.GetProperties();
            var rowIndex   = 0;
            foreach (var propertyInfo in properties)
            {
                var converter = this.resolver.GetConverter(propertyInfo.PropertyType);
                propertyInfo.SetValue(data, converter.ConvertFromString(dataMatrix[0, rowIndex++], propertyInfo.PropertyType));
            }

            return data;
        }

        private async UniTask<string[,]> ParseFileToMatrixAsync(string csvData)
        {
            var rows = new List<string[]>();

            using (var reader = new StringReader(csvData))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                while (await csv.ReadAsync().AsUniTask())
                {
                    var row = new string[csv.Parser.Count];
                    for (var i = 0; i < csv.Parser.Count; i++)
                    {
                        row[i] = csv.GetField(i);
                    }

                    rows.Add(row);
                }
            }

            if (rows.Count == 0) return new string[0, 0];

            var rowCount = rows.Count;
            var colCount = rows.Max(r => r.Length);
            var matrix   = new string[rowCount, colCount];

            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < rows[r].Length; c++)
                {
                    matrix[r, c] = rows[r][c];
                }
            }

            return matrix;
        }
    }
}