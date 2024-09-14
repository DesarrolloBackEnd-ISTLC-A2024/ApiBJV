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
    public class ActorController : ControllerBase
    {
        private const string BaseUrlApiA = "https://localhost:7071/api/Actors";

        // GET: api/Actor
        [HttpGet]
        public IActionResult Get()
        {
            List<Actor> actores = new List<Actor>();
            try
            {
                using (SqlConnection conn = ConexionDB.abrirConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Actor", conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Actor actor = new Actor
                            {
                                Idactor = (int)reader["id_actor"],
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                FechaNacimiento = reader["fecha_nacimiento"].ToString(),
                                Nacionalidad = reader["nacionalidad"].ToString(),
                                GeneroBiografia = reader["genero_biografia"].ToString(),
                                Premios = reader["premios"].ToString(),
                                NumeroPeliculas = reader["numero_peliculas"] != DBNull.Value ? (int?)reader["numero_peliculas"] : null,
                                FechaCreacion = reader["fecha_creacion"] != DBNull.Value ? reader["fecha_creacion"].ToString() : null
                            };
                            actores.Add(actor);
                        }
                    }
                }
                return Ok(actores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener los actores.", error = ex.Message });
            }
            finally
            {
                ConexionDB.CerrarConexion();
            }
        }

        // GET api/Actor/{id}
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                using (SqlConnection conn = ConexionDB.abrirConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Actor WHERE id_actor = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            Actor actor = new Actor
                            {
                                Idactor = (int)reader["id_actor"],
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                FechaNacimiento = reader["fecha_nacimiento"].ToString(),
                                Nacionalidad = reader["nacionalidad"].ToString(),
                                GeneroBiografia = reader["genero_biografia"].ToString(),
                                Premios = reader["premios"].ToString(),
                                NumeroPeliculas = reader["numero_peliculas"] != DBNull.Value ? (int?)reader["numero_peliculas"] : null,
                                FechaCreacion = reader["fecha_creacion"] != DBNull.Value ? reader["fecha_creacion"].ToString() : null
                            };
                            return Ok(actor);
                        }
                        else
                        {
                            return NotFound(new { message = "Actor no encontrado." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener el actor.", error = ex.Message });
            }
            finally
            {
                ConexionDB.CerrarConexion();
            }
        }

        // POST api/Actor
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Actor actor)
        {
            try
            {
                using (SqlConnection conn = ConexionDB.abrirConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("InsertActor", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nombre", actor.Nombre);
                        cmd.Parameters.AddWithValue("@apellido", actor.Apellido);
                        cmd.Parameters.AddWithValue("@fecha_nacimiento", actor.FechaNacimiento);
                        cmd.Parameters.AddWithValue("@nacionalidad", actor.Nacionalidad);
                        cmd.Parameters.AddWithValue("@genero_biografia", actor.GeneroBiografia);
                        cmd.Parameters.AddWithValue("@premios", actor.Premios);
                        cmd.Parameters.AddWithValue("@numero_peliculas", actor.NumeroPeliculas);
                        cmd.Parameters.AddWithValue("@fecha_creacion", actor.FechaCreacion ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        int newActorId = Convert.ToInt32(cmd.ExecuteScalar());
                        // Llamada a la API externa
                        using (HttpClient client = new HttpClient())
                        {
                            string contenidoJson = JsonSerializer.Serialize(actor);
                            var contenidoPeticion = new StringContent(contenidoJson, Encoding.UTF8, "application/json");

                            try
                            {
                                HttpResponseMessage respuestaHttp = await client.PostAsync(BaseUrlApiA, contenidoPeticion);

                                if (respuestaHttp.IsSuccessStatusCode)
                                {
                                    return Ok(new { id = newActorId, message = "Actor creado exitosamente y sincronizado con BASE_A." });
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
                return StatusCode(500, new { message = "Ocurrió un error al insertar el actor.", error = ex.Message });
            }
            finally
            {
                ConexionDB.CerrarConexion();
            }
        }

        // PUT api/Actor/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Actor actor)
        {
            // Implementa la lógica para actualizar un actor aquí
            return NoContent(); // Temporal
        }

        // DELETE api/Actor/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Implementa la lógica para eliminar un actor aquí
            return NoContent(); // Temporal
        }
    }
}
