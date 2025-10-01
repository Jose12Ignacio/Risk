namespace CrazyRisk.Core
{
    public class ResultadoCombate
    {
        // Tiradas[0] = atacante, Tiradas[1] = defensor
        public int[][] Tiradas { get; set; }

        // Detalles de cada comparación
        public string[] Detalles { get; set; }

        public int PerdidasAtacante { get; set; }
        public int PerdidasDefensor { get; set; }

        public string AtacanteColor { get; set; }
        public string DefensorColor { get; set; }
    }
}

