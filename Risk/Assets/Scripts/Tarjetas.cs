using System;
using System.Collections.Generic;

namespace CrazyRisk.Core
{
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

        public override string ToString() => EsComodin ? "Comodín" : $"{Tipo} - {Territorio}";
    }

    public class MazoTarjetas
    {
        private readonly List<Tarjeta> _mazo = new List<Tarjeta>();
        private readonly List<Tarjeta> _descarte = new List<Tarjeta>();
        private readonly Random _rng = new Random();

        public int Count => _mazo.Count;
        public int CountDescarte => _descarte.Count;

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

                _mazo.AddRange(_descarte);
                _descarte.Clear();
                Barajar();
            }

            var top = _mazo[_mazo.Count - 1]; // equivalente a _mazo[^1];
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

            if (inf + comodines >= 3 && inf > 0) return true;
            if (cab + comodines >= 3 && cab > 0) return true;
            if (art + comodines >= 3 && art > 0) return true;

            int faltantes = (inf == 0 ? 1 : 0) + (cab == 0 ? 1 : 0) + (art == 0 ? 1 : 0);
            if (faltantes <= comodines) return true;

            if (comodines == 2 && (inf + cab + art) == 1) return true;

            return false;
        }
    }
}

