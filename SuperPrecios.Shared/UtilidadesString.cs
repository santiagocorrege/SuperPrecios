using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Shared
{
    public static class UtilidadesString
    {
        public static string FormatearTexto(string texto)
        {
            texto = texto.Trim();
            string[] palabras = texto.Split(' ');
            for (int i = 0; i < palabras.Length; i++)
            {
                if (palabras[i].Length > 0)
                {
                    palabras[i] = char.ToUpper(palabras[i][0]) + palabras[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", palabras);
        }
    }
}
