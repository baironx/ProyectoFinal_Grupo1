using System;
using System.Globalization;

namespace AppEntrenamientoPersonal.UI
{
    /// <summary>
    /// Clase para manejar la interfaz de usuario de consola.
    /// Implementa el principio Single Responsibility (SRP).
    /// </summary>
    public class InterfazUsuario
    {
        #region Delegates

        /// <summary>
        /// Delegate para validación de entrada personalizada.
        /// </summary>
        public delegate bool ValidadorEntrada(string entrada);

        /// <summary>
        /// Delegate para transformación de entrada.
        /// </summary>
        public delegate string TransformadorEntrada(string entrada);

        #endregion

        #region Constantes de Color

        private const string COLOR_TITULO = "\u001b[36m";      // Cyan
        private const string COLOR_EXITO = "\u001b[32m";       // Verde
        private const string COLOR_ERROR = "\u001b[31m";       // Rojo
        private const string COLOR_ADVERTENCIA = "\u001b[33m"; // Amarillo
        private const string COLOR_INFO = "\u001b[37m";        // Blanco
        private const string COLOR_RESET = "\u001b[0m";        // Reset

        #endregion

        #region Métodos de Salida

        /// <summary>
        /// Muestra un título principal.
        /// </summary>
        public void MostrarTitulo(string titulo)
        {
            Console.WriteLine();
            Console.WriteLine($"{COLOR_TITULO}{'=' * titulo.Length}{COLOR_RESET}");
            Console.WriteLine($"{COLOR_TITULO}{titulo}{COLOR_RESET}");
            Console.WriteLine($"{COLOR_TITULO}{'=' * titulo.Length}{COLOR_RESET}");
            Console.WriteLine();
        }

        /// <summary>
        /// Muestra un subtítulo.
        /// </summary>
        public void MostrarSubTitulo(string subtitulo)
        {
            Console.WriteLine();
            Console.WriteLine($"{COLOR_TITULO}{subtitulo}{COLOR_RESET}");
            Console.WriteLine($"{COLOR_TITULO}{new string('-', subtitulo.Length)}{COLOR_RESET}");
        }

