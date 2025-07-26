using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Server.Application.Port;

namespace Server.Infrastructure.Service;

public class CsvService : ICsvService
{
    public List<T> ReadCsv<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
        });
        
        var records = csv.GetRecords<T>();
        return new List<T>(records);
    }
}