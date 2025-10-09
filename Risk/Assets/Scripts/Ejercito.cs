#nullable enable
using System;
using System.Collections.Generic;
using CrazyRisk;

namespace CrazyRisk.Core
{
    // La clase Ejercito representa a un jugador o facción dentro del juego.
    // Contiene la información del alias, color, tropas disponibles y tarjetas asociadas.
    public class Ejercito
    {
        // ============================================================
        // Propiedades principales del ejército
        // ============================================================

        public string Alias { get; }  // Nombre o identificador del jugador.
        public string Color { get; }  // Color asociado al ejército (para representación visual en el mapa).
        public int TropasDisponibles { get; private set; } // Número de tropas disponibles para colocar o mover.

        // Listas que almacenan las tropas y tarjetas del ejército.
        public List<Tropa> Tropas { get; } = new List<Tropa>();       // Lista con las tropas individuales.
        public List<Tarjeta> Tarjetas { get; } = new List<Tarjeta>(); // Lista de tarjetas de refuerzo o bonificación.


        // Constructor

        // Inicializa un ejército con alias, color y cantidad inicial de tropas.
        public Ejercito(string alias, string color, int tropasIniciales)
        {
            Alias = alias;
            Color = color;
            TropasDisponibles = tropasIniciales;

            // Crea las tropas iniciales y las agrega a la lista.
            for (int i = 0; i < tropasIniciales; i++)
                Tropas.Add(new Tropa(color));
        }

        // Método: Recibir refuerzos

        // Aumenta la cantidad de tropas disponibles y las agrega a la lista.
        public void RecibirRefuerzos(int cantidad)
        {
            TropasDisponibles += cantidad;

            // Crea nuevas tropas con el color del ejército.
            for (int i = 0; i < cantidad; i++)
                Tropas.Add(new Tropa(Color));
        }


        // Método: Recibir tarjeta

        // Agrega una tarjeta al ejército, pero limita la cantidad máxima a 6.
        public void RecibirTarjeta(Tarjeta tarjeta)
        {
            if (Tarjetas.Count >= 6)
                throw new InvalidOperationException("Máximo 6 tarjetas permitidas.");

            Tarjetas.Add(tarjeta);
        }

 
        // Método: ToString

        // Devuelve una descripción legible del ejército con su alias, color y estadísticas.
        public override string ToString()
            => $"Ejército {Alias} ({Color}) - Tropas: {TropasDisponibles}, Tarjetas: {Tarjetas.Count}";


        // Método: removeTrop
        // Resta una tropa disponible al ejército. 
        // Se usa, por ejemplo, cuando una unidad es eliminada o movida al campo.
        public void removeTrop()
        {
            TropasDisponibles--;
        }
    }
}
