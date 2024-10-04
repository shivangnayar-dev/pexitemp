using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;

public class PexiticsscoreEmailService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PexiticsscoreEmailService> _logger;
    private readonly TimeZoneInfo _indianTimeZone;

  public PexiticsscoreEmailService(IServiceProvider serviceProvider, ILogger<PexiticsscoreEmailService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata"); // Get IST Time Zone
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Calculate delay until next 11:24 PM IST
            var delay = CalculateDelayUntilNext1124PMIST();

            _logger.LogInformation($"Waiting for {delay.TotalHours} hours until 11:24 PM IST to send the email.");

            // Wait until 11:24 PM IST
            await Task.Delay(delay, stoppingToken);

            try
            {
                // Step 1: Fetch data from both tables and generate CSV in-memory, side by side
                string combinedCsvData = await GetCombinedDataAsSideBySideCsvAsync();

                // Step 2: Send the email with the combined CSV data as an attachment
                await SendEmailWithCsvAttachmentAsync(combinedCsvData);
                _logger.LogInformation("Email sent with combined CSV data (side-by-side).");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PexiticsscoreEmailService while sending email: {ex.Message}");
            }
        }
    }

    private TimeSpan CalculateDelayUntilNext1124PMIST()
    {
        DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _indianTimeZone);
        DateTime next1124PM = now.Date.AddHours(23).AddMinutes(50); // Today's 11:24 PM IST

        if (now > next1124PM)
        {
            // If 11:24 PM has passed today, calculate the delay for tomorrow's 11:24 PM
            next1124PM = next1124PM.AddDays(1);
        }

        return next1124PM - now;
    }


    private async Task<string> GetCombinedDataAsSideBySideCsvAsync()
    {
        string pexiticsscoreCsvData = await GetPexiticsscoreDataAsCsvAsync();
        string pexiticsTableCsvData = await GetPexiticsTableDataAsCsvAsync();

        // Split the CSV data into rows
        var pexiticsscoreRows = pexiticsscoreCsvData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var pexiticsTableRows = pexiticsTableCsvData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Determine the number of columns for each table
        int pexiticsscoreColumns = pexiticsscoreRows[0].Split(',').Length;
        int pexiticsTableColumns = pexiticsTableRows[0].Split(',').Length;

        int maxRows = Math.Max(pexiticsscoreRows.Length, pexiticsTableRows.Length);
        StringBuilder combinedCsvBuilder = new StringBuilder();

        // Ensure both tables have the same number of rows by padding the shorter one
        for (int i = 0; i < maxRows; i++)
        {
            string pexiticsscoreRow = i < pexiticsscoreRows.Length ? pexiticsscoreRows[i] : new string(',', pexiticsscoreColumns - 1);
            string pexiticsTableRow = i < pexiticsTableRows.Length ? pexiticsTableRows[i] : new string(',', pexiticsTableColumns - 1);

            // Add the rows from both tables side by side, separated by some space
            combinedCsvBuilder.Append(pexiticsscoreRow).Append(", , ,").Append(pexiticsTableRow).AppendLine();
        }

        return combinedCsvBuilder.ToString();
    }

    private async Task<string> GetPexiticsscoreDataAsCsvAsync()
    {
        string connectionString = "Server=178.16.139.40;Database=CandidateDatabase;User=neww;Password=your_password;";
        StringBuilder csvBuilder = new StringBuilder();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM pexiticsscore";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Build CSV header for pexiticsscore
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            csvBuilder.Append(EscapeCsvValue(reader.GetName(i)));
                            if (i < reader.FieldCount - 1)
                                csvBuilder.Append(",");
                        }
                        csvBuilder.AppendLine();

                        // Build CSV rows for pexiticsscore
                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString();
                                csvBuilder.Append(EscapeCsvValue(value));
                                if (i < reader.FieldCount - 1)
                                    csvBuilder.Append(",");
                            }
                            csvBuilder.AppendLine();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching data from pexiticsscore: {ex.Message}");
        }

        return csvBuilder.ToString();
    }

    private async Task<string> GetPexiticsTableDataAsCsvAsync()
    {
        string connectionString = "Server=178.16.139.40;Database=CandidateDatabase;User=neww;Password=your_password;";
        StringBuilder csvBuilder = new StringBuilder();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Pexitics_Table";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Build CSV header for Pexitics_Table
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            csvBuilder.Append(EscapeCsvValue(reader.GetName(i)));
                            if (i < reader.FieldCount - 1)
                                csvBuilder.Append(",");
                        }
                        csvBuilder.AppendLine();

                        // Build CSV rows for Pexitics_Table
                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString();
                                csvBuilder.Append(EscapeCsvValue(value));
                                if (i < reader.FieldCount - 1)
                                    csvBuilder.Append(",");
                            }
                            csvBuilder.AppendLine();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching data from Pexitics_Table: {ex.Message}");
        }

        return csvBuilder.ToString();
    }

    private string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // Escape values containing commas, quotes, or newlines by wrapping in quotes
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }

        return value;
    }

    private async Task SendEmailWithCsvAttachmentAsync(string csvData)
    {
        var smtpServer = "smtp.gmail.com"; // Replace with your SMTP server
        var smtpPort = 587; // Replace with your SMTP port
        var enableSsl = true; // Replace with your SSL/TLS configuration
        var username = "ai.careertests@gmail.com"; // Replace with your email address
        var password = "szyaorwsvdbdajqb"; // Use your app-specific password

        using (var client = new SmtpClient(smtpServer, smtpPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = enableSsl;

            var message = new MailMessage();
            message.From = new MailAddress(username);

            // Add recipient(s)
            message.To.Add("shivangnayar22@gmail.com");
            message.To.Add("subhashini@pexitics.com");

            message.Subject = "Pexiticsscore and Pexitics_Table Data CSV";
            message.Body = "Please find the attached CSV file for Pexiticsscore and Pexitics_Table data.";
            message.IsBodyHtml = false;

            // Attach the combined CSV data as an in-memory attachment
            byte[] byteArray = Encoding.UTF8.GetBytes(csvData);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                Attachment attachment = new Attachment(stream, "combined_pexiticsscore_and_pexitics_table.csv", "text/csv");
                message.Attachments.Add(attachment);

                // Send the email
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent with combined CSV attachment.");
            }
        }
    }
}
