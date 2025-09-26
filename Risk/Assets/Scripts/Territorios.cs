#nullable enable
using System;
using System.Collections.Generic;

namespace CrazyRisk.Core
{
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
            Continente = continente;
            Tropas = 0;
        }

        public void CambiarDuenio(Ejercito nuevoDuenio) => Duenio = nuevoDuenio;

        public void AgregarTropas(int cantidad)
        {
            if (cantidad <= 0)
                throw new InvalidOperationException("La cantidad de tropas a agregar debe ser positiva.");

            Tropas += cantidad;
        }

        public void QuitarTropas(int cantidad)
        {
            if (cantidad <= 0 || cantidad > Tropas)
                throw new InvalidOperationException("Cantidad de tropas inválida.");

            Tropas -= cantidad;
        }

        public bool EsVecinoDe(TerritorioId otro) => Vecinos.Contains(otro);

        public override string ToString()
        {
            string duenioStr = Duenio == null ? "Sin dueño" : Duenio.Alias;
            return $"{Nombre} [{Continente}] - Tropas: {Tropas}, Dueño: {duenioStr}";
        }
    }
}
