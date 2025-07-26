namespace Server.Application.Port;

public interface ICsvService
{
    List<T> ReadCsv<T>(string filePath);
}