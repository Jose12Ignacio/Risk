using System;


public class Dado
{
    protected static Random random = new Random();

    public virtual int Lanzar()
    {
        return random.Next(1, 7); // 1 a 6
    }
}

// Clase hija: Dado del atacante
public class DadoAtacante : Dado
{
    public override int Lanzar()
    {
        // Aquí podrías agregar lógica especial (ventajas del atacante)
        return base.Lanzar();
    }
}

// Clase hija: Dado del defensor
public class DadoDefensor : Dado
{
    public override int Lanzar()
    {
        // Aquí podrías agregar lógica especial (ventajas del defensor)
        return base.Lanzar();
    }
}

public class Batalla
{
    private DadoAtacante dadoAtacante = new DadoAtacante();
    private DadoDefensor dadoDefensor = new DadoDefensor();

    // Lanzar varios dados y ordenarlos de mayor a menor
    private List<int> LanzarDados(Dado dado, int cantidad)
    {
        List<int> resultados = new List<int>();
        for (int i = 0; i < cantidad; i++)
        {
            resultados.Add(dado.Lanzar());
        }
        resultados.Sort((a, b) => b.CompareTo(a)); // ordenar desc
        return resultados;
    }

    // Resolver un combate entre atacante y defensor
    public ResultadoCombate ResolverCombate(
        ref int ejercitosAtacante, 
        ref int ejercitosDefensor, 
        int dadosAtacante, 
        int dadosDefensor)
    {
        ResultadoCombate resultado = new ResultadoCombate();

        // Tiradas
        resultado.TiradaAtacante = LanzarDados(dadoAtacante, dadosAtacante);
        resultado.TiradaDefensor = LanzarDados(dadoDefensor, dadosDefensor);

        // Comparar mayor con mayor, segundo con segundo
        int comparaciones = Math.Min(resultado.TiradaAtacante.Count, resultado.TiradaDefensor.Count);

        for (int i = 0; i < comparaciones; i++)
        {
            int dadoA = resultado.TiradaAtacante[i];
            int dadoD = resultado.TiradaDefensor[i];

            if (dadoA > dadoD)
            {
                ejercitosDefensor--;
                resultado.PerdidasDefensor++;
                resultado.Detalles.Add($"Comparación {i+1}: {dadoA} vs {dadoD} Pierde defensor");
            }
            else
            {
                ejercitosAtacante--;
                resultado.PerdidasAtacante++;
                resultado.Detalles.Add($"Comparación {i+1}: {dadoA} vs {dadoD} Pierde atacante");
            }
        }

        return resultado;
    }
}

public class ResultadoCombate
{
    public List<int> TiradaAtacante { get; set; } = new List<int>();
    public List<int> TiradaDefensor { get; set; } = new List<int>();

    public List<string> Detalles { get; set; } = new List<string>();
    public int PerdidasAtacante { get; set; }
    public int PerdidasDefensor { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        int ejercitosAtacante = 5;
        int ejercitosDefensor = 3;

        Batalla batalla = new Batalla();
        ResultadoCombate resultado = batalla.ResolverCombate(ref ejercitosAtacante, ref ejercitosDefensor, 3, 2);

        Console.WriteLine("Dados Atacante: " + string.Join(", ", resultado.TiradaAtacante));
        Console.WriteLine("Dados Defensor: " + string.Join(", ", resultado.TiradaDefensor));
        foreach (string detalle in resultado.Detalles)
        {
            Console.WriteLine(detalle);
        }
        Console.WriteLine($"Pérdidas atacante: {resultado.PerdidasAtacante}");
        Console.WriteLine($"Pérdidas defensor: {resultado.PerdidasDefensor}");
        Console.WriteLine($"Ejércitos atacante restantes: {ejercitosAtacante}");
        Console.WriteLine($"Ejércitos defensor restantes: {ejercitosDefensor}");
    }
}