        /// <summary>
        /// Muestra un menú de opciones.
        /// </summary>
        public void MostrarMenu(string[] opciones)
        {
            Console.WriteLine();
            foreach (var opcion in opciones)
            {
                Console.WriteLine($"{COLOR_INFO}{opcion}{COLOR_RESET}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Muestra un mensaje de éxito.
        /// </summary>
        public void MostrarExito(string mensaje)
        {
            Console.WriteLine($"{COLOR_EXITO} {mensaje}{COLOR_RESET}");
        }

        /// <summary>
        /// Muestra un mensaje de error.
        /// </summary>
        public void MostrarError(string mensaje)
        {
            Console.WriteLine($"{COLOR_ERROR} {mensaje}{COLOR_RESET}");
        }

        /// <summary>
        /// Muestra un mensaje de advertencia.
        /// </summary>
        public void MostrarAdvertencia(string mensaje)
        {
            Console.WriteLine($"{COLOR_ADVERTENCIA} {mensaje}{COLOR_RESET}");
        }

        /// <summary>
        /// Muestra información general.
        /// </summary>
        public void MostrarInformacion(string mensaje)
        {
            Console.WriteLine($"{COLOR_INFO}{mensaje}{COLOR_RESET}");
        }

        /// <summary>
        /// Limpia la pantalla de la consola.
        /// </summary>
        public void LimpiarPantalla()
        {
            Console.Clear();
        }

        /// <summary>
        /// Espera a que el usuario presione una tecla.
        /// </summary>
        public void EsperarTecla()
        {
            Console.WriteLine();
            Console.WriteLine($"{COLOR_INFO}Presione cualquier tecla para continuar...{COLOR_RESET}");
            Console.ReadKey(true);
        }

        #endregion

        #region Métodos de Entrada

        /// <summary>
        /// Lee una entrada de texto del usuario.
        /// </summary>
        public string LeerEntrada(string prompt)
        {
            Console.Write($"{COLOR_INFO}{prompt}{COLOR_RESET}");
            return Console.ReadLine() ?? string.Empty;
        }

        /// <summary>
        /// Lee una entrada con validación personalizada.
        /// </summary>
        public string LeerEntradaValidada(string prompt, ValidadorEntrada validador, string mensajeError = "Entrada inválida")
        {
            string entrada;
            do
            {
                entrada = LeerEntrada(prompt);
                if (!validador(entrada))
                {
                    MostrarError(mensajeError);
                }
            } while (!validador(entrada));

            return entrada;
        }

        /// <summary>
        /// Lee una entrada opcional (permite vacío).
        /// </summary>
        public string LeerEntradaOpcional(string prompt, string valorPorDefecto = "")
        {
            var entrada = LeerEntrada(prompt);
            return string.IsNullOrWhiteSpace(entrada) ? valorPorDefecto : entrada;
        }

        /// <summary>
        /// Lee un número entero con validación.
        /// </summary>
        public int LeerEntero(string prompt, int minimo = int.MinValue, int maximo = int.MaxValue)
        {
            return LeerNumero<int>(prompt, int.TryParse, minimo, maximo);
        }

        /// <summary>
        /// Lee un número double con validación.
        /// </summary>
        public double LeerDouble(string prompt, double minimo = double.MinValue, double maximo = double.MaxValue)
        {
            return LeerNumero<double>(prompt, double.TryParse, minimo, maximo);
        }

        /// <summary>
        /// Lee un número decimal con validación.
        /// </summary>
        public decimal LeerDecimal(string prompt, decimal minimo = decimal.MinValue, decimal maximo = decimal.MaxValue)
        {
            return LeerNumero<decimal>(prompt, decimal.TryParse, minimo, maximo);
        }

        /// <summary>
        /// Lee un número entero opcional.
        /// </summary>
        public int LeerEnteroOpcional(string prompt, int valorPorDefecto)
        {
            var entrada = LeerEntradaOpcional(prompt);
            return int.TryParse(entrada, out int valor) ? valor : valorPorDefecto;
        }

        /// <summary>
        /// Lee un número double opcional.
        /// </summary>
        public double LeerDoubleOpcional(string prompt, double valorPorDefecto)
        {
            var entrada = LeerEntradaOpcional(prompt);
            return double.TryParse(entrada, out double valor) ? valor : valorPorDefecto;
        }

        /// <summary>
        /// Lee una fecha con validación.
        /// </summary>
        public DateTime LeerFecha(string prompt, DateTime? valorPorDefecto = null)
        {
            while (true)
            {
                var entrada = LeerEntrada(prompt);

                if (string.IsNullOrWhiteSpace(entrada) && valorPorDefecto.HasValue)
                {
                    return valorPorDefecto.Value;
                }

                if (DateTime.TryParse(entrada, out DateTime fecha))
                {
                    return fecha;
                }

                MostrarError("Formato de fecha inválido. Use AAAA-MM-DD o DD/MM/AAAA");
            }
        }

        /// <summary>
        /// Lee una fecha opcional.
        /// </summary>
        public DateTime? LeerFechaOpcional(string prompt)
        {
            var entrada = LeerEntradaOpcional(prompt);

            if (string.IsNullOrWhiteSpace(entrada))
                return null;

            return DateTime.TryParse(entrada, out DateTime fecha) ? fecha : null;
        }

        /// <summary>
        /// Lee confirmación del usuario (s/n).
        /// </summary>
        public bool LeerConfirmacion(string prompt, bool valorPorDefecto = false)
        {
            var defaultText = valorPorDefecto ? "(S/n)" : "(s/N)";
            var entrada = LeerEntrada($"{prompt} {defaultText}: ").ToLower();

            if (string.IsNullOrWhiteSpace(entrada))
                return valorPorDefecto;

            return entrada.StartsWith("s") || entrada.StartsWith("y");
        }

        #endregion

        #region Métodos Privados Genéricos

        /// <summary>
        /// Método genérico para leer números con validación.
        /// </summary>
        private T LeerNumero<T>(string prompt, TryParseDelegate<T> tryParse, T minimo, T maximo)
            where T : struct, IComparable<T>
        {
            while (true)
            {
                var entrada = LeerEntrada(prompt);

                if (tryParse(entrada, out T numero))
                {
                    if (numero.CompareTo(minimo) >= 0 && numero.CompareTo(maximo) <= 0)
                    {
                        return numero;
                    }
                    else
                    {
                        MostrarError($"El valor debe estar entre {minimo} y {maximo}");
                    }
                }
                else
                {
                    MostrarError($"Debe ingresar un número válido de tipo {typeof(T).Name}");
                }
            }
        }

        /// <summary>
        /// Delegate para métodos TryParse genéricos.
        /// </summary>
        private delegate bool TryParseDelegate<T>(string input, out T result);

        #endregion

        #region Métodos de Utilidad

        /// <summary>
        /// Muestra una lista paginada de elementos.
        /// </summary>
        public void MostrarListaPaginada<T>(IEnumerable<T> elementos, int elementosPorPagina = 10,
                                           Func<T, string> formatoElemento = null!)
        {
            var lista = elementos.ToList();
            if (!lista.Any())
            {
                MostrarAdvertencia("No hay elementos para mostrar");
                return;
            }

            formatoElemento ??= (elemento => elemento!.ToString()!);

            var totalPaginas = (int)Math.Ceiling((double)lista.Count / elementosPorPagina);
            var paginaActual = 1;

            while (true)
            {
                LimpiarPantalla();
                MostrarSubTitulo($"Página {paginaActual} de {totalPaginas}");

                var elementosPagina = lista
                    .Skip((paginaActual - 1) * elementosPorPagina)
                    .Take(elementosPorPagina);

                var indiceInicial = (paginaActual - 1) * elementosPorPagina;
                foreach (var (elemento, indice) in elementosPagina.Select((e, i) => (e, i)))
                {
                    MostrarInformacion($"{indiceInicial + indice + 1}. {formatoElemento(elemento)}");
                }

                Console.WriteLine();
                MostrarInformacion("Navegación: [A]nterior | [S]iguiente | [Q] Salir");

                var tecla = Console.ReadKey(true).Key;

                switch (tecla)
                {
                    case ConsoleKey.A:
                        if (paginaActual > 1) paginaActual--;
                        break;
                    case ConsoleKey.S:
                        if (paginaActual < totalPaginas) paginaActual++;
                        break;
                    case ConsoleKey.Q:
                        return;
                }
            }
        }

        /// <summary>
        /// Muestra una tabla simple de datos.
        /// </summary>
        public void MostrarTabla<T>(IEnumerable<T> datos, params (string Header, Func<T, string> Selector)[] columnas)
        {
            var lista = datos.ToList();
            if (!lista.Any())
            {
                MostrarAdvertencia("No hay datos para mostrar");
                return;
            }

            // Calcular anchos de columna
            var anchos = columnas.Select((col, index) =>
            {
                var anchoHeader = col.Header.Length;
                var anchoDatos = lista.Max(item => col.Selector(item)?.Length ?? 0);
                return Math.Max(anchoHeader, anchoDatos) + 2;
            }).ToArray();

            // Mostrar encabezados
            Console.WriteLine();
            for (int i = 0; i < columnas.Length; i++)
            {
                Console.Write($"{COLOR_TITULO}{columnas[i].Header.PadRight(anchos[i])}{COLOR_RESET}");
            }
            Console.WriteLine();

            // Mostrar línea separadora
            for (int i = 0; i < columnas.Length; i++)
            {
                Console.Write($"{COLOR_TITULO}{new string('-', anchos[i])}{COLOR_RESET}");
            }
            Console.WriteLine();

            // Mostrar datos
            foreach (var item in lista)
            {
                for (int i = 0; i < columnas.Length; i++)
                {
                    var valor = columnas[i].Selector(item) ?? "";
                    Console.Write($"{COLOR_INFO}{valor.PadRight(anchos[i])}{COLOR_RESET}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Muestra un indicador de progreso simple.
        /// </summary>
        public void MostrarProgreso(int actual, int total, string descripcion = "Progreso")
        {
            var porcentaje = (double)actual / total * 100;
            var barras = (int)(porcentaje / 5); // 20 caracteres máximo
            var progreso = new string('█', barras) + new string('░', 20 - barras);

            Console.Write($"\r{COLOR_INFO}{descripcion}: [{progreso}] {porcentaje:F1}% ({actual}/{total}){COLOR_RESET}");

            if (actual == total)
            {
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Muestra un menú de selección y retorna la opción elegida.
        /// </summary>
        public T MostrarMenuSeleccion<T>(string titulo, Dictionary<string, T> opciones)
        {
            while (true)
            {
                MostrarSubTitulo(titulo);

                var keys = opciones.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    MostrarInformacion($"{i + 1}. {keys[i]}");
                }

                var seleccion = LeerEntero("\nSeleccione una opción: ", 1, keys.Length);
                return opciones[keys[seleccion - 1]];
            }
        }

        #endregion

        #region Métodos de Validación de Entrada

        /// <summary>
        /// Validador para entradas no vacías.
        /// </summary>
        public static readonly ValidadorEntrada ValidadorNoVacio = entrada => !string.IsNullOrWhiteSpace(entrada);

        /// <summary>
        /// Validador para emails (básico).
        /// </summary>
        public static readonly ValidadorEntrada ValidadorEmail = entrada =>
            !string.IsNullOrWhiteSpace(entrada) && entrada.Contains("@") && entrada.Contains(".");

        /// <summary>
        /// Crea un validador para longitud mínima.
        /// </summary>
        public static ValidadorEntrada CrearValidadorLongitudMinima(int longitud) =>
            entrada => !string.IsNullOrWhiteSpace(entrada) && entrada.Length >= longitud;

        /// <summary>
        /// Crea un validador para opciones específicas.
        /// </summary>
        public static ValidadorEntrada CrearValidadorOpciones(params string[] opcionesValidas) =>
            entrada => opcionesValidas.Any(opcion => opcion.Equals(entrada, StringComparison.OrdinalIgnoreCase));

        #endregion
    }
}