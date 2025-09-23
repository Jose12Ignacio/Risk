using System;              // si usás excepciones u otros tipos del sistema
using System.Collections.Generic;  // solo si usás listas o colecciones

namespace CrazyRisk.Core
{
    public class Tropa
    {
        public string Color { get; }
        public string Tipo { get; }

        public Tropa(string color, string tipo = "Infantería")
        {
            Color = color;
            Tipo = tipo;
        }

        public override string ToString() => $"{Tipo} ({Color})";
    }
}
