using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ServiceConnectorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty] 
        public string Query { get; set; }
        public DataTable Results { get; private set; }
        public string ErrorMessage { get; private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPost()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                string connectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING")!;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(Query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    try
                    {
                        connection.Open();
                        adapter.Fill(dataTable);
                        Results = dataTable;
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while executing the query: {Query}", Query);
                        ErrorMessage = "An error occurred while executing the query. Please check the query and try again.";
                    }
                }
            }
        }

    }
}