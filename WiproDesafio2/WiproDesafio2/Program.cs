// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using RestSharp;
using System.Text.Json;
using WiproDesafio2.Models;
using WiproDesafio2.Models.Dto;
using Microsoft.Extensions.Hosting;
using WiproDesafio2.Models.Enuns;
using System.Text;
using System.Data;

using IHost host = Host.CreateDefaultBuilder(args).Build();

string caminhoBaseCsv = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

string caminhoCsvMoeda = $"{caminhoBaseCsv}/Arquivos/DadosMoeda.csv";
string caminhoCsvCotacao =$"{caminhoBaseCsv}/Arquivos/DadosCotacao.csv";

List<CsvMoeda> csvMoedas = getCsvMoeda();
List<CsvCotacao> csvCotacoes = getCsvCotacao();

while (true) {
    ItemDto itemDto = await obterItem();

    if (itemDto.error != null) { 
        await Task.Delay(120000);
    } else {
        List<CsvMoeda> itensNovoCsv = csvMoedas.Where(a => a.ID_MOEDA == itemDto.moeda && a.DATA_REF >= itemDto.data_inicio && a.DATA_REF <= itemDto.data_fim).ToList();

        itensNovoCsv.Add(new CsvMoeda {
            DATA_REF = itemDto.data_fim,
            ID_MOEDA = itemDto.moeda
        });

        gerarNovoCsv();

        await Task.Delay(120000);

        void gerarNovoCsv() {
            DataTable tb = new DataTable();
            tb.Columns.Add("ID_MOEDA", typeof(string));
            tb.Columns.Add("DATA_REF", typeof(string));
            tb.Columns.Add("VLR_COTACAO", typeof(string));

            List<CsvNovo> listaNovoCsv = itensNovoCsv.Select(a => new CsvNovo {
                ID_MOEDA = a.ID_MOEDA,
                DATA_REF = a.DATA_REF,
                vlr_cotacao = csvCotacoes.Where(c => c.moeda_cotacao == a.ID_MOEDA).Select(a=> a.vlr_cotacao).First()
            }).ToList();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 3; i++) {
                sb.Append($"{tb.Columns[i].ColumnName};");
            }

            sb.Append("\r\n");

            for (int i = 0; i < listaNovoCsv.Count; i++)
            {
                CsvNovo row = listaNovoCsv[i];

                sb.Append($"{row.ID_MOEDA}; {row.DATA_REF.ToString("dd/MM/yyyy")}; {row.vlr_cotacao}" );

                sb.Append("\r\n");
            }
    
            var nomeArquivo = $"Resultado_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv";

            File.WriteAllText($"C:\\Users\\Public\\{nomeArquivo}", sb.ToString());
        }
    }
}


List<CsvMoeda> getCsvMoeda() {
    List<CsvMoeda> listaMoedas = new List<CsvMoeda>();

    using (TextFieldParser csvReader = new TextFieldParser(caminhoCsvMoeda))
    {
        csvReader.CommentTokens = new string[] { "#" };
        csvReader.SetDelimiters(new string[] { "," });
        csvReader.HasFieldsEnclosedInQuotes = true;

        // Skip the row with the column names
        csvReader.ReadLine();

        while (!csvReader.EndOfData)
        {
            // Read current line fields, pointer moves to the next line.
            string[] fields = csvReader.ReadFields();

            foreach (string field in fields) {
                var moeda = field.Split(';');

                listaMoedas.Add(new CsvMoeda {
                    ID_MOEDA = moeda[0],
                    DATA_REF = DateTime.Parse(moeda[1])
                });
            }
        } 
    }

    return listaMoedas;
}

List<CsvCotacao> getCsvCotacao() {
    List<CsvCotacao> listaCotacoes = new List<CsvCotacao>();

    StreamReader reader = new StreamReader(File.OpenRead(caminhoCsvCotacao));
    int x = 0;
    while (!reader.EndOfStream) {
        string line = reader.ReadLine();
        
        if (x > 0) {
            var cotacao = line.Split(";");

            listaCotacoes.Add(new CsvCotacao { 
                vlr_cotacao = Convert.ToDouble(cotacao[0]),
                cod_cotacao = Convert.ToInt32(cotacao[1]),
                moeda_cotacao = ((Cotacao)Convert.ToInt32(cotacao[1])).ToString(),
                dat_cotacao = DateTime.Parse(cotacao[2])
            });

            x++;
        }

        x++;
    }

    return listaCotacoes;
}

async Task<ItemDto> obterItem() {
    var url = $"{obterUrl()}/Items/Item";
    var client = new RestClient(url);
    var request = new RestRequest(url, Method.Get);
    RestResponse response = await client.ExecuteAsync(request);
    var Output = response.Content;

    return JsonSerializer.Deserialize<ItemDto>(Output);
}

string obterUrl() {
    IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    var url = config["url"];

    return url;
}