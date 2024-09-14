namespace ApiB.Models
{
    public class Actor
    {
        public int Idactor { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string FechaNacimiento { get; set; }
        public string Nacionalidad { get; set; }
        public string GeneroBiografia { get; set; }
        public string Premios { get; set; }
        public int? NumeroPeliculas { get; set; }
        public string FechaCreacion { get; set; }
    }
}
