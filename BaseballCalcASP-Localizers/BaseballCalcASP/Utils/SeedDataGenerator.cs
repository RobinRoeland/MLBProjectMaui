using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using BaseballModelsLib.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseballCalcASP.Utils
{
    public class PlayerCsvRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Rugnummer { get; set; }
        public string Position { get; set; }
        public DateTime DOB { get; set; }
        public string APILink { get; set; }
        public int MLBPersonId { get; set; }
        public int TeamId { get; set; }
        public bool Deleted { get; set; }
    }

    public class PlayerCsvMap : ClassMap<PlayerCsvRecord>
    {
        public PlayerCsvMap()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Rugnummer).Index(2);
            Map(m => m.Position).Index(3);
            Map(m => m.DOB).Index(4);
            Map(m => m.APILink).Index(5);
            Map(m => m.MLBPersonId).Index(6);
            Map(m => m.TeamId).Index(7);
            Map(m => m.Deleted).Index(8);
        }
    }

    public static class SeedDataGenerator
    {
        public static string GeneratePlayerSeedData(string csvFilePath)
        {
            // Define date range for random DOB
            DateTime startDate = new DateTime(2000, 1, 1);
            DateTime endDate = new DateTime(2006, 12, 31);
            int rangeDays = (endDate - startDate).Days;
            var random = new Random();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                HeaderValidated = null,  // Don't throw if header names don't match exactly
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim  // Trim whitespace from fields
            };

            try
            {
                using var reader = new StreamReader(csvFilePath);
                using var csv = new CsvReader(reader, config);
                csv.Context.RegisterClassMap<PlayerCsvMap>();

                var records = csv.GetRecords<PlayerCsvRecord>().ToList();

                var sb = new StringBuilder();
                foreach (var record in records)
                {
                    DateTime randomDOB = startDate.AddDays(random.Next(rangeDays));

                    sb.AppendLine($"            new Player {{ " +
                                 $"Id = {record.Id}, " +
                                 $"Name = \"{record.Name}\", " +
                                 $"Rugnummer = {record.Rugnummer}, " +
                                 $"Position = {record.Position}, " +
                                 $"APILink = {record.APILink}, " +
                                 $"MLBPersonId = {record.MLBPersonId}, " +
                                 $"TeamId = {record.TeamId}, " +
                                 $"DOB = DateTime.ParseExact(\"{randomDOB:dd-MM-yyyy} 12:00 am\", \"dd-MM-yyyy hh:mm tt\", CultureInfo.InvariantCulture), " +
                                 $"Deleted = {record.Deleted.ToString().ToLower()} }},");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV: {ex.Message}");
                throw;
            }
        }

    }
}