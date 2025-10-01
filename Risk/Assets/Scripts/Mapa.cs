using System;
using CrazyRisk; // TerritorioId, Continente

namespace CrazyRisk.Core
{
    public class Mapa
    {
        // Array de territorios
        private Territorio[] territorios = new Territorio[42]; // ajusta según total de territorios
        private int count = 0;

        public Territorio Get(TerritorioId id)
        {
            for (int i = 0; i < count; i++)
                if (territorios[i].Id == id)
                    return territorios[i];
            throw new InvalidOperationException($"Territorio no encontrado: {id}");
        }

        public Territorio[] GetAllTerritorios()
        {
            Territorio[] copy = new Territorio[count];
            for (int i = 0; i < count; i++)
                copy[i] = territorios[i];
            return copy;
        }

        public bool SonVecinos(TerritorioId a, TerritorioId b)
        {
            var ta = Get(a);
            for (int i = 0; i < ta.Vecinos.Count(); i++)
                if (ta.Vecinos.Get(i).Equals(b)) return true;
            return false;
        }

        public void Agregar(Territorio t)
        {
            for (int i = 0; i < count; i++)
                if (territorios[i].Id == t.Id)
                    throw new InvalidOperationException($"Duplicado: {t.Id}");

            if (count >= territorios.Length)
                throw new InvalidOperationException("Se alcanzó el máximo de territorios");

            territorios[count] = t;
            count++;
        }

        public void Conectar(TerritorioId a, TerritorioId b)
        {
            var ta = Get(a);
            var tb = Get(b);

            if (!ta.Vecinos.Contains(b)) ta.Vecinos.Add(b);
            if (!tb.Vecinos.Contains(a)) tb.Vecinos.Add(a);
        }

        // Método de ejemplo para crear mapa base
        public static Mapa CrearMapaBase()
        {
            var mapa = new Mapa();

            mapa.Agregar(new Territorio(TerritorioId.Alaska, "Alaska", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.NWTerritory, "NW Territory", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.Alberta, "Alberta", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.Kamchatka, "Kamchatka", Continente.Asia));
            mapa.Agregar(new Territorio(TerritorioId.OesteEEUU, "W. USA", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.Ontario, "Ontario", Continente.AmericaNorte));

            mapa.Conectar(TerritorioId.Alaska, TerritorioId.NWTerritory);
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.Alberta);
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.Kamchatka);
            mapa.Conectar(TerritorioId.Alberta, TerritorioId.Ontario);
            mapa.Conectar(TerritorioId.Alberta, TerritorioId.OesteEEUU);
            mapa.Conectar(TerritorioId.OesteEEUU, TerritorioId.Ontario);

            return mapa;
        }
    }
}
