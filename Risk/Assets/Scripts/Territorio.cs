#nullable enable
using System;

namespace CrazyRisk.Core
{
    public class Territorio
    {
        public TerritorioId Id { get; }
        public string? Nombre { get; }
        public Continente? Continente { get; }
        public Ejercito? Duenio { get; private set; }
        public int Tropas { get; private set; }

        // Ahora usamos nuestra lista enlazada genérica para los vecinos
        public LinkedList<TerritorioId> Vecinos { get; } = new LinkedList<TerritorioId>();

        public Territorio(TerritorioId id, Continente Continente)
        {
            Id = id;
            Tropas = 0;
            this.Continente = Continente;
            Nombre = id.ToString();
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

        public override string ToString()
        {
            string duenioStr = Duenio == null ? "Sin dueño" : Duenio.Alias;
            return $"{Nombre} [{Continente}] - Tropas: {Tropas}, Dueño: {duenioStr}";
        }
    }
}