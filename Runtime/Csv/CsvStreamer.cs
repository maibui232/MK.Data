namespace MK.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using CsvHelper;
    using CsvHelper.Configuration;
    using Cysharp.Threading.Tasks;

    public sealed class CsvStreamer
    {
        private readonly ConverterResolver resolver;
        private readonly CsvConfiguration  configuration;

        public CsvStreamer(ConverterResolver resolver)
        {
            this.resolver = resolver;
            this.configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                                 {
                                     HasHeaderRecord  = true,
                                     Delimiter        = ",",
                                     Quote            = '"',
                                     IgnoreBlankLines = true,
                                     TrimOptions      = TrimOptions.Trim,
                                     BadDataFound     = null
                                 };
        }

        public CsvStreamer(ConverterResolver resolver, CsvConfiguration configuration)
        {
            this.resolver      = resolver;
            this.configuration = configuration;
        }

        public async UniTask<T> ReadAsync<T>(string csvData) where T : IData => (T)await this.ReadAsync(typeof(T), csvData);

        public async UniTask<IData> ReadAsync(Type type, string csvData)
        {
            if (string.IsNullOrEmpty(csvData))
                throw new ArgumentException("CSV data cannot be null or empty", nameof(csvData));

            var dataMatrix = await this.ParseToMatrixAsync(csvData);

            if (dataMatrix.GetLength(0) == 0 || dataMatrix.GetLength(1) == 0)
                return Activator.CreateInstance(type) as IData;

            if (typeof(CsvColumnData).IsAssignableFrom(type))
                return this.ParseColumnData(dataMatrix, type) as IData;

            if (this.IsCsvRowDataType(type))
                return this.ParseRowData(dataMatrix, type) as IData;

            throw new NotSupportedException($"Type {type.Name} must inherit from CsvColumnData or CsvRowData<,>");
        }

        public async UniTask<string> WriteAsync(IData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var stringBuilder = new StringBuilder();
            await using (var writer = new StringWriter(stringBuilder))
            await using (var csv = new CsvWriter(writer, this.configuration))
            {
                if (data is CsvColumnData columnData)
                {
                    await this.WriteColumnDataAsync(csv, columnData);
                }
                else if (data.GetType().IsGenericType && data.GetType().GetGenericTypeDefinition() == typeof(CsvRowData<,>))
                {
                    await this.WriteRowDataAsync(csv, data);
                }
                else throw new NotSupportedException($"Type {data.GetType().Name} must inherit from CsvColumnData or CsvRowData<,>");
            }

            return stringBuilder.ToString();
        }

        private async UniTask<string[,]> ParseToMatrixAsync(string csvData) => this.ConvertToMatrix(await this.ReadCsvRowsAsync(csvData));

        private async UniTask<List<string[]>> ReadCsvRowsAsync(string csvData)
        {
            var rows = new List<string[]>();

            using var reader    = new StringReader(csvData);
            using var csv       = new CsvReader(reader, this.configuration);
            var       hasHeader = this.configuration.HasHeaderRecord;

            while (await csv.ReadAsync())
            {
                if (hasHeader && csv.Parser.Row == 1)
                {
                    csv.ReadHeader();
                    if (csv.HeaderRecord != null)
                        rows.Add(csv.HeaderRecord);

                    continue;
                }

                rows.Add(this.ExtractRowFields(csv));
            }

            return rows;
        }

        private string[] ExtractRowFields(CsvReader csv)
        {
            var row = new string[csv.Parser.Count];
            for (var i = 0; i < csv.Parser.Count; i++)
                row[i] = csv.GetField(i) ?? string.Empty;

            return row;
        }

        private string[,] ConvertToMatrix(List<string[]> rows)
        {
            if (rows.Count == 0) return new string[0, 0];

            var rowCount = rows.Count;
            var colCount = rows.Max(r => r.Length);
            var matrix   = new string[rowCount, colCount];

            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < rows[r].Length; c++)
                    matrix[r, c] = rows[r][c];
                for (var c = rows[r].Length; c < colCount; c++)
                    matrix[r, c] = string.Empty;
            }

            return matrix;
        }

        private object ParseColumnData(string[,] dataMatrix, Type type)
        {
            var instance   = Activator.CreateInstance(type);
            var properties = type.GetWritableProperties();

            var startRow = this.configuration.HasHeaderRecord ? 1 : 0;
            var rowCount = dataMatrix.GetLength(0);
            var colIndex = 0;

            foreach (var property in properties)
            {
                if (colIndex >= dataMatrix.GetLength(1)) break;

                var propertyType = property.PropertyType;

                if (this.IsCsvRowDataType(propertyType)) continue;
                if (startRow < rowCount)
                {
                    var value = dataMatrix[startRow, colIndex];

                    if (!string.IsNullOrEmpty(value))
                    {
                        this.SetPropertyValue(instance, property, value);
                    }
                }

                colIndex++;
            }

            return instance;
        }

        private void WritePropertyValues(CsvWriter csv, object instance, PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                var value = property.GetValue(instance);
                this.WriteFieldValue(csv, value, property.PropertyType);
            }
        }

        private void WriteFieldValue(CsvWriter csv, object value, Type propertyType)
        {
            if (value != null)
            {
                var converter = this.resolver.GetConverter(propertyType);
                csv.WriteField(converter.ConvertToString(value, propertyType));
            }
            else
            {
                csv.WriteField(string.Empty);
            }
        }

        private void SetPropertyValue(object instance, PropertyInfo property, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            var converter      = this.resolver.GetConverter(property.PropertyType);
            var convertedValue = converter.ConvertFromString(value, property.PropertyType);
            property.SetValue(instance, convertedValue);
        }

        private object GetConvertedValue(string value, Type targetType) => this.resolver.GetConverter(targetType).ConvertFromString(value, targetType);

        private bool IsCsvRowDataType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CsvRowData<,>)) return true;

            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(CsvRowData<,>)) return true;
                baseType = baseType.BaseType;
            }

            return false;
        }

        private object ParseRowData(string[,] dataMatrix, Type type)
        {
            var instance    = Activator.CreateInstance(type);
            var genericArgs = type.GetGenericArguments();
            var keyType     = genericArgs[0];
            var recordType  = genericArgs[1];

            var dictionary = instance as IDictionary;

            if (dictionary == null) return instance;

            var recordProperties = recordType.GetWritableProperties();
            var keyProperty      = recordType.GetKeyProperty();

            var startRow = this.configuration.HasHeaderRecord ? 1 : 0;
            var rowCount = dataMatrix.GetLength(0);
            var colCount = dataMatrix.GetLength(1);

            for (var row = startRow; row < rowCount; row++)
            {
                var    record = Activator.CreateInstance(recordType);
                object key    = null;

                for (var col = 0; col < colCount && col < recordProperties.Length; col++)
                {
                    var property = recordProperties[col];
                    var value    = dataMatrix[row, col];

                    if (!string.IsNullOrEmpty(value))
                    {
                        var convertedValue = this.GetConvertedValue(value, property.PropertyType);
                        property.SetValue(record, convertedValue);

                        if (property == keyProperty)
                        {
                            key = keyType != property.PropertyType ? this.GetConvertedValue(value, keyType) : convertedValue;
                        }
                    }
                }

                if (key != null && !dictionary.Contains(key))
                {
                    dictionary.Add(key, record);
                }
            }

            return instance;
        }

        private async UniTask WriteColumnDataAsync(CsvWriter csv, CsvColumnData data)
        {
            var properties = data.GetType().GetReadableProperties();

            if (this.configuration.HasHeaderRecord)
            {
                foreach (var property in properties)
                {
                    csv.WriteField(property.Name);
                }

                await csv.NextRecordAsync();
            }

            foreach (var property in properties)
            {
                var value = property.GetValue(data);
                this.WriteFieldValue(csv, value, property.PropertyType);
            }

            await csv.NextRecordAsync();
        }

        private async UniTask WriteRowDataAsync(CsvWriter csv, IData data)
        {
            var dictionary = data as IDictionary;

            if (dictionary == null) return;

            var type        = data.GetType();
            var genericArgs = type.GetGenericArguments();
            var recordType  = genericArgs[1];

            var properties = recordType.GetReadableProperties();

            if (this.configuration.HasHeaderRecord && dictionary.Count > 0)
            {
                foreach (var property in properties)
                {
                    csv.WriteField(property.Name);
                }

                await csv.NextRecordAsync();
            }

            foreach (var key in dictionary.Keys)
            {
                var record = dictionary[key];
                this.WritePropertyValues(csv, record, properties);
                await csv.NextRecordAsync();
            }
        }
    }

    internal static class CsvStreamerExtensions
    {
        internal static PropertyInfo[] GetWritableProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(p => p.CanWrite && p.GetCustomAttribute<CsvIgnoreAttribute>() == null)
               .ToArray();
        }

        internal static PropertyInfo[] GetReadableProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(p => p.CanRead && p.GetCustomAttribute<CsvIgnoreAttribute>() == null)
               .ToArray();
        }

        internal static PropertyInfo GetKeyProperty(this Type type)
        {
            var properties = type.GetWritableProperties();

            var keyProperty = properties.FirstOrDefault(p =>
                                                            p.GetCustomAttribute<CsvKeyAttribute>() != null          ||
                                                            p.Name.Equals("Id",  StringComparison.OrdinalIgnoreCase) ||
                                                            p.Name.Equals("Key", StringComparison.OrdinalIgnoreCase));

            return keyProperty ?? (properties.Length > 0 ? properties[0] : null);
        }
    }
}