using System;
using System.Collections.Generic;

namespace CrazyRisk.Core
{
    public enum TipoTarjeta
    {
        Infanteria,
        Caballeria,
        Artilleria,
        Comodin
    }

    public class Tarjeta
    {
        public TipoTarjeta Tipo { get; }
        public TerritorioId? Territorio { get; }  // null para comodines
        public bool EsComodin => Tipo == TipoTarjeta.Comodin;

        public Tarjeta(TipoTarjeta tipo, TerritorioId? territorio = null)
        {
            Tipo = tipo;
            Territorio = territorio;
        }

        public override string ToString()
            => EsComodin ? "Comodín" : $"{Tipo} - {Territorio}";
    }

    public class MazoTarjetas
    {
        private readonly List<Tarjeta> _mazo = new List<Tarjeta>();
        private readonly List<Tarjeta> _descarte = new List<Tarjeta>();
        private readonly Random _rng = new Random();

        public int Count => _mazo.Count;
        public int CountDescarte => _descarte.Count;

        public MazoTarjetas()
        {
            // Aquí más adelante vas a poblar el mazo con las 42 cartas + 2 comodines
        }

        public void Barajar()
        {
            for (int i = _mazo.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (_mazo[i], _mazo[j]) = (_mazo[j], _mazo[i]);
            }
        }

        public Tarjeta Robar()
        {
            if (_mazo.Count == 0)
            {
                if (_descarte.Count == 0)
                    throw new InvalidOperationException("No hay cartas en el mazo ni en el descarte.");

                // reinyectar descarte en el mazo
                _mazo.AddRange(_descarte);
                _descarte.Clear();
                Barajar();
            }

            var top = _mazo[^1];
            _mazo.RemoveAt(_mazo.Count - 1);
            return top;
        }

        public void Descartar(Tarjeta tarjeta) => _descarte.Add(tarjeta);

        public static bool EsTrioValido(IList<Tarjeta> trio)
        {
            if (trio == null || trio.Count != 3) return false;

            int comodines = 0, inf = 0, cab = 0, art = 0;

            foreach (var t in trio)
            {
                if (t.EsComodin) { comodines++; continue; }
                if (t.Tipo == TipoTarjeta.Infanteria) inf++;
                else if (t.Tipo == TipoTarjeta.Caballeria) cab++;
                else if (t.Tipo == TipoTarjeta.Artilleria) art++;
            }

            // Tres iguales (considerando comodines)
            if (inf + comodines >= 3 && inf > 0) return true;
            if (cab + comodines >= 3 && cab > 0) return true;
            if (art + comodines >= 3 && art > 0) return true;

            // Uno de cada (comodines completan faltantes)
            int faltantes = 0;
            if (inf == 0) faltantes++;
            if (cab == 0) faltantes++;
            if (art == 0) faltantes++;
            if (faltantes <= comodines) return true;

            // Dos comodines + 1 cualquiera
            if (comodines == 2 && (inf + cab + art) == 1) return true;

            return false;
        }

        // 👉 Aquí luego harás un método para poblar el mazo base (42 territorios + 2 comodines)
        // private void ConstruirMazoBase() { ... }
    }
}
