using System;

namespace CrazyRisk.Core
{
    public class Batalla
    {
        private DadoAtacante dadoAtacante = new DadoAtacante();
        private DadoDefensor dadoDefensor = new DadoDefensor();

        private int[] LanzarDados(Dado dado, int cantidad)
        {
            int[] resultados = new int[cantidad];
            for (int i = 0; i < cantidad; i++)
                resultados[i] = dado.Lanzar();

            // ordenar descendente (burbuja)
            for (int i = 0; i < cantidad - 1; i++)
                for (int j = i + 1; j < cantidad; j++)
                    if (resultados[i] < resultados[j])
                    {
                        int tmp = resultados[i];
                        resultados[i] = resultados[j];
                        resultados[j] = tmp;
                    }

            return resultados;
        }

        public ResultadoCombate ResolverCombate(
    ref int ejercitosAtacante,
    ref int ejercitosDefensor,
    int dadosAtacante,
    int dadosDefensor,
    string territorioA,
    string territorioD,
    string colorDefensor)
{
    ResultadoCombate resultado = new ResultadoCombate();

    resultado.Tiradas = new int[2][];
    resultado.Tiradas[0] = LanzarDados(dadoAtacante, dadosAtacante);
    resultado.Tiradas[1] = LanzarDados(dadoDefensor, dadosDefensor);

    int comparaciones = Math.Min(resultado.Tiradas[0].Length, resultado.Tiradas[1].Length);
    resultado.Detalles = new string[comparaciones];

    for (int i = 0; i < comparaciones; i++)
    {
        int dadoA = resultado.Tiradas[0][i];
        int dadoD = resultado.Tiradas[1][i];

        if (dadoA > dadoD)
        {
            ejercitosDefensor--;
            resultado.PerdidasDefensor++;
            resultado.Detalles[i] = $"Comparación {i + 1}: {dadoA} vs {dadoD} → Pierde defensor";
        }
        else
        {
            ejercitosAtacante--;
            resultado.PerdidasAtacante++;
            resultado.Detalles[i] = $"Comparación {i + 1}: {dadoA} vs {dadoD} → Pierde atacante";
        }
    }

    resultado.AtacanteColor = User_info.color;
    resultado.DefensorColor = colorDefensor;

    // 🚀 Guardamos las tropas finales
    resultado.TropasAtacanteFinal = ejercitosAtacante;
    resultado.TropasDefensorFinal = ejercitosDefensor;

    return resultado;
}

    }
}

