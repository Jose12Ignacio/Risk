using System;
using System.Collections.Generic;

namespace CrazyRisk.Core
{
    public class Ejercito
    {
        public string Alias { get; }
        public string Color { get; }
        public int TropasDisponibles { get; private set; }
        public List<Tropa> Tropas { get; } = new List<Tropa>();
        public List<Tarjeta> Tarjetas { get; } = new List<Tarjeta>();

        public Ejercito(string alias, string color, int tropasIniciales)
        {
            Alias = alias; Color = color; TropasDisponibles = tropasIniciales;
            for (int i = 0; i < tropasIniciales; i++) Tropas.Add(new Tropa(color));
        }

        public void RecibirRefuerzos(int cantidad)
        {
            TropasDisponibles += cantidad;
            for (int i = 0; i < cantidad; i++) Tropas.Add(new Tropa(Color));
        }

        public void RecibirTarjeta(Tarjeta tarjeta)
        {
            if (Tarjetas.Count >= 6) throw new InvalidOperationException("Máximo 6 tarjetas permitidas.");
            Tarjetas.Add(tarjeta);
        }

        public override string ToString()
            => $"Ejército {Alias} ({Color}) - Tropas: {TropasDisponibles}, Tarjetas: {Tarjetas.Count}";
    }
}
