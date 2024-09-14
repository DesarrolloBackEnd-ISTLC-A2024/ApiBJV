using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;


namespace ApiB.Comunes
{
    public class ConexionDB
    {
        private static SqlConnection conexion;

        // Método para abrir la conexión
        public static SqlConnection abrirConexion()
        {
            conexion = new SqlConnection("Server=JKV\\SQLEXPRESS;Database=BaseB;Trusted_Connection=True;TrustServerCertificate=True;");
            conexion.Open();
            return conexion;
        }

        // Método para cerrar la conexión y liberar recursos
        public static void CerrarConexion()
        {
            if (conexion != null && conexion.State == System.Data.ConnectionState.Open)
            {
                conexion.Close();
                conexion.Dispose();
            }
        }
    }
}
