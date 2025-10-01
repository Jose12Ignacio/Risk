using System;
using System.Collections.Generic;
using CrazyRisk; // para TerritorioId y Continente

namespace CrazyRisk.Core
{
    public class Mapa
    {
        // Acceso rápido por Id
        private readonly Dictionary<TerritorioId, Territorio> _territorios =
            new Dictionary<TerritorioId, Territorio>();

        public IReadOnlyDictionary<TerritorioId, Territorio> Territorios => _territorios;

        public Territorio Get(TerritorioId id) =>
            _territorios.TryGetValue(id, out var t) ? t :
            throw new KeyNotFoundException($"Territorio no encontrado: {id}");

        public bool SonVecinos(TerritorioId a, TerritorioId b)
        {
            var ta = Get(a);
            for (int i = 0; i < ta.Vecinos.Count; i++)
                if (ta.Vecinos[i].Equals(b)) return true;
            return false;
        }

        // ===== Construcción del mapa =====
        public static Mapa CrearMapaBase()
        {
            var mapa = new Mapa();

            // --- EJEMPLO con unos pocos territorios (completa los 42) ---
            // Crea territorios
            mapa.Agregar(new Territorio(TerritorioId.Alaska, "Alaska", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.NWTerritory, "NW Territory", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.Alberta, "Alberta", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.Kamchatka, "Kamchatka", Continente.Asia));
            mapa.Agregar(new Territorio(TerritorioId.OesteEEUU, "W. USA", Continente.AmericaNorte));
            mapa.Agregar(new Territorio(TerritorioId.Ontario, "Ontario", Continente.AmericaNorte));

            // Conexiones (siempre bidireccionales)
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.NWTerritory);
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.Alberta);
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.Kamchatka);
            mapa.Conectar(TerritorioId.Alberta, TerritorioId.Ontario);
            mapa.Conectar(TerritorioId.Alberta, TerritorioId.OesteEEUU);
            mapa.Conectar(TerritorioId.OesteEEUU, TerritorioId.Ontario);
            // --- FIN EJEMPLO ---

            return mapa;
        }

        // ===== utilidades internas =====
        public void Agregar(Territorio t)
        {
            if (_territorios.ContainsKey(t.Id))
                throw new InvalidOperationException($"Duplicado: {t.Id}");
            _territorios.Add(t.Id, t);
        }

        public void Conectar(TerritorioId a, TerritorioId b)
        {
            var ta = Get(a);
            var tb = Get(b);
            if (!ta.Vecinos.Contains(b)) ta.Vecinos.Add(b);
            if (!tb.Vecinos.Contains(a)) tb.Vecinos.Add(a);
        }
    }
}
