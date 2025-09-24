using System;
using System.Collections.Generic;

namespace CrazyRisk.Core
{
    public enum Continente
    {
        AmericaNorte, AmericaSur, Europa, Africa, Asia, Oceania
    }

    public enum TerritorioId
    {
        // Aquí luego irán los 42 territorios (de momento vacío)
    }

    public class Territorio
    {
        public TerritorioId Id { get; }
        public string Nombre { get; }
        public Continente Continente { get; }
        public Ejercito? Duenio { get; private set; }
        public int Tropas { get; private set; }
        public List<TerritorioId> Vecinos { get; } = new List<TerritorioId>();

        public Territorio(TerritorioId id, string nombre, Continente cont)
        {
            Id = id;
            Nombre = nombre;
            Continente = cont;
        }

        public void CambiarDuenio(Ejercito nuevoDuenio) => Duenio = nuevoDuenio;

        public void AgregarTropas(int n)
        {
            if (n <= 0) throw new InvalidOperationException("Cantidad inválida.");
            Tropas += n;
        }

        public void QuitarTropas(int n)
        {
            if (n <= 0 || n > Tropas) throw new InvalidOperationException("Cantidad inválida.");
            Tropas -= n;
        }

        public bool EsVecinoDe(TerritorioId otro) => Vecinos.Contains(otro);

        public override string ToString() => $"{Nombre} [{Continente}] - Tropas: {Tropas}";
    }

    public static class MapaRisk
    {
        public static Dictionary<TerritorioId, Territorio> Crear()
        {
            var t = new Dictionary<TerritorioId, Territorio>();

            // Aquí más adelante se agregarán los 42 territorios y sus adyacencias

            return t;
        }

        private static Territorio T(TerritorioId id, string nombre, Continente c) =>
            new Territorio(id, nombre, c);

        private static void Add(Dictionary<TerritorioId, Territorio> map, TerritorioId a, params TerritorioId[] vecinos)
        {
            foreach (var v in vecinos)
            {
                if (!map[a].Vecinos.Contains(v)) map[a].Vecinos.Add(v);
                if (!map[v].Vecinos.Contains(a)) map[v].Vecinos.Add(a);
            }
        }
    }
}
