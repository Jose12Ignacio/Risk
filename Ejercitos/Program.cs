using System;
using System.Collections.Generic;

namespace CrazyRisk.Core
{
    // --------------------------
    // TARJETA
    // --------------------------
    public enum TipoTarjeta { Infanteria, Caballeria, Artilleria }

    public class Tarjeta
    {
        public TipoTarjeta Tipo { get; private set; }
        public string TerritorioAsociado { get; private set; } // opcional, por si la carta tiene nombre

        public Tarjeta(TipoTarjeta tipo, string territorioAsociado = null)
        {
            Tipo = tipo;
            TerritorioAsociado = territorioAsociado;
        }

        public override string ToString() => $"{Tipo}" + (TerritorioAsociado != null ? $" ({TerritorioAsociado})" : "");
    }

    // --------------------------
    // TERRITORIO
    // --------------------------
    public class Territorio
    {
        public string Nombre { get; private set; }
        public Ejercito Duenio { get; private set; }
        public int Tropas { get; private set; }

        public Territorio(string nombre, Ejercito duenioInicial, int tropasIniciales = 1)
        {
            if (tropasIniciales < 1) throw new ArgumentException("Un territorio no puede quedar con 0 tropas.");
            Nombre = nombre;
            Duenio = duenioInicial;
            Tropas = tropasIniciales;
        }

        public void CambiarDuenio(Ejercito nuevoDuenio)
        {
            Duenio = nuevoDuenio;
        }

        public void AgregarTropas(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad a agregar debe ser positiva.");
            Tropas += cantidad;
        }

        public void QuitarTropas(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad a quitar debe ser positiva.");
            if (Tropas - cantidad < 1) throw new InvalidOperationException("Un territorio no puede quedar vacío (mínimo 1).");
            Tropas -= cantidad;
        }

        public override string ToString() => $"{Nombre} - {Duenio?.Alias ?? "Sin dueño"} - Tropas: {Tropas}";
    }

    // --------------------------
    // EJERCITO
    // --------------------------
    public class Ejercito
    {
        public string Alias { get; private set; }
        public string Color { get; private set; }
        public int TropasDisponibles { get; private set; }

        // (Prototipo con List<T>—luego lo cambiamos por una estructura propia si hace falta)
        public List<Territorio> Territorios { get; private set; } = new List<Territorio>();
        public List<Tarjeta> Tarjetas { get; private set; } = new List<Tarjeta>();

        public Ejercito(string alias, string color, int tropasIniciales)
        {
            Alias = alias;
            Color = color;
            TropasDisponibles = tropasIniciales;
        }

        public bool PuedeColocar(int cantidad) => cantidad > 0 && cantidad <= TropasDisponibles;

        public void AsignarTropa(Territorio territorio, int cantidad)
        {
            if (territorio == null) throw new ArgumentNullException(nameof(territorio));
            if (territorio.Duenio != this) throw new InvalidOperationException("Solo puedes asignar tropas a tus territorios.");
            if (!PuedeColocar(cantidad)) throw new InvalidOperationException("No hay suficientes tropas disponibles.");

            territorio.AgregarTropas(cantidad);
            TropasDisponibles -= cantidad;
        }

        public void RecibirRefuerzos(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("Los refuerzos deben ser positivos.");
            TropasDisponibles += cantidad;
        }

        public void ConquistarTerritorio(Territorio territorio, int tropasMovidas)
        {
            if (territorio == null) throw new ArgumentNullException(nameof(territorio));
            if (tropasMovidas <= 0) throw new ArgumentException("Debes mover al menos 1 tropa.");
            if (tropasMovidas > TropasDisponibles) throw new InvalidOperationException("No tienes tantas tropas para mover.");

            // Si el territorio tenía otro dueño, quítalo de su lista
            if (territorio.Duenio != null && territorio.Duenio != this)
                territorio.Duenio.Territorios.Remove(territorio);

            territorio.CambiarDuenio(this);
            territorio.AgregarTropas(tropasMovidas);
            TropasDisponibles -= tropasMovidas;

            if (!Territorios.Contains(territorio))
                Territorios.Add(territorio);
        }

        public void AñadirTerritorioInicial(Territorio t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (t.Duenio != this) throw new InvalidOperationException("El territorio no pertenece a este ejército.");
            if (!Territorios.Contains(t)) Territorios.Add(t);
        }

        public void RecibirTarjeta(Tarjeta tarjeta)
        {
            if (tarjeta == null) throw new ArgumentNullException(nameof(tarjeta));
            if (Tarjetas.Count >= 6) throw new InvalidOperationException("Máximo 6 tarjetas por ejército.");
            Tarjetas.Add(tarjeta);
        }

        public override string ToString() =>
            $"Ejército {Alias} ({Color}) - Tropas disp.: {TropasDisponibles} | Territorios: {Territorios.Count} | Tarjetas: {Tarjetas.Count}";
    }
}


