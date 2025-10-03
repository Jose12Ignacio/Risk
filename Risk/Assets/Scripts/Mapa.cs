using System;
using CrazyRisk; // TerritorioId, Continente

namespace CrazyRisk.Core
{
    public class Mapa
    {
        // ===== Modelo array-based =====
        private struct TerrData
        {
            public bool Usado;
            public TerritorioId Id;
            public string Nombre;
            public Continente Continente;
            public int Tropas;

            // Vecinos como array
            public TerritorioId[] Vecinos;   // tamaño fijo razonable
            public int VecinosCount;
        }

        private readonly TerrData[] _t; // indexado por (int)TerritorioId
        private readonly int _maxIds;

        public Mapa()
        {
            _maxIds = Enum.GetValues(typeof(TerritorioId)).Length;
            _t = new TerrData[_maxIds];

            for (int i = 0; i < _maxIds; i++)
            {
                _t[i].Vecinos = new TerritorioId[12]; // capacidad por territorio (ajustable)
                _t[i].VecinosCount = 0;
            }
        }

        // ===== Acceso =====
        public bool Existe(TerritorioId id) => _t[(int)id].Usado;

        public void Agregar(TerritorioId id, string nombre, Continente c, int tropas = 0)
        {
            int idx = (int)id;
            if (_t[idx].Usado) throw new InvalidOperationException($"Duplicado: {id}");
            _t[idx].Usado = true;
            _t[idx].Id = id;
            _t[idx].Nombre = nombre ?? id.ToString();
            _t[idx].Continente = c;
            _t[idx].Tropas = tropas < 0 ? 0 : tropas;
        }

        public string GetName(TerritorioId id)
        {
            var td = _t[(int)id];
            if (!td.Usado) throw new ArgumentException($"Territorio no encontrado: {id}");
            return td.Nombre;
        }

        public int GetTroops(TerritorioId id)
        {
            var td = _t[(int)id];
            if (!td.Usado) throw new ArgumentException($"Territorio no encontrado: {id}");
            return td.Tropas;
        }

