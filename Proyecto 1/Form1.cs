using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Proyecto_1
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Server=TU_SERVIDOR;Database=InvestigacionesAI;Trusted_Connection=True;"; // Reemplaza con tu cadena de conexión

        public Form1()
        {
            InitializeComponent();
            txtResultado.ReadOnly = true;
        }
        private  void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void btnInvestigar_Click(object sender, EventArgs e)
        {
            string tema = txtTema.Text.Trim();
            if (string.IsNullOrEmpty(tema))
            {
                MessageBox.Show("Por favor, ingresa un tema.");
                return;
            }

            string respuesta = await ObtenerRespuestaDeNvidia("Investiga sobre: " + tema);
            txtResultado.Text = respuesta;

            GuardarEnBaseDeDatos(tema, respuesta);
        }
        private async Task<string> ObtenerRespuestaDeNvidia(string prompt)
        {
            using (HttpClient client = new HttpClient())
            {
                // Endpoint de inferencia de NVIDIA LLaMA 3.1
                string endpoint = "https://integrate.api.nvidia.com/v1/chat/completions";

                // Reemplaza con tu clave API real
                string apiKey = "API_de_NVIDIA";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "meta/llama3-70b-instruct",
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            }
                };

                string json = JsonSerializer.Serialize(requestBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                string responseString = await response.Content.ReadAsStringAsync();

                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseString))
                    {
                        var root = doc.RootElement;

                        var message = root
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();

                        return message;
                    }

                }
                catch (Exception ex)
                {
                    return $"Error al analizar respuesta de NVIDIA API: {ex.Message}\nRespuesta cruda:\n{responseString}";
                }
            }
        }
        private void GuardarEnBaseDeDatos(string consulta, string respuesta)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Investigaciones (Consulta, Resultado, Fecha) VALUES (@consulta, @resultado, @fecha)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@consulta", consulta);
                cmd.Parameters.AddWithValue("@resultado", respuesta);
                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtTema.Clear();
            txtResultado.Clear();
            txtResultado.ReadOnly = true;
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            txtResultado.ReadOnly = !txtResultado.ReadOnly;
        }
        private void btnGenerarDocs_Click(object sender, EventArgs e)
        {
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Investigacion_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(carpeta);

            string wordPath = Path.Combine(carpeta, "informe.docx");
            string pptPath = Path.Combine(carpeta, "presentacion.pptx");

            WordHelper.CrearDocumento(txtResultado.Text, wordPath);
            PowerPointHelper.CrearPresentacion(txtResultado.Text, pptPath);

            MessageBox.Show("Documentos generados en: " + carpeta);
        }
        private void txtTema_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
