using System;
using System.Collections.Generic;

namespace CrazyRisk
{
    // Tipos de cartas
    public enum TipoTarjeta
    {
        Infanteria,
        Caballeria,
        Artilleria,
        Comodin
    }

    // Continentes
    public enum Continente
    {
        AmericaNorte,
        AmericaSur,
        Europa,
        Africa,
        Asia,
        Oceania
    }

    // Identificador de territorios (lo puedes completar después)
    public enum TerritorioId
    {
        // Aquí luego van los 42 territorios...
    }

    // Clase Tarjeta
    public class Tarjeta
    {
        public TipoTarjeta Tipo { get; }
        public TerritorioId? Territorio { get; }
        public bool EsComodin => Tipo == TipoTarjeta.Comodin;

        public Tarjeta(TipoTarjeta tipo, TerritorioId? territorio = null)
        {
            Tipo = tipo;
            Territorio = territorio;
        }

        public override string ToString()
            => EsComodin ? "Comodín" : $"{Tipo} - {Territorio}";
    }

    // Clase Tropa
    public class Tropa
    {
        public string Color { get; }

        public Tropa(string color)
        {
            Color = color;
        }

        public override string ToString() => $"Tropa {Color}";
    }

    // Clase Territorio (sin vecinos aún)
    public class Territorio
    {
        public TerritorioId Id { get; }
        public string Nombre { get; }
        public Continente Continente { get; }
        public Ejercito? Duenio { get; private set; }
        public int Tropas { get; private set; }
        public List<TerritorioId> Vecinos { get; } = new List<TerritorioId>();

        public Territorio(TerritorioId id, string nombre, Continente continente)
        {
            Id = id;
            Nombre = nombre;
            Continente = continente;
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

        public override string ToString() => $"{Nombre} [{Continente}] - Tropas: {Tropas}";
    }

    // Clase Ejercito
    public class Ejercito
    {
        public string Alias { get; }
        public string Color { get; }
        public int TropasDisponibles { get; private set; }
        public List<Tropa> Tropas { get; } = new List<Tropa>();
        public List<Tarjeta> Tarjetas { get; } = new List<Tarjeta>();

        public Ejercito(string alias, string color, int tropasIniciales)
        {
            Alias = alias;
            Color = color;
            TropasDisponibles = tropasIniciales;

            for (int i = 0; i < tropasIniciales; i++)
                Tropas.Add(new Tropa(color));
        }

        public void RecibirRefuerzos(int cantidad)
        {
            TropasDisponibles += cantidad;
            for (int i = 0; i < cantidad; i++)
                Tropas.Add(new Tropa(Color));
        }

        public void RecibirTarjeta(Tarjeta tarjeta)
        {
            if (Tarjetas.Count >= 6)
                throw new InvalidOperationException("Máximo 6 tarjetas permitidas.");
            Tarjetas.Add(tarjeta);
        }

        public override string ToString()
            => $"Ejército {Alias} ({Color}) - Tropas: {TropasDisponibles}, Tarjetas: {Tarjetas.Count}";
    }
}
