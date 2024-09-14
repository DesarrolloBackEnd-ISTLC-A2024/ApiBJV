using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ApiB.Models;
using ApiB.Comunes;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace ApiB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculaController : ControllerBase
    {
        private const string BaseUrlApiA = "https://localhost:7071/api/Peliculas";

        // GET: api/Pelicula
        [HttpGet]
        public IActionResult Get()
        {
            List<Pelicula> peliculas = new List<Pelicula>();
            try
            {
                using (SqlConnection conn = ConexionDB.abrirConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Pelicula", conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Pelicula pelicula = new Pelicula
                            {
                                IdPelicula = (int)reader["id_pelicula"],
                                Titulo = reader["titulo"].ToString(),
                                Genero = reader["genero"].ToString(),
                                Director = reader["director"].ToString(),
                                AnioEstreno = reader["anio_estreno"] != DBNull.Value ? (int?)reader["anio_estreno"] : null,
                                Duracion = reader["duracion"] != DBNull.Value ? (int?)reader["duracion"] : null,
                                Sinopsis = reader["sinopsis"].ToString(),
                                FechaCreacion = reader["fecha_creacion"] != DBNull.Value ? (DateTime?)reader["fecha_creacion"] : null
                            };
                            peliculas.Add(pelicula);
                        }
                    }
                }
                return Ok(peliculas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener las películas.", error = ex.Message });
            }
            finally
            {
                ConexionDB.CerrarConexion();
            }
        }

        // POST api/Pelicula
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pelicula pelicula)
        {
            try
            {
                using (SqlConnection conn = ConexionDB.abrirConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("InsertPelicula", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@titulo", pelicula.Titulo);
                        cmd.Parameters.AddWithValue("@genero", pelicula.Genero);
                        cmd.Parameters.AddWithValue("@director", pelicula.Director);
                        cmd.Parameters.AddWithValue("@anio_estreno", pelicula.AnioEstreno);
                        cmd.Parameters.AddWithValue("@duracion", pelicula.Duracion);
                        cmd.Parameters.AddWithValue("@sinopsis", pelicula.Sinopsis);
                        cmd.Parameters.AddWithValue("@fecha_creacion", pelicula.FechaCreacion ?? DateTime.Now);

                        int newPeliculaId = Convert.ToInt32(cmd.ExecuteScalar());
                        // Llamada a la API externa
                        using (HttpClient client = new HttpClient())
                        {
                            string contenidoJson = JsonSerializer.Serialize(pelicula);
                            var contenidoPeticion = new StringContent(contenidoJson, Encoding.UTF8, "application/json");

                            try
                            {
                                HttpResponseMessage respuestaHttp = await client.PostAsync(BaseUrlApiA, contenidoPeticion);

                                if (respuestaHttp.IsSuccessStatusCode)
                                {
                                    return Ok(new { id = newPeliculaId, message = "Película creada exitosamente y sincronizada con BASE_A." });
                                }
                                else
                                {
                                    return StatusCode((int)respuestaHttp.StatusCode, new { message = $"Error al crear el servicio en BASE_A: {respuestaHttp.StatusCode}" });
                                }
                            }
                            catch (HttpRequestException e)
                            {
                                return StatusCode(500, new { message = $"Excepción al intentar crear el servicio en BASE_A: {e.Message}" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al insertar la película.", error = ex.Message });
            }
            finally
            {
                ConexionDB.CerrarConexion();
            }
        }

        // PUT api/Pelicula/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Pelicula pelicula)
        {
            return NoContent(); // Temporal
        }

        // DELETE api/Pelicula/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent(); // Temporal
        }
    }
}