        // Devuelve todos los IDs existentes en buffer (y el count por out)
        public void GetAllIds(TerritorioId[] buffer, out int count)
        {
            count = 0;
            for (int i = 0; i < _maxIds; i++)
                if (_t[i].Usado) { buffer[count++] = (TerritorioId)i; }


        public bool SonVecinos(TerritorioId a, TerritorioId b)
        {
            var ta = _t[(int)a];
            if (!ta.Usado) return false;
            for (int i = 0; i < ta.VecinosCount; i++)
                if (ta.Vecinos[i] == b) return true;
            return false;
        }

        public void GetVecinos(TerritorioId a, TerritorioId[] buffer, out int count)
        {
            var ta = _t[(int)a];
            count = 0;
            if (!ta.Usado) return;
            for (int i = 0; i < ta.VecinosCount; i++)
                buffer[count++] = ta.Vecinos[i];
        }

        private void AddVecino(TerritorioId a, TerritorioId b)
        {
            int ia = (int)a;
            // evitar duplicados
            for (int i = 0; i < _t[ia].VecinosCount; i++)
                if (_t[ia].Vecinos[i] == b) return;

            if (_t[ia].VecinosCount < _t[ia].Vecinos.Length)
                _t[ia].Vecinos[_t[ia].VecinosCount++] = b;
            else
                throw new InvalidOperationException($"Capacidad de vecinos llena para {a}");
        }

        public void Conectar(TerritorioId a, TerritorioId b)
        {
            if (!Existe(a) || !Existe(b)) throw new InvalidOperationException("Conectar: territorio no existe.");
            AddVecino(a, b);
            AddVecino(b, a);
        }

        // ===== Construcción del mapa base (arrays only) =====
        public static Mapa CrearMapaBase()
        {
            var mapa = new Mapa();

            // América del Norte (9)
            mapa.Agregar(TerritorioId.Alaska,         "Alaska",              Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.NWTerritory,    "NW Territory",        Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.Groenlandia,    "Groenlandia",         Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.Alberta,        "Alberta",             Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.Ontario,        "Ontario",             Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.Quebec,         "Quebec",              Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.OesteEEUU,      "O EEUU",          Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.EsteEEUU,       "E EEUU",           Continente.AmericaNorte);
            mapa.Agregar(TerritorioId.CentroAmerica,  "Centroamérica",       Continente.AmericaNorte);

            // Sudamérica (4)
            mapa.Agregar(TerritorioId.Venezuela,      "Venezuela",           Continente.AmericaSur);
            mapa.Agregar(TerritorioId.Peru,           "Perú",                Continente.AmericaSur);
            mapa.Agregar(TerritorioId.Brasil,         "Brasil",              Continente.AmericaSur);
            mapa.Agregar(TerritorioId.Argentina,      "Argentina",           Continente.AmericaSur);

            // Europa (7)
            mapa.Agregar(TerritorioId.Islandia,       "Islandia",            Continente.Europa);
            mapa.Agregar(TerritorioId.GranBretana,    "Gran Bretaña",        Continente.Europa);
            mapa.Agregar(TerritorioId.Escandinavia,   "Escandinavia",        Continente.Europa);
            mapa.Agregar(TerritorioId.EuropaNorte,    "Europa Norte",    Continente.Europa);
            mapa.Agregar(TerritorioId.EuropaOccidental,"Europa Occidental",  Continente.Europa);
            mapa.Agregar(TerritorioId.EuropaSur,      "Europa Sur",      Continente.Europa);
            mapa.Agregar(TerritorioId.Ucrania,        "Ucrania",             Continente.Europa);

            // África (6)
            mapa.Agregar(TerritorioId.AfricaNorte,    "África Norte",    Continente.Africa);
            mapa.Agregar(TerritorioId.Egipto,         "Egipto",              Continente.Africa);
            mapa.Agregar(TerritorioId.AfricaEste,     "África Este",     Continente.Africa);
            mapa.Agregar(TerritorioId.Congo,          "Congo",               Continente.Africa);
            mapa.Agregar(TerritorioId.AfricaSur,      "África Sur",      Continente.Africa);
            mapa.Agregar(TerritorioId.Madagascar,     "Madagascar",          Continente.Africa);

            // Asia (12)
            mapa.Agregar(TerritorioId.Ural,           "Ural",                Continente.Asia);
            mapa.Agregar(TerritorioId.Siberia,        "Siberia",             Continente.Asia);
            mapa.Agregar(TerritorioId.Yakutsk,        "Yakutsk",             Continente.Asia);
            mapa.Agregar(TerritorioId.Kamchatka,      "Kamchatka",           Continente.Asia);
            mapa.Agregar(TerritorioId.Irkutsk,        "Irkutsk",             Continente.Asia);
            mapa.Agregar(TerritorioId.Mongolia,       "Mongolia",            Continente.Asia);
            mapa.Agregar(TerritorioId.Japon,          "Japón",               Continente.Asia);
            mapa.Agregar(TerritorioId.China,          "China",               Continente.Asia);
            mapa.Agregar(TerritorioId.MedioOriente,   "Medio Oriente",       Continente.Asia);
            mapa.Agregar(TerritorioId.India,          "India",               Continente.Asia);
            mapa.Agregar(TerritorioId.Siam,           "Siam",                Continente.Asia);
            mapa.Agregar(TerritorioId.Afganistan,     "Afganistán",          Continente.Asia);

            // Oceanía (4)
            mapa.Agregar(TerritorioId.Indonesia,          "Indonesia",        Continente.Oceania);
            mapa.Agregar(TerritorioId.NuevaGuinea,        "Nueva Guinea",     Continente.Oceania);
            mapa.Agregar(TerritorioId.AustraliaOccidental,"Australia Occ.",   Continente.Oceania);
            mapa.Agregar(TerritorioId.AustraliaOriental,  "Australia Ori.",   Continente.Oceania);

            // Conexiones (bidireccionales)
            // América del Norte
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.NWTerritory);
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.Alberta);
            mapa.Conectar(TerritorioId.Alaska, TerritorioId.Kamchatka);

