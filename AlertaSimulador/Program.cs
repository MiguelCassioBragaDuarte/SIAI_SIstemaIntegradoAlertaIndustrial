using Shared;
using System.Net.Http.Json;
using Shared.DTOs;


var http = new HttpClient();
string urlApi = "https://localhost:7127/api/Alerta";


Console.WriteLine("--- SIMULADOR DE SENSOR INDUSTRIAL INICIADO ---");

while (true)
{
    // Criando o objeto conforme sua classe no Shared
    var leituraDto = new LeituraSensorDTO
    {
        SensorNome = "Sensor_Caldeira_A1",
        Valor = new Random().Next(20, 110), // Simula de 20 a 110 graus
        UnidadeMedida = "°C",
        DataHora = DateTime.Now
    };

    // Enviando para a API (Endpoint de recepção)
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

        // Destaque visual caso gere um alerta (conforme sua regra de > 90 na Service)
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

    // Aguarda 2 segundos para a próxima leitura
    await Task.Delay(2000);
}