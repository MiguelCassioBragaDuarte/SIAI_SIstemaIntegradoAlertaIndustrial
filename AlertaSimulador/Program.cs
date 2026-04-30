using Shared;
using System.Net.Http.Json;
using Shared.DTOs;


var http = new HttpClient();
string urlApi = "https://localhost:7127/api/Alerta";

Console.WriteLine("Aguardando a API estabilizar...");
Thread.Sleep(3000);

Console.WriteLine("--- SIMULADOR DE SENSOR INDUSTRIAL INICIADO ---");

while (true)
{

    var leituraDto = new LeituraSensorDTO
    {
        SensorNome = "Sensor_Caldeira_A1",
        Valor = new Random().Next(20, 110),
        UnidadeMedida = "°C",
        DataHora = DateTime.Now
    };

    var response = await http.PostAsJsonAsync(urlApi, leituraDto);

    if (!response.IsSuccessStatusCode)
    {
        var erro = await response.Content.ReadAsStringAsync();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Erro: {response.StatusCode} - {erro}");
        Console.ResetColor();
    }
    else
    {
        Console.Write($"[{leituraDto.DataHora:HH:mm:ss}] Valor enviado: {leituraDto.Valor}°C ");

        if (leituraDto.Valor > 90)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" -> [ALERTA GERADO]");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine(" -> [OK]");
        }
    }

    await Task.Delay(2000);
}