            mapa.Conectar(TerritorioId.NWTerritory, TerritorioId.Alberta);
            mapa.Conectar(TerritorioId.NWTerritory, TerritorioId.Ontario);
            mapa.Conectar(TerritorioId.NWTerritory, TerritorioId.Groenlandia);

            mapa.Conectar(TerritorioId.Groenlandia, TerritorioId.Ontario);
            mapa.Conectar(TerritorioId.Groenlandia, TerritorioId.Quebec);
            mapa.Conectar(TerritorioId.Groenlandia, TerritorioId.Islandia);

            mapa.Conectar(TerritorioId.Alberta, TerritorioId.Ontario);
            mapa.Conectar(TerritorioId.Alberta, TerritorioId.OesteEEUU);

            mapa.Conectar(TerritorioId.Ontario, TerritorioId.Quebec);
            mapa.Conectar(TerritorioId.Ontario, TerritorioId.EsteEEUU);
            mapa.Conectar(TerritorioId.Ontario, TerritorioId.OesteEEUU);

            mapa.Conectar(TerritorioId.Quebec, TerritorioId.EsteEEUU);

            mapa.Conectar(TerritorioId.OesteEEUU, TerritorioId.EsteEEUU);
            mapa.Conectar(TerritorioId.OesteEEUU, TerritorioId.CentroAmerica);

            mapa.Conectar(TerritorioId.EsteEEUU, TerritorioId.CentroAmerica);

            mapa.Conectar(TerritorioId.CentroAmerica, TerritorioId.Venezuela);

            // Sudamérica
            mapa.Conectar(TerritorioId.Venezuela, TerritorioId.Peru);
            mapa.Conectar(TerritorioId.Venezuela, TerritorioId.Brasil);

            mapa.Conectar(TerritorioId.Peru, TerritorioId.Brasil);
            mapa.Conectar(TerritorioId.Peru, TerritorioId.Argentina);

            mapa.Conectar(TerritorioId.Brasil, TerritorioId.Argentina);
            mapa.Conectar(TerritorioId.Brasil, TerritorioId.AfricaNorte);

            // Europa
            mapa.Conectar(TerritorioId.Islandia, TerritorioId.GranBretana);
            mapa.Conectar(TerritorioId.Islandia, TerritorioId.Escandinavia);

            mapa.Conectar(TerritorioId.GranBretana, TerritorioId.Escandinavia);
            mapa.Conectar(TerritorioId.GranBretana, TerritorioId.EuropaNorte);
            mapa.Conectar(TerritorioId.GranBretana, TerritorioId.EuropaOccidental);

            mapa.Conectar(TerritorioId.Escandinavia, TerritorioId.EuropaNorte);
            mapa.Conectar(TerritorioId.Escandinavia, TerritorioId.Ucrania);

            mapa.Conectar(TerritorioId.EuropaNorte, TerritorioId.EuropaOccidental);
            mapa.Conectar(TerritorioId.EuropaNorte, TerritorioId.EuropaSur);
            mapa.Conectar(TerritorioId.EuropaNorte, TerritorioId.Ucrania);

            mapa.Conectar(TerritorioId.EuropaOccidental, TerritorioId.EuropaSur);
            mapa.Conectar(TerritorioId.EuropaOccidental, TerritorioId.AfricaNorte);

            mapa.Conectar(TerritorioId.EuropaSur, TerritorioId.Ucrania);
            mapa.Conectar(TerritorioId.EuropaSur, TerritorioId.Egipto);
            mapa.Conectar(TerritorioId.EuropaSur, TerritorioId.AfricaNorte);
            mapa.Conectar(TerritorioId.EuropaSur, TerritorioId.MedioOriente);

            mapa.Conectar(TerritorioId.Ucrania, TerritorioId.Ural);
            mapa.Conectar(TerritorioId.Ucrania, TerritorioId.Afganistan);
            mapa.Conectar(TerritorioId.Ucrania, TerritorioId.MedioOriente);

