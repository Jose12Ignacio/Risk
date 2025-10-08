using System;
using Newtonsoft.Json;
using CrazyRisk;
using CrazyRisk.Core;

[Serializable]
public class TerritorioDTO
{
    public string nombre;
    public string duenio;
    public int tropas;
    public string continente;
    public string id;

    public TerritorioDTO() { }

    public TerritorioDTO(Territorio territorio)
    {
        if (territorio == null) return;

        nombre = territorio.Nombre;
        duenio = territorio.Duenio != null ? territorio.Duenio.Alias : "Sin dueño";
        tropas = territorio.Tropas;
        continente = territorio.Continente.ToString();
        id = territorio.Id.ToString();
    }

    public Territorio ToTerritorio()
    {
        // Convierte de nuevo el DTO al objeto Territorio real
        if (!Enum.TryParse(id, out TerritorioId terrId))
            terrId = TerritorioId.Alaska; // fallback mínimo

        if (!Enum.TryParse(continente, out Continente cont))
            cont = Continente.AmericaNorte;

        // Crear nuevo territorio con su continente
        Territorio territorio = new Territorio(terrId, cont);

        // Si tu clase Territorio tiene un método para agregar tropas, úsalo
        territorio.AgregarTropas(tropas);

        // Si tiene un método para cambiar dueño, puedes dejarlo preparado
        // El dueño real se asignará desde TurnInfo/GameManager

        return territorio;
    }
}