            // África
            mapa.Conectar(TerritorioId.AfricaNorte, TerritorioId.Egipto);
            mapa.Conectar(TerritorioId.AfricaNorte, TerritorioId.AfricaEste);
            mapa.Conectar(TerritorioId.AfricaNorte, TerritorioId.Congo);

            mapa.Conectar(TerritorioId.Egipto, TerritorioId.AfricaEste);
            mapa.Conectar(TerritorioId.Egipto, TerritorioId.MedioOriente);

            mapa.Conectar(TerritorioId.AfricaEste, TerritorioId.Congo);
            mapa.Conectar(TerritorioId.AfricaEste, TerritorioId.AfricaSur);
            mapa.Conectar(TerritorioId.AfricaEste, TerritorioId.Madagascar);
            mapa.Conectar(TerritorioId.AfricaEste, TerritorioId.MedioOriente);

            mapa.Conectar(TerritorioId.Congo, TerritorioId.AfricaSur);

            mapa.Conectar(TerritorioId.AfricaSur, TerritorioId.Madagascar);

            // Asia
            mapa.Conectar(TerritorioId.Ural, TerritorioId.Siberia);
            mapa.Conectar(TerritorioId.Ural, TerritorioId.China);
            mapa.Conectar(TerritorioId.Ural, TerritorioId.Afganistan);

            mapa.Conectar(TerritorioId.Siberia, TerritorioId.Yakutsk);
            mapa.Conectar(TerritorioId.Siberia, TerritorioId.Irkutsk);
            mapa.Conectar(TerritorioId.Siberia, TerritorioId.Mongolia);
            mapa.Conectar(TerritorioId.Siberia, TerritorioId.China);

            mapa.Conectar(TerritorioId.Yakutsk, TerritorioId.Irkutsk);
            mapa.Conectar(TerritorioId.Yakutsk, TerritorioId.Kamchatka);

            mapa.Conectar(TerritorioId.Irkutsk, TerritorioId.Kamchatka);
            mapa.Conectar(TerritorioId.Irkutsk, TerritorioId.Mongolia);

            mapa.Conectar(TerritorioId.Mongolia, TerritorioId.Kamchatka);
            mapa.Conectar(TerritorioId.Mongolia, TerritorioId.China);

            mapa.Conectar(TerritorioId.Japon, TerritorioId.Kamchatka);
            mapa.Conectar(TerritorioId.Japon, TerritorioId.Mongolia);

            mapa.Conectar(TerritorioId.China, TerritorioId.Afganistan);
            mapa.Conectar(TerritorioId.China, TerritorioId.India);
            mapa.Conectar(TerritorioId.China, TerritorioId.Siam);

            mapa.Conectar(TerritorioId.Afganistan, TerritorioId.India);
            mapa.Conectar(TerritorioId.Afganistan, TerritorioId.MedioOriente);

            mapa.Conectar(TerritorioId.India, TerritorioId.MedioOriente);
            mapa.Conectar(TerritorioId.India, TerritorioId.Siam);

            mapa.Conectar(TerritorioId.MedioOriente, TerritorioId.Siam);
            mapa.Conectar(TerritorioId.MedioOriente, TerritorioId.Egipto);
            mapa.Conectar(TerritorioId.MedioOriente, TerritorioId.EuropaSur);
            mapa.Conectar(TerritorioId.MedioOriente, TerritorioId.AfricaEste);

            // Oceanía
            mapa.Conectar(TerritorioId.Indonesia, TerritorioId.Siam);
            mapa.Conectar(TerritorioId.Indonesia, TerritorioId.NuevaGuinea);
            mapa.Conectar(TerritorioId.Indonesia, TerritorioId.AustraliaOccidental);

            mapa.Conectar(TerritorioId.NuevaGuinea, TerritorioId.AustraliaOccidental);
            mapa.Conectar(TerritorioId.NuevaGuinea, TerritorioId.AustraliaOriental);

            mapa.Conectar(TerritorioId.AustraliaOccidental, TerritorioId.AustraliaOriental);

            return mapa;
        }
    }
